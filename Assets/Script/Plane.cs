using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace flight.one
{
    [RequireComponent(typeof(Rigidbody))]
    public class Plane : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MouseFlightController controller = null;

        [Header("Physics")]
        [Tooltip("Force to push plane forwards with")] public float thrust = 100f;
        [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(90f, 25f, 45f);
        [Tooltip("Multiplier for all forces")] public float forceMult = 1000f;

        [Header("Input")]
        [SerializeField] [Range(-1f, 1f)] private float pitch = 0f;
        [SerializeField] [Range(-1f, 1f)] private float yaw = 0f;
        [SerializeField] [Range(-1f, 1f)] private float roll = 0f;

        [Header("Post-Processing")]
        [SerializeField] private Volume globalVolume;

        [Header("Visuals")]
        [SerializeField] private TrailRenderer leftTrail;
        [SerializeField] private TrailRenderer rightTrail;

        [Header("Boundary Settings")]
        [SerializeField] private float returnRadius = 500f;
        [SerializeField] private Color radiusColor = Color.green;


        private FilmGrain filmGrain;
        private ChromaticAberration chromaticAberration;
        private Coroutine postEffectCoroutine;

        public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return pitch; } }
        public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return yaw; } }
        public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return roll; } }

        private Rigidbody rigid;
        private Vector3 startPos;
        private Quaternion startRot;

        private static HashSet<GameObject> collectedSpheres = new HashSet<GameObject>();

        [SerializeField] float respawnDelayTime;
        [SerializeField] float postProcessTime;

        
        private bool isGrounded;
        [SerializeField] private float groundCheckDistance = 1.5f;
        [SerializeField] private LayerMask groundLayer;


        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();

            if (controller == null)
                Debug.LogWarning(name + ": Plane - Missing reference to MouseFlightController!");
            
            startPos = transform.position;
            startRot = transform.rotation;

            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.TryGet(out filmGrain);
                globalVolume.profile.TryGet(out chromaticAberration);
            }
        }

        private void Update()
        {
            pitch = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
            yaw = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            roll = (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);

            pitch += Input.GetAxis("DPadVertical"); 
            yaw += Input.GetAxis("DPadHorizontal"); 
            roll += Input.GetAxis("RightStickX");

            pitch = Mathf.Clamp(pitch, -1f, 1f);
            yaw = Mathf.Clamp(yaw, -1f, 1f);
            roll = Mathf.Clamp(roll, -1f, 1f);
        }

        private void FixedUpdate()
        {
            CheckIfGrounded();

            if (leftTrail != null) leftTrail.enabled = !isGrounded;
            if (rightTrail != null) rightTrail.enabled = !isGrounded;

            if (isGrounded) {
                rigid.AddRelativeForce(Vector3.forward * thrust * forceMult, ForceMode.Force);
            }
            else{
                rigid.AddRelativeForce(Vector3.forward * thrust * forceMult, ForceMode.Force);
                rigid.AddRelativeTorque(new Vector3(turnTorque.x * pitch,
                                                turnTorque.y * yaw,
                                                -turnTorque.z * roll) * forceMult,
                                    ForceMode.Force);
            }

            float distanceFromStart = Vector3.Distance(transform.position, startPos);
            if (distanceFromStart > returnRadius)
            {
                Respawn();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Obstacles"))
            {
                if (postEffectCoroutine != null)
                    StopCoroutine(postEffectCoroutine);

                postEffectCoroutine = StartCoroutine(EnablePostProcessingEffects());
                StartCoroutine(RespawnAfterDelay(respawnDelayTime));
            } 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sphere"))
                CollectSphere(other.gameObject);
        }

        private IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Respawn();
        }

        private void Respawn()
        {
            transform.position = startPos;
            transform.rotation = startRot;
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

        private void CollectSphere(GameObject sphere)
        {
            if (!collectedSpheres.Contains(sphere))
            {
                collectedSpheres.Add(sphere);
                sphere.SetActive(false); 
            }
        }

        private IEnumerator EnablePostProcessingEffects()
        {
            yield return new WaitForSeconds(0.2f);
            
            if (filmGrain != null) filmGrain.active = true;
            if (chromaticAberration != null) chromaticAberration.active = true;

            yield return new WaitForSeconds(postProcessTime);

            if (filmGrain != null) filmGrain.active = false;
            if (chromaticAberration != null) chromaticAberration.active = false;

            postEffectCoroutine = null;
        }

        private void CheckIfGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, -transform.up, groundCheckDistance, groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = radiusColor;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPos : transform.position, returnRadius);
        }


    }
}
