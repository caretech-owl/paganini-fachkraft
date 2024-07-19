using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    public GameObject ConnectionErrorPrefab;
    public Canvas TargetCanvas;
    private AlertWifi ConnectionErrorView;


    private void Start()
    {
        // Initially, hide the connection error message.
        

        RESTAPI.Instance.OnRequestError.AddListener(OnRequestError);

        var neu = Instantiate(ConnectionErrorPrefab, TargetCanvas.transform);
        ConnectionErrorView = neu.GetComponent<AlertWifi>();

    }

    private void Update()
    {
        if (ConnectionErrorView == null)
            return;

        // Check for internet connection status.
        if (!IsInternetConnected())
        {
            // Internet connection is not available. Show the connection error message.
            ConnectionErrorView.RenderInternetError();
        }
        else
        {
            // Internet connection is available. Hide the connection error message.
            ConnectionErrorView.HideInternetConnection();
        }
    }

    /** Internet Connection Errors */


    private bool IsInternetConnected()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            // Internet connection is available.
            return true;
        }
        else
        {
            // No internet connection.
            return false;
        }
    }

    /** Session Errors */

    private void OnRequestError(ErrorData errorData)
    {
        if (errorData.HTTPStatus == 401)
        {
            ConnectionErrorView.RenderSessionExpired();
        }
        else
        {
            Debug.LogWarning(errorData.Message);
        }
        
    }


    private void OnDestroy()
    {
        if (RESTAPI.Instance != null && RESTAPI.Instance.OnRequestError != null)
        {
            RESTAPI.Instance.OnRequestError.RemoveListener(OnRequestError);
        }
        
    }


}
