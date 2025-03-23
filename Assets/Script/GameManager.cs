using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private int totalSpheres = 10; 
    private int collectedSpheres = 0;
    
    [SerializeField] private TextMeshProUGUI collectionText;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [SerializeField] private GameObject particleGroupPrefab; 
    [SerializeField] private Transform player;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private int maxParticles = 5;
    private GameObject[] activeParticles;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        panelCanvasGroup.alpha = 0;
        activeParticles = new GameObject[maxParticles];

        for (int i = 0; i < maxParticles; i++)
        {
            Vector3 spawnPos = GetSpawnPosition();
            activeParticles[i] = Instantiate(particleGroupPrefab, spawnPos, Quaternion.identity);
        }
        UpdateUI();
    }

    void Update()
    {
        for (int i = 0; i < maxParticles; i++)
        {
            if (activeParticles[i] != null)
            {
                float distance = Vector3.Distance(player.position, activeParticles[i].transform.position);
                if (distance > spawnRadius * 1.5f)
                {
                    activeParticles[i].transform.position = GetSpawnPosition();
                }
            }
        }
    }

    public void CollectSphere()
    {
        collectedSpheres++;
        UpdateUI();
        
        if (collectedSpheres == 1) 
        {
            StartCoroutine(FadeInPanel());
        }
    }

    private void UpdateUI()
    {
        collectionText.text = $"{collectedSpheres}/{totalSpheres}";
    }

    IEnumerator FadeInPanel()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            panelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panelCanvasGroup.alpha = 1;
    }

    public void PlayerRespawned()
    {
        UpdateUI(); 
        if (collectedSpheres > 0)
        {
            panelCanvasGroup.alpha = 1; 
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPos = player.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = player.position.y; 
        return spawnPos;
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}
