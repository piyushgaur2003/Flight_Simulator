using UnityEngine;

public class FlightCameraController : MonoBehaviour
{
    public Transform target;  // Assign your aircraft here
    public float smoothSpeed = 5f;  // Follow speed
    private Vector3 fixedOffset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target (aircraft) not assigned to the FlightCameraController!");
            return;
        }

        // Store the initial offset in local space (relative to the aircraft)
        fixedOffset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Maintain the original offset relative to the aircraft's rotation
        Vector3 targetPos = target.position + target.rotation * fixedOffset;

        // Smoothly move the camera to maintain offset
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Make the camera rotate with the aircraft
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothSpeed);
    }
}
