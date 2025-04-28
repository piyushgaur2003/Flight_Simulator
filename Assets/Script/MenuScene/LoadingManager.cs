using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("References")]
    public GameObject loadingScreenPanel;

    [Header("Settings")]
    public float loadingDelay = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(true);

        yield return new WaitForSeconds(loadingDelay); 

        SceneManager.LoadScene(sceneName);

        yield return null; 

        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(false);
    }
}
