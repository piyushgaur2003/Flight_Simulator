using Carousel;
using UnityEngine;


[System.Serializable]
public class LocationData{
    public Sprite sprite;
    public string sceneName;
}

public class LocationCarousel : CarouselController<LocationData>
{
    private LocationData currentSelectedLocation;

    private void OnEnable()
    {
        OnItemSelected.AddListener(HandleItemSelected);
        OnCurrentItemUpdated.AddListener(LogItem);
    }

    private void OnDisable()
    {
        OnItemSelected.RemoveListener(HandleItemSelected);
        OnCurrentItemUpdated.RemoveListener(LogItem);
    }

    private void HandleItemSelected(LocationData data)
    {
        currentSelectedLocation = data;
        Debug.Log($"Selected Location: {currentSelectedLocation.sprite.name}");
    }

    private void LogItem(LocationData data){
       Debug.Log($"Current Item Updated: {data.sprite.name}");
    }

    public LocationData GetCurrentSelectedLocation()
    {
        return currentSelectedLocation;
    }
}
