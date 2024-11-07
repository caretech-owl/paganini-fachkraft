using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : PersistentLazySingleton<ToastMessage>
{
    public ToastItem ErrorToast;
    public ToastItem AlertToast;
    public ToastItem SuccessToast;
    public float toastDuration = 3f; // duration in seconds before the toast closes
    public float fadeDuration = 1f;  // duration in seconds for the fade-out

    // Start is called before the first frame update
    protected override void Awake()
    {
        gameObject.SetActive(true);
        ShowView(null); // hide all by default        
    }

    public void CloseToast()
    {
        gameObject.SetActive(false);
    }

    public void RenderErrorToast(string title, string description)
    {
        RenderToast(ErrorToast, title, description);
    }

    public void RenderAlertToast(string title, string description)
    {
        RenderToast(AlertToast, title, description);
    }

    public void RenderSuccessToast(string title, string description)
    {
        RenderToast(SuccessToast, title, description);
    }

    private void RenderToast(ToastItem toast, string title, string description)
    {
        gameObject.SetActive(true);
        
        toast.FillToast(title, description);
        ShowView(toast.gameObject);
        StopAllCoroutines(); // Stop any previous fade-out coroutine if active
        StartCoroutine(AutoCloseToast(toast.gameObject));
    }

    private void ShowView(GameObject view)
    {
        ErrorToast.gameObject.SetActive(ErrorToast.gameObject == view);
        AlertToast.gameObject.SetActive(AlertToast.gameObject == view);
        SuccessToast.gameObject.SetActive(SuccessToast.gameObject == view);
    }

    private IEnumerator AutoCloseToast(GameObject toastObject)
    {
        yield return new WaitForSeconds(toastDuration); // Wait for the specified display duration

        // Start fading out
        CanvasGroup canvasGroup = toastObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = toastObject.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        canvasGroup.alpha = 1f;
    }
}
