using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UserPicItem : MonoBehaviour
{
    public Button UserButton;
    public TMPro.TMP_Text MnemonicToken;

    public RawImage UserPhoto;

    public UserItemEvent OnSelected;

    private User UserItem;

    // Start is called before the first frame update
    void Start()
    {
        UserButton.onClick.AddListener(itemSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillUser(User user)
    {
        MnemonicToken.text = user.Mnemonic_token;

        if (user.ProfilePic != null)
        {
            RenderPicture(user.ProfilePic);
        }
        else {
            // Set a default image
        }
        
        UserItem = user;
    }

    private void RenderPicture(byte[] imageBytes)
    {
        if (UserPhoto.texture != null)
        {
            DestroyImmediate(UserPhoto.texture, true);
        }

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Set the aspect ratio of the image
        UserPhoto.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

        UserPhoto.texture = texture;
        UserPhoto.gameObject.SetActive(true);

    }    

    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(UserItem);
        }
    }

    private void OnDestroy() {
        UserButton.onClick.RemoveListener(itemSelected);

        if (UserPhoto.texture != null)
        {
            DestroyImmediate(UserPhoto.texture, true);
        }        
    }

}
