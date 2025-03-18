using Unity.Mathematics;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    [SerializeField] private Transform CM;
    [SerializeField] private Transform liftPoint;
    [SerializeField] private Transform enginePoint;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float airPressure = 14.7f;
    [SerializeField] private float wingArea = 10f;
    [SerializeField] private float verticalStabilizerArea = 1f;
    [SerializeField] private float frontAera = 1.5f;
    [SerializeField] private float sideAera = 10f;

    [SerializeField] private float aileronAera;
    [SerializeField] private float elevatorAera;
    [SerializeField] private float rudderAera;

    [SerializeField] private float ac = 1;
    [SerializeField] private float ec = 1;
    [SerializeField] private float rc = 1;


    [SerializeField] private AnimationCurve c1;
    [SerializeField] private AnimationCurve rudderAoa;
    [SerializeField] private AnimationCurve sideDrag;
    [SerializeField] private AnimationCurve frontDrag;

    public float Aoa;
    public float AoaYaw;
    public float maxAoa = 4f;
    public float maxAoaYaw = 2f;
    public float enginePower = 4f;

    public Vector3 velocity;
    public Vector3 localVelocity;
    public Vector3 localAngularVelocity;

    [Header("Inputs")]
    [SerializeField] private float inputPitch;
    [SerializeField] private float inputRoll;
    [SerializeField] private float inputYaw;

    void Start()
    {
        rb.centerOfMass = CM.localPosition;
    }

    void FixedUpdate() {
        inputPitch = Input.GetAxis("Vertical");
        inputRoll = Input.GetAxis("Horizontal");
        inputYaw = Input.GetAxis("Yaw");

        CalculateState();

        c1 = AnimationCurve.Linear(-10, 0, 10, 1);
        rb.AddForceAtPosition(-enginePoint.forward * enginePower, enginePoint.position);
        float lift = c1.Evaluate(Aoa) * ((airPressure * (localVelocity.z * localVelocity.z)) / 2) * wingArea;
        lift = Mathf.Max(lift, 0.1f);
        rb.AddForceAtPosition(liftPoint.up * lift, liftPoint.position);

        float _frontDrag = frontDrag.Evaluate(localVelocity.z) * ((airPressure * (localVelocity.z * localVelocity.z)) /2) * frontAera;
        rb.AddForce(transform.forward * -_frontDrag);

        float _sideDrag = sideDrag.Evaluate(localVelocity.x) * ((airPressure * (localVelocity.x * localVelocity.x)) /2) * sideAera;
        rb.AddForce(transform.right * -_sideDrag);

        float _aileron = ac * ((airPressure) * (localVelocity.z * localVelocity.z) / 2) * aileronAera;
        float _elevator = ec * ((airPressure) * (localVelocity.z * localVelocity.z) / 2) * elevatorAera;
        float _rudder = rc * ((airPressure) * (localVelocity.z * localVelocity.z) / 2) * rudderAera;

        rb.AddTorque(transform.right * lift * 0.01f);
        rb.AddTorque(transform.up * rudderAoa.Evaluate(AoaYaw) * (0.5f * airPressure * (localVelocity.z * localVelocity.z)) * verticalStabilizerArea);
        if (Mathf.Sqrt(AoaYaw * AoaYaw) < maxAoaYaw)
            rb.AddTorque(transform.up * _rudder * inputYaw);
        rb.AddTorque(transform.forward * _aileron * inputRoll);
        if (Mathf.Sqrt(Aoa * Aoa) < maxAoa) 
            rb.AddTorque(transform.right * _elevator * inputPitch);

        Debug.Log($"Lift Force: {lift}");
        Debug.Log($"Velocity: {velocity}");
        Debug.Log($"Angle of Attack: {Aoa}");
    }

    void CalculateState(){
        var invRot = Quaternion.Inverse(rb.rotation);
        velocity = rb.velocity;
        localVelocity = invRot * velocity;
        localAngularVelocity = invRot * rb.angularVelocity;

        CalculateAngleOfAttack();
    }

    void CalculateAngleOfAttack(){
        Aoa = Mathf.Atan2(-localVelocity.y, localVelocity.z) * 57.2957795f;
        AoaYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * 57.2957795f;
    }
}
