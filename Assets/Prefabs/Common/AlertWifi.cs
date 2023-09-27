using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertWifi : MonoBehaviour
{
    public GameObject InternetErrorView;
    public GameObject SessionErrorView;

    public GameObject CurrentView;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        ShowView(null); // hide all by default        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderInternetError()
    {
        ShowView(InternetErrorView);
        CurrentView = InternetErrorView;
        gameObject.SetActive(true);
    }

    public void RenderSessionExpired()
    {
        ShowView(SessionErrorView);
        CurrentView = SessionErrorView;
        gameObject.SetActive(true);
    }

    public void HideInternetConnection()
    {
        InternetErrorView.SetActive(false);
        
        if (CurrentView == InternetErrorView)
        {
            gameObject.SetActive(false);
            CurrentView = null;
        }
        
    }

    public void HideSessionExpired()
    {
        SessionErrorView.SetActive(false);

        if (CurrentView == SessionErrorView)
        {
            gameObject.SetActive(false);
            CurrentView = null;
        }

    }

    private void ShowView(GameObject view)
    {
        InternetErrorView.SetActive(view == InternetErrorView);
        SessionErrorView.SetActive(view == SessionErrorView);
    }

    public void BackToLogin()
    {
        SceneSwitcher.LoadLogin();
    }

}
