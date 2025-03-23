using System.Collections;
using UnityEngine;

public class ParticleMaterialChanger : MonoBehaviour
{
    [SerializeField] private string particleEffectsPrefabName = "Partical effects";
    [SerializeField] private Material newMaterial1;
    [SerializeField] private Material newMaterial2;
    [SerializeField] private float transitionDuration = 2f;
    
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.transform.root.CompareTag("Player"))
        {
            ChangeSpawnedParticleMaterials();
            hasTriggered = true;
        }
    }

    void ChangeSpawnedParticleMaterials()
    {
        GameObject[] spawnedParticles = GameObject.FindGameObjectsWithTag("ParticleEffects");

        if (spawnedParticles.Length == 0)
        {
            Debug.LogError("No spawned ParticleEffects found in the scene!");
            return;
        }

        foreach (GameObject particleEffect in spawnedParticles)
        {
            ParticleSystemRenderer[] renderers = particleEffect.GetComponentsInChildren<ParticleSystemRenderer>();

            if (renderers.Length < 2)
            {
                Debug.LogWarning("Skipping " + particleEffect.name + " - Not enough ParticleSystemRenderers.");
                continue; 
            }

            StartCoroutine(SmoothMaterialTransition(renderers[0], newMaterial1, transitionDuration));
            StartCoroutine(SmoothMaterialTransition(renderers[1], newMaterial2, transitionDuration));

            Debug.Log("Particle materials changed for: " + particleEffect.name);
        }
    }

    IEnumerator SmoothMaterialTransition(ParticleSystemRenderer renderer, Material targetMaterial, float duration)
    {
        Material startMaterial = new Material(renderer.material);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            renderer.material.Lerp(startMaterial, targetMaterial, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        renderer.material = targetMaterial;
    }
}
