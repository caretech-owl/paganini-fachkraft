using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PaganiniRestAPI;

public class PhotoSlideShow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject NoData;
    public RectTransform slideContainer;
    public PhotoFeedbackToggle PhotoFeedback;
    public float dragThreshold = 30f;

    public RouteSharedData.EditorMode EditMode { get; set; }

    private int currentSlideIndex;
    public bool interactable = true;
    private Vector2 startPosition;
    private bool isDragging;
    private List<PathpointPhoto> pathpointPhotos;
    private List<PathpointPhoto> renderedPhotos;
    private List<Image> slides;

    private void Start()
    {
        //currentSlideIndex = 0;
        //isDragging = false;
        //UpdateVisualIndicators();
        //PhotoFeedback.gameObject.SetActive(false);
    }

    public void LoadSlideShow(List<PathpointPhoto> photos)
    {
        ResetSlides();
        pathpointPhotos = photos;

        if (photos.Count > 0)
        {
            NoData?.gameObject.SetActive(false);

            GenerateSlides();
            interactable = true;
            RenderFeebackPanel();
        }

        if (renderedPhotos.Count == 0)
        {
            NoData?.gameObject.SetActive(true);
            interactable = false;
            HideFeedbackPanel();
        }
    }

    /* Data processing */

    public void OnPhotoFeedbackValueChanged(PathpointPhoto.PhotoFeedback feedback) {
        var photo = renderedPhotos[currentSlideIndex];
        photo.DiscussionFeedback = feedback;
        photo.InsertDirty();        
    }


    /* UI methods and events */

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable) return;

        startPosition = eventData.position;
        isDragging = true;
        HideFeedbackPanel();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable) return;

        if (isDragging)
        {
            float deltaX = eventData.position.x - startPosition.x;
            slideContainer.anchoredPosition = new Vector2(-currentSlideIndex * Screen.width + deltaX, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!interactable) return;

        isDragging = false;
        float deltaX = eventData.position.x - startPosition.x;

        if (Mathf.Abs(deltaX) > dragThreshold)
        {
            if (deltaX < 0 && currentSlideIndex < slides.Count - 1)
            {
                currentSlideIndex++;
            }
            else if (deltaX > 0 && currentSlideIndex > 0)
            {
                currentSlideIndex--;
            }
        }

        StartCoroutine(SmoothSlideTransition());
    }

    private IEnumerator SmoothSlideTransition()
    {
        //float targetX = -currentSlideIndex * Screen.width;
        float targetX = -currentSlideIndex * slideContainer.rect.width;
        float duration = 0.2f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.Lerp(slideContainer.anchoredPosition.x, targetX, elapsedTime / duration);
            slideContainer.anchoredPosition = new Vector2(newX, 0);
            yield return null;
        }

        RenderFeebackPanel();
    }

    private void GenerateSlides()
    {
        slides = new List<Image>();
        renderedPhotos = new List<PathpointPhoto>();

        for (int i = 0; i < pathpointPhotos.Count; i++)
        {
            if (pathpointPhotos[i].CleaningFeedback != PathpointPhoto.PhotoFeedback.Delete) { 
                GameObject newSlide = new GameObject("Slide" + i);
                newSlide.transform.SetParent(slideContainer, false);

                Image slideImage = newSlide.AddComponent<Image>();
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(pathpointPhotos[i].Photo);
                slideImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                // Set Preserve Aspect to true
                slideImage.preserveAspect = true;

                // Set anchors and pivot point
                slideImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                slideImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                slideImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                slideImage.rectTransform.sizeDelta = new Vector2(slideContainer.rect.width, slideContainer.rect.height);
                slideImage.rectTransform.anchoredPosition = new Vector2(0, 0);

                slides.Add(slideImage);
                renderedPhotos.Add(pathpointPhotos[i]);
            }
        }
    }

    public void ResetSlides()
    {
        if (slides != null)
        {
            for (int i = 0; i < slides.Count; i++)
            {
                Destroy(slides[i].gameObject);
            }
            slides.Clear();
        }

        currentSlideIndex = 0;
        isDragging = false;
    }

    public void RenderFeebackPanel() {
        
        // We display the FeedbackPanel only in Discussion mode
        if (EditMode == RouteSharedData.EditorMode.Discussion) {
            PhotoFeedback.gameObject.SetActive(true);
            // display value        
            PhotoFeedback.SetFeedbackValue(renderedPhotos[currentSlideIndex].DiscussionFeedback);
        } else {
            PhotoFeedback.gameObject.SetActive(false);
        }
    }

    public void HideFeedbackPanel() {
        PhotoFeedback.gameObject.SetActive(false);
    }


}
