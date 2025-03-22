using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public int totalSpheres = 10; 
    private int collectedSpheres = 0;
    
    public TextMeshProUGUI collectionText;
    public CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        panelCanvasGroup.alpha = 0; 
        UpdateUI();
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
}
