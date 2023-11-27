using UnityEngine;
using UnityEngine.UI;

public class InactiveOverlay : MonoBehaviour
{
    private GameObject overlayPanel;

    private void Start()
    {
        // Create the overlay panel
        CreateOverlayPanel();
    }

    void Update()
    {
        if (overlayPanel.active != SharedData.Instance.CurrentlyDisabled)
        {
            ShowOverlay(SharedData.Instance.CurrentlyDisabled);
        }
    }

    private void CreateOverlayPanel()
    {
        // Create a new GameObject for the overlay panel
        overlayPanel = new GameObject("OverlayPanel");
        overlayPanel.transform.SetParent(transform); // Make the overlay panel a child of this GameObject

        // Add a Canvas component to the overlay panel
        Canvas canvas = overlayPanel.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1; // Adjust the sorting order to make it appear on top

        // Add a GraphicRaycaster component to the overlay panel (enables interactions with UI elements underneath)
        overlayPanel.AddComponent<GraphicRaycaster>();

        // Add an Image component (white-colored panel) with an alpha effect
        Image image = overlayPanel.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.5f); // White color with 50% alpha

        // Set the RectTransform to cover the entire screen
        RectTransform rectTransform = overlayPanel.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        rectTransform.localScale = Vector3.one;

        Vector3 position = rectTransform.localPosition;
        position.z = -1f;
        rectTransform.localPosition = position;

        // Make the overlay panel initially inactive
        overlayPanel.SetActive(false);
    }

    // Enable the overlay effect
    public void ShowOverlay(bool active)
    {
        if (overlayPanel != null)
        {
            overlayPanel.SetActive(active);
        }
    }

    public class SharedData : PersistentLazySingleton<SharedData>
    {
        public bool CurrentlyDisabled = false;
    }

}
