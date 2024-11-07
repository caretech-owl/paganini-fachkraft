
using UnityEngine;

public class AppSettings : MonoBehaviour
{
    public GameObject ToastPrefab;
    void Start ()
    {
        ToastMessageManager.Instance.Initialise(ToastPrefab);
    }
}
