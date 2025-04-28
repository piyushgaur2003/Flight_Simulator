using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    private LocationData currentSelectedLocation;

    public void OnItemSelected(LocationData data)
    {
        currentSelectedLocation = data;
    }

    public void OnSelectButtonClicked()
    {
        if (currentSelectedLocation != null)
        {
            SceneManager.LoadScene(currentSelectedLocation.sceneName);
        }
        else
        {
            Debug.LogWarning("No level selected!");
        }
    }
}
