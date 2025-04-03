using UnityEngine;

public class FlightCameraController : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 5f;
    private Vector3 fixedOffset;

    private void Start()
    {
        if (target == null)
        {
            return;
        }

        fixedOffset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + target.rotation * fixedOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothSpeed);
    }
}
