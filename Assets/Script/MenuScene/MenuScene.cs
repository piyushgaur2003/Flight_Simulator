using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f;

    [SerializeField] RectTransform menuContainer;
    
    [SerializeField] Transform planePanel;
    [SerializeField] Transform modePanel;

    [SerializeField] TextMeshProUGUI planeBuySetText;
    [SerializeField] TextMeshProUGUI goldText;

    private MenuCamera menuCam;

    [SerializeField] GameObject levelPanel;
    [SerializeField] GameObject backButton;

    private int[] planeCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 10 };
    private int selectedPlaneIndex;
    private int activePlaneIndex;

    private Vector3 desiredMenuPos;

    void Start()
    {
        menuCam = FindObjectOfType<MenuCamera>();
        UpdateGoldText();

        fadeGroup = FindObjectOfType<CanvasGroup>();
        fadeGroup.alpha = 1;

        InitShop();

        InitMode();

        OnPlaneSelect(SaveManager.Instance.state.activePlane);
        SetPlane(SaveManager.Instance.state.activePlane);

        planePanel.GetChild(SaveManager.Instance.state.activePlane).GetComponent<RectTransform>().localScale = Vector3.one * 1.1f;
    }

    void Update()
    {
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPos, 0.03f);
    }

    void NavigateTo(int menuIndex){
        switch(menuIndex){
            default:
            case 0:
                desiredMenuPos = Vector3.zero;
                menuCam.BackToMainMenu();
                break;
            case 1:
                desiredMenuPos = Vector3.right * 1920;
                menuCam.MoveToLevel();
                break;
            case 2:
                desiredMenuPos = Vector3.left * 1920;
                menuCam.MoveToShop();
                break;
        }
    }

    private void SetPlane(int index){
        activePlaneIndex = index;
        SaveManager.Instance.state.activePlane = index;
        
        planeBuySetText.text = "Current";

        SaveManager.Instance.Save();
    }

    private void UpdateGoldText(){
        goldText.text = SaveManager.Instance.state.gold.ToString();
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

            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsPlaneOwned(i) ? Color.white : new Color(0.7f, 0.7f, 0.7f);

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

        if (selectedPlaneIndex == currentIndex)
            return;
        
        planePanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.1f;
        planePanel.GetChild(selectedPlaneIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        selectedPlaneIndex = currentIndex;

        if (SaveManager.Instance.IsPlaneOwned(currentIndex)){
            if (activePlaneIndex == currentIndex){
                planeBuySetText.text = "Current";
            } else{
                planeBuySetText.text = "Select";
            }
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

                planePanel.GetChild(selectedPlaneIndex).GetComponent<Image>().color = Color.white;

                UpdateGoldText();
            } else{
                Debug.Log("Not Enough gold!");
            }
        }
    }

    public void OnModeSelect(int currentIndex){
        Debug.Log("Selecting Level: " + currentIndex);
    }

    public void ShowPanel()
    {   
        backButton.SetActive(false);
        modePanel.gameObject.SetActive(false);

        levelPanel.SetActive(true);
        levelPanel.transform.localScale = Vector3.zero;
        levelPanel.GetComponent<CanvasGroup>().alpha = 0;

        levelPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        levelPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }

    public void HidePanel()
    {
        levelPanel.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
        levelPanel.GetComponent<CanvasGroup>().DOFade(0, 0.4f)
            .OnComplete(() =>
            {
                levelPanel.SetActive(false);
                
                modePanel.gameObject.SetActive(true);
                backButton.SetActive(true);

                modePanel.localScale = Vector3.zero;
                backButton.transform.localScale = Vector3.zero;

                modePanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                backButton.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            });
    }
}
