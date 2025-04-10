using System.Collections;
using UnityEngine;

public class ParticleMaterialChanger : MonoBehaviour
{
    [Header("Particle Effect Settings")]
    [SerializeField] private string particleEffectsPrefabName = "Partical effects";
    [SerializeField] private Material newMaterial1;
    [SerializeField] private Material newMaterial2;
    [SerializeField] private float transitionDuration = 2f;

    [Header("Object Material Change")]
    [SerializeField] private GameObject objectToChange; 
    [SerializeField] private Material objectNewMaterial;

    [Header("Skybox Change")]
    [SerializeField] private Material newSkyboxMaterial;
    
    private bool hasTriggered = false;
    private Material originalSkyboxMaterial;

    private void Start()
    {
        if (RenderSettings.skybox != null)
        {
            originalSkyboxMaterial = new Material(RenderSettings.skybox);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.transform.root.CompareTag("Player"))
        {
            ChangeSpawnedParticleMaterials();
            ChangeObjectMaterial();
            ChangeSkyboxMaterial();
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

    void ChangeObjectMaterial()
    {
        if (objectToChange != null)
        {
            Renderer objRenderer = objectToChange.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                StartCoroutine(SmoothObjectMaterialTransition(objRenderer, objectNewMaterial, transitionDuration));
                Debug.Log("Object material changed: " + objectToChange.name);
            }
            else
            {
                Debug.LogError("Object to change does not have a Renderer component!");
            }
        }
    }

    IEnumerator SmoothObjectMaterialTransition(Renderer renderer, Material targetMaterial, float duration)
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

    void ChangeSkyboxMaterial()
    {
        if (newSkyboxMaterial != null)
        {
            RenderSettings.skybox = newSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
            Debug.Log("Skybox material changed instantly.");
        }
    }

    public void ResetSkybox()
    {
        if (originalSkyboxMaterial != null)
        {
            RenderSettings.skybox = originalSkyboxMaterial;

            if (RenderSettings.skybox.HasProperty("_Tint"))
            {
                RenderSettings.skybox.SetColor("_Tint", originalSkyboxMaterial.GetColor("_Tint"));
            }

            if (RenderSettings.skybox.HasProperty("_Exposure"))
            {
                RenderSettings.skybox.SetFloat("_Exposure", originalSkyboxMaterial.GetFloat("_Exposure"));
            }

            if (RenderSettings.skybox.HasProperty("_Rotation"))
            {
                RenderSettings.skybox.SetFloat("_Rotation", originalSkyboxMaterial.GetFloat("_Rotation"));
            }

            RenderSettings.skybox.shader = Shader.Find("Skybox/Panoramic");

            DynamicGI.UpdateEnvironment();
            Debug.Log("Skybox has been fully reset!");
        }
        else
        {
            Debug.LogWarning("Original Skybox Material is missing!");
        }
    }

    private void OnDisable()
    {
        ResetSkybox();
    }
}
