using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f;

    [SerializeField] RectTransform menuContainer;
    
    [SerializeField] Transform planePanel;
    [SerializeField] Transform modePanel;

    [SerializeField] TextMeshProUGUI planeBuySetText;

    private int[] planeCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 10 };
    private int selectedPlaneIndex;

    private Vector3 desiredMenuPos;

    void Start()
    {
        fadeGroup = FindObjectOfType<CanvasGroup>();
        fadeGroup.alpha = 1;

        //adding the button onclick events
        InitShop();

        InitMode();
    }

    void Update()
    {
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPos, 0.01f);
    }

    void NavigateTo(int menuIndex){
        switch(menuIndex){
            default:
            case 0:
                desiredMenuPos = Vector3.zero;
                break;
            case 1:
                desiredMenuPos = Vector3.right * 1280;
                break;
            case 2:
                desiredMenuPos = Vector3.left * 1280;
                break;
        }
    }

    private void SetPlane(int index){
        planeBuySetText.text = "Current";
    }

    public void OnPlayClick(){
        NavigateTo(1);
    }

    public void OnShopClick(){
        NavigateTo(2);
    }

    public void OnBackClick(){
        NavigateTo(0);
    }

    private void InitShop(){
        //assigning the refs
        if (planePanel == null)
            Debug.Log("Shop Panel not assigned!");
        
        int i = 0;
        foreach(Transform t in planePanel){
            int currentIndex = i;

            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnPlaneSelect(currentIndex));

            i++;
        }
    }

    private void InitMode(){
        if (modePanel == null)
            Debug.Log("Mode Panel not assigned!");
        
        int i = 0;
        foreach(Transform t in modePanel){
            int currentIndex = i;

            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnModeSelect(currentIndex));

            i++;
        }
    }

    private void OnPlaneSelect(int currentIndex){
        Debug.Log("Selecting Plane: " + currentIndex);

        selectedPlaneIndex = currentIndex;

        if (SaveManager.Instance.IsPlaneOwned(currentIndex)){
            planeBuySetText.text = "Select";
        } else{
            planeBuySetText.text = "Buy: " + planeCost[currentIndex].ToString();
        }
    }

    public void OnPlaneBuySet(){
        Debug.Log("Buy/Set Plane");

        if (SaveManager.Instance.IsPlaneOwned(selectedPlaneIndex)){
            SetPlane(selectedPlaneIndex);
        } else{
            if (SaveManager.Instance.BuyPlane(selectedPlaneIndex, planeCost[selectedPlaneIndex])){
                SetPlane(selectedPlaneIndex);
            } else{
                Debug.Log("Not Enough gold!");
            }
        }
    }

    public void OnModeSelect(int currentIndex){
        Debug.Log("Selecting Level: " + currentIndex);
    }
}
