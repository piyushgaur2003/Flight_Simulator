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

    [SerializeField] private AnimationCurve c1;
    [SerializeField] private AnimationCurve rudderAoa;
    [SerializeField] private AnimationCurve sideDrag;
    [SerializeField] private AnimationCurve frontDrag;

    public float Aoa;
    public float AoaYaw;
    public float enginePower;

    public Vector3 velocity;
    public Vector3 localVelocity;
    public Vector3 localAngularVelocity;

    void Start()
    {
        rb.centerOfMass = CM.localPosition;
    }

    void FixedUpdate() {
        CalculateState();

        c1 = AnimationCurve.Linear(-10, 0, 10, 1);
        rb.AddForceAtPosition(-enginePoint.forward * enginePower, enginePoint.position);
        float lift = c1.Evaluate(Aoa) * ((airPressure * (localVelocity.z * localVelocity.z)) / 2) * wingArea;
        lift = Mathf.Max(lift, 0.1f);
        rb.AddForceAtPosition(liftPoint.up * lift, liftPoint.position);

        float _frontDrag = frontDrag.Evaluate(localVelocity.z) * ((airPressure * (localVelocity.z * localVelocity.z)) /2) * frontAera;
        rb.AddForce(transform.forward * -_frontDrag);

        float _sideDrag = sideDrag.Evaluate(localVelocity.x) * ((airPressure * (localVelocity.x * localVelocity.x)) /2) * sideAera;
        rb.AddForce(transform.forward * -_sideDrag);

        rb.AddTorque(transform.right * lift * 0.01f);
        rb.AddTorque(transform.up * rudderAoa.Evaluate(AoaYaw) * (0.5f * airPressure * (localVelocity.z * localVelocity.z)) * verticalStabilizerArea);

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
