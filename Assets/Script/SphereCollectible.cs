using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SphereCollectible : MonoBehaviour
{
    [SerializeField] Transform ring1;
    [SerializeField] Transform ring2;
    [SerializeField] float rotSpeed = 90f;

    [SerializeField] float fadeOutTime = 1f;
    [SerializeField] float bounceForce = 5f;
    private bool isCollected = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ring1 != null){
            ring1.Rotate(Vector3.right * rotSpeed * Time.deltaTime);
        } 
        if (ring2 != null){
            ring2.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Collider>().enabled = false;

        if (isCollected) return;


        if (collision.gameObject.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
        {
            isCollected = true;

            GetComponent<Collider>().enabled = false;

            rb.isKinematic = false;
            rb.AddForce((transform.up + transform.forward) * bounceForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            GameManager.instance.CollectSphere();
        }
    }


    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < fadeOutTime)
        {
            float t = elapsed / fadeOutTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
