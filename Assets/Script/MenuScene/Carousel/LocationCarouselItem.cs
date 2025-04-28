using Carousel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LocationCarouselItem : CarouselItem<LocationData>
{
    [SerializeField] Image _image;

    protected override void OnDataUpdated(LocationData data){

        base.OnDataUpdated(data);
        _image.sprite = data.sprite;

    }

    protected override void OnActivated()
    {
        base.OnActivated();

        this.CreateSequence()
            .Join(_image.DOFade(1, 0.25f)).Join(_rectTransform.DOScale(1, 0.25f));
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        this.CreateSequence()
            .Join(_image.DOFade(0.25f, 0.25f)).Join(_rectTransform.DOScale(0.75f, 0.25f));
    }
}
