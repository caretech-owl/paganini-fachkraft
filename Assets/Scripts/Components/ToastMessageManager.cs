using UnityEngine;

public class ToastMessageManager : PersistentLazySingleton<ToastMessageManager>
{
    private ToastMessage toastMessageInstance;

    [SerializeField]
    private GameObject toastPrefab; // Assign the ToastMessage prefab here in the Inspector

    // protected override void Awake()
    // {
    //     base.Awake();

    //     if (toastMessageInstance == null)
    //     {
    //         CreateToastMessageInstance();
    //     }
    // }

    public void Initialise(GameObject prefab)
    {
        toastPrefab = prefab;
    }

    private void CreateToastMessageInstance()
    {
        if (toastMessageInstance == null)
        {
            GameObject toastObject = Instantiate(toastPrefab);
            toastMessageInstance = toastObject.GetComponent<ToastMessage>();
            DontDestroyOnLoad(toastObject);

            AttachToCurrentCanvas(toastObject);
        }
    }

    private void AttachToCurrentCanvas(GameObject toastObject)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            toastObject.transform.SetParent(canvas.transform, false);
        }
        else
        {
            Debug.LogError("No Canvas found in the scene. The ToastMessage might not render correctly.");
        }
    }

    public ToastMessage Toast
    {
        get
        {
            if (toastMessageInstance == null)
            {
                CreateToastMessageInstance();
            }
            return toastMessageInstance;
        }
    }
}
