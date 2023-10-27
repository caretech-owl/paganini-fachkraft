using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoSlideShow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PhotoSlideStatus SlideStatus;

    [Header("UI Elements")]
    public GameObject NoData;
    public RectTransform slideContainer;
    public PhotoFeedbackToggle PhotoFeedback;
    public float dragThreshold = 30f;

    public RouteSharedData.EditorMode EditMode { get; set; }

    private int currentSlideIndex;
    public bool interactable = true;
    private Vector2 startPosition;
    private Vector2 originalPosition;
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

        originalPosition = slideContainer.anchoredPosition;
    }

    public void LoadSlideShow(List<PathpointPhoto> photos, int startIndex)
    {
        ResetSlides();
        pathpointPhotos = photos;

        if (photos.Count > 0)
        {
            NoData?.gameObject.SetActive(false);

            
            slideContainer.anchoredPosition = originalPosition;
            // generate the slides, and normalise the starting index to the list of actually rendered ones
            // which skip the 'unselected' pictures.
            currentSlideIndex = GenerateSlides(startIndex);            

            // set the starting photo           
            float targetX = -currentSlideIndex * slideContainer.rect.width;
            slideContainer.anchoredPosition = new Vector2(targetX, 0);

            // initialise the status
            SlideStatus.GenerateDots(renderedPhotos.Count);
            SlideStatus.SetActiveSlide(currentSlideIndex);


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
            slideContainer.anchoredPosition = new Vector2(-currentSlideIndex * slideContainer.rect.width + deltaX, 0);
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
        SlideStatus.SetActiveSlide(currentSlideIndex);
    }

    private int GenerateSlides(int startIndex)
    {
        slides = new List<Image>();
        renderedPhotos = new List<PathpointPhoto>();

        var renderListIndex = -1;
        for (int i = 0; i < pathpointPhotos.Count; i++)
        {
            if (pathpointPhotos[i].CleaningFeedback != PathpointPhoto.PhotoFeedback.Delete) { 
                GameObject newSlide = new GameObject("Slide" + i);
                newSlide.transform.SetParent(slideContainer, false);

                Image slideImage = newSlide.AddComponent<Image>();
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(pathpointPhotos[i].Data.Photo);
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
                renderListIndex++;
            }

            if (startIndex == i)
            {
                startIndex = renderListIndex;
            }
        }

        Debug.Log($"Start Index: {startIndex}");

        return startIndex;
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

        renderedPhotos?.Clear();

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


    public void CleanupView()
    {
        // Stop any running coroutines
        StopAllCoroutines();

        // Remove any event listeners or cleanup any other resources as needed.
        // For example, if you have registered event listeners, unregister them here.

        // Clear references to objects
        //slideContainer = null; // Clear reference to slideContainer
        //PhotoFeedback = null; // Clear reference to PhotoFeedback

        // Destroy all slide images and their textures
        if (slides != null)
        {
            foreach (var slide in slides)
            {
                if (slide != null)
                {
                    var slideTexture = slide.sprite?.texture;
                    Destroy(slide.gameObject);
                    Destroy(slideTexture); // Destroy the texture to release memory
                }
            }
            slides.Clear();
        }

        // Clear lists and variables
        //pathpointPhotos?.Clear();
        renderedPhotos?.Clear();
        currentSlideIndex = 0;
        isDragging = false;

        // Deactivate or hide any game objects or UI elements that are no longer needed
        NoData?.gameObject.SetActive(false);

        // Clear any references to coroutines
        // Remove any remaining references or states that may cause memory leaks

        // Clear any other variables or states as needed

        // Set interactable to false
        interactable = false;
    }

}
