using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;

    private Vector3 desiredPos;
    private Quaternion desiredRot;

    public Transform showWayPoint;
    public Transform levelWayPoint;

    void Start()
    {
        startPos = desiredPos = transform.localPosition;
        startRot = desiredRot = transform.rotation;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, 0.02f);
        transform.localRotation = Quaternion.Lerp(transform.rotation, desiredRot, 0.02f);
    }

    public void BackToMainMenu(){
        desiredPos = startPos;
        desiredRot = startRot;
    }

    public void MoveToShop(){
        desiredPos = showWayPoint.localPosition;
        desiredRot = showWayPoint.localRotation;
    }

     public void MoveToLevel(){
        desiredPos = levelWayPoint.localPosition;
        desiredRot = levelWayPoint.localRotation;
     }
}
