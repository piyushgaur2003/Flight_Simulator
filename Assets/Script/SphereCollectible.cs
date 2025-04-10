using System.Collections;
using UnityEngine;

public class SphereCollectible : MonoBehaviour
{
    [SerializeField] Transform ring1;
    [SerializeField] Transform ring2;
    [SerializeField] float rotSpeed = 90f;

    [SerializeField] float fadeOutTime = 1f;
    private bool isCollected = false;

    void Update()
    {
        if (ring1 != null){
            ring1.Rotate(Vector3.right * rotSpeed * Time.deltaTime);
        } 
        if (ring2 != null){
            ring2.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player")) 
        {
            isCollected = true;
            GameManager.instance.CollectSphere();
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Material[] materials = new Material[renderers.Length];
        Color[] startColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            startColors[i] = materials[i].color;
        }

        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            float t = elapsed / fadeOutTime;
            for (int i = 0; i < materials.Length; i++)
            {
                Color c = startColors[i];
                c.a = Mathf.Lerp(1f, 0f, t);
                materials[i].color = c;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
