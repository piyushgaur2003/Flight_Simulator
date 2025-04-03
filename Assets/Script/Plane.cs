using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

        public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return pitch; } }
        public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return yaw; } }
        public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return roll; } }

        private Rigidbody rigid;
        private Vector3 startPos;
        private Quaternion startRot;

        private static HashSet<GameObject> collectedSpheres = new HashSet<GameObject>();

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();

            if (controller == null)
                Debug.LogWarning(name + ": Plane - Missing reference to MouseFlightController!");
            
            startPos = transform.position;
            startRot = transform.rotation;
        }

        private void Update()
        {
            pitch = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
            yaw = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            roll = (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
        }

        private void FixedUpdate()
        {
            rigid.AddRelativeForce(Vector3.forward * thrust * forceMult, ForceMode.Force);
            rigid.AddRelativeTorque(new Vector3(turnTorque.x * pitch,
                                                turnTorque.y * yaw,
                                                -turnTorque.z * roll) * forceMult,
                                    ForceMode.Force);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Obstacles"))
                StartCoroutine(RespawnAfterDelay(1f)); 
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
    }
}
