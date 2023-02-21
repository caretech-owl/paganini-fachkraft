﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public GameObject InputFieldUsername;
    public GameObject InputFieldPassword;
    public Button LoginButton;
    //public UnityEvent OnLoginSucceed;
    //public UnityEvent OnLoginFail;
    public Toggle ToggleIsLoginRemembered;

    private void Awake()
    {
        DBConnector.Instance.Startup();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ToggleIsLoginRemembered.isOn = InternalDataModelController.GetInternalDataModelController().idm.isLoginRemembered;

        var list = AuthToken.GetAll<AuthToken>();

        AuthToken authToken = null;
        while (list.MoveNext())
        {
            authToken = list.Current;
            break;
        }

        if (authToken != null)
        {
            ContinueUserSignIn(authToken);
        } 

    }

    public void SendPinToAPI()
    {
        LoginButton.interactable = false;
        //ServerCommunication.Instance.GetSocialWorkerAuthentification(GetAuthSucceed, GetAuthFailed, InputFieldUsername.GetComponent<TMP_InputField>().text, InputFieldPassword.GetComponent<TMP_InputField>().text);

        PaganiniRestAPI.SocialWorker.Authenticate(InputFieldUsername.GetComponent<TMP_InputField>().text,
                                                  InputFieldPassword.GetComponent<TMP_InputField>().text,
                                                  GetAuthSucceed, GetAuthFailed);
    }

    public void ToggleValueChanged()
    {
        //InternalDataModelController.GetInternalDataModelController().idm.isLoginRemembered = ToggleIsLoginRemembered.isOn;
    }

    /// <summary>
    /// Request was successful
    /// </summary>
    /// <param String authtoken</param>
    private void GetAuthSucceed(AuthTokenAPI token)
    {

        var authToken = new AuthToken(token);

        if (ToggleIsLoginRemembered.isOn)
        {
            // Let's remember the user token            
            authToken.Insert();
        }

        ContinueUserSignIn(authToken);

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

    private void ContinueUserSignIn(AuthToken authToken)
    {
        // Let's keep track of the apitoken
        AppState.APIToken = authToken.ApiToken;
        SceneSwitcher.LoadUserManager();
    }
}
