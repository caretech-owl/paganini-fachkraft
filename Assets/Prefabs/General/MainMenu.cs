using System;
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

    // Start is called before the first frame update
    void Start()
    {
        ButtonOpen.onClick.AddListener(OpenMenu);
        ButtonClose.onClick.AddListener(CloseMenu);

        CloseMenu();

        RenderPicture(AppState.CurrenSocialWorker.ProfilePic);
        SWName.text = AppState.CurrenSocialWorker.Firstname + " " + AppState.CurrenSocialWorker.Surname;


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

    private void OnDestroy()
    {
        ButtonOpen.onClick.RemoveAllListeners();
        ButtonClose.onClick.RemoveAllListeners();
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
