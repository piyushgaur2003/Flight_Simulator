using UnityEngine;

public class SphereCollectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player")) 
        {
            GameManager.instance.CollectSphere();
            Destroy(gameObject);
        }
    }
}
