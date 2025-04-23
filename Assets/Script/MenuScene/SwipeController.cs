using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] RectTransform contentPanel;
    [SerializeField] int totalButtons;
    [SerializeField] int buttonsPerPage = 4; 
    [SerializeField] float buttonWidth = 200f; 
    [SerializeField] float spacing = 20f; 

    [SerializeField] float tweenTime = 0.5f;
    [SerializeField] LeanTweenType tweenType;

    private int currentPage = 0;
    private int maxPages;
    private float pageWidth;

    void Start()
    {
        maxPages = Mathf.CeilToInt((float)totalButtons / buttonsPerPage);
        pageWidth = (buttonWidth + spacing) * buttonsPerPage;
    }

    public void Next()
    {
        if (currentPage < maxPages - 1)
        {
            currentPage++;
            MoveToPage(currentPage);
        }
    }

    public void Previous()
    {
        if (currentPage > 0)
        {
            currentPage--;
            MoveToPage(currentPage);
        }
    }

    void MoveToPage(int page)
    {
        float targetX = -page * pageWidth;
        LeanTween.moveLocalX(contentPanel.gameObject, targetX, tweenTime).setEase(tweenType);
    }
}
