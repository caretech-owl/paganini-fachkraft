﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button ButtonOpen;
    public Button ButtonClose;
    public GameObject Menu;

    // SW Profile information
    public RawImage SWPhoto;
    public TMPro.TMP_Text SWName;

    public static class MenuOptions{
        public const string LOGOUT = "OptionLogout";
        public const string SWITCH_USER = "OptionSwitchUser";
        public const string SW_PROFILE = "OptionSWProfile";
    }


    // Start is called before the first frame update
    void Start()
    {
        ButtonOpen.onClick.AddListener(OpenMenu);
        ButtonClose.onClick.AddListener(CloseMenu);

        CloseMenu();

        RenderPicture(AppState.CurrenSocialWorker.ProfilePic);
        SWName.text = AppState.CurrenSocialWorker.Firstname + " " + AppState.CurrenSocialWorker.Surname;

        SetupMenuOptions();

    }

    // public events
    public void Logout()
    {
        AuthToken.DeleteAll();
        AppState.ResetValues();
        SceneSwitcher.LoadLogin();
    }    

    public void SwitchUser(){

    }

    public void OpenSWProfile(){
        //SceneSwitcher.LoadUserProfile();
    }

    // private events
    private void OpenMenu()
    {
        Menu.SetActive(true);  
        RenderCanOpen(false);      
    }

    private void CloseMenu()
    {
        Menu.SetActive(false);
        RenderCanOpen(true);        
    }


  private void RenderPicture(byte[] imageBytes)
    {
        if (SWPhoto.texture != null)
        {
            DestroyImmediate(SWPhoto.texture, true);
        }

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Set the aspect ratio of the image
        SWPhoto.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

        SWPhoto.texture = texture;
        SWPhoto.gameObject.SetActive(true);

    }    

    public void SetCurrentOption(string option)
    {
        AppState.CurrentMenuOption = option;

        foreach (MainMenuOption menuOption in Menu.GetComponentsInChildren<MainMenuOption>())
        {
            if (menuOption.name == option){
                menuOption.EnableOption(false);
            }
            
        }
    }

    // Attach OptionSelected to all the MainMenuOption buttons in the menu
    private void SetupMenuOptions()
    {
        foreach (MainMenuOption option in Menu.GetComponentsInChildren<MainMenuOption>())
        {
            option.OnOptionSelected.AddListener(OptionSelected);

            if (option.name == AppState.CurrentMenuOption)
            {
                option.EnableOption(false);
            }            
        }
    }

    // Save the name of the option selected
    private void OptionSelected(string option)
    {
        AppState.CurrentMenuOption = option;
    }


    private void OnDestroy()
    {
        ButtonOpen.onClick.RemoveAllListeners();
        ButtonClose.onClick.RemoveAllListeners();

        if (SWPhoto.texture != null)
        {
            DestroyImmediate(SWPhoto.texture, true);
        }

        foreach (MainMenuOption option in Menu.GetComponentsInChildren<MainMenuOption>())
        {
            option.OnOptionSelected.RemoveAllListeners();
        }
    }

    private void RenderCanOpen(bool canOpen){
        ButtonOpen.gameObject.SetActive(canOpen);
        ButtonClose.gameObject.SetActive(!canOpen);

        if (SWPhoto.texture != null)
        {
            DestroyImmediate(SWPhoto.texture, true);
        }        
    }

}
