using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    public GameObject InputFieldUsername;
    public GameObject InputFieldPassword;
    public Button LoginButton;
    //public UnityEvent OnLoginSucceed;
    //public UnityEvent OnLoginFail;
    public Toggle ToggleIsLoginRemembered;

    // Start is called before the first frame update
    void Start()
    {
        ToggleIsLoginRemembered.isOn = InternalDataModelController.GetInternalDataModelController().idm.isLoginRemembered;
    }

    public void SendPinToAPI()
    {
        LoginButton.interactable = false;
        ServerCommunication.Instance.GetSocialWorkerAuthentification(GetAuthSucceed, GetAuthFailed, InputFieldUsername.GetComponent<TMP_InputField>().text, InputFieldPassword.GetComponent<TMP_InputField>().text);
    }

    public void ToggleValueChanged()
    {
        InternalDataModelController.GetInternalDataModelController().idm.isLoginRemembered = ToggleIsLoginRemembered.isOn;
    }

    /// <summary>
    /// Request was successful
    /// </summary>
    /// <param String authtoken</param>
    private void GetAuthSucceed(APIToken token)
    {
        SceneManager.LoadScene("Overview");
        //ServerCommunication.Instance.GetUserProfile(GetUserProfileSucceed, GetUserProfileFailed, token.apitoken);
    }

    /// <summary>
    /// There were some problems with request.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    private void GetAuthFailed(string errorMessage)
    {
        Debug.LogError(errorMessage);
        LoginButton.interactable = true;

        Assets.ErrorHandlerSingleton.GetErrorHandler().AddNewError("AuthFailed", errorMessage);

        //OnLoginFail.Invoke();
    }
}
