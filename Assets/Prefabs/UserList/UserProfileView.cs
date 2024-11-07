using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserProfileView : MonoBehaviour
{
    [Header("Main views")]    
    public GameObject UserSettingsView;

    [Header("Profile picture")]
    public UserPicEdit UserPhoto;

    [Header("Meta data")]
    public TMPro.TMP_Text UserName;
    public TMPro.TMP_Text ErrorText;
    public TMPro.TMP_InputField UserNameInput;
    public TMPro.TMP_InputField ContactTelInput;
    public ButtonPrefab UpdateButton;

    private User UpdatedUser;
    private bool IsNewUser = false;
    
    void Awake()
    {
        UserPhoto.OnProfilePicChanged.AddListener(PhotoChangedHandler);
    }

    // Start is called before the first frame update
    void Start()
    {
        ErrorText.gameObject.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Display the user data
    /// </summary>
    public void DisplayUserData(){
        IsNewUser = false;
        UserName.text = AppState.CurrentUser.Mnemonic_token;
        UserNameInput.text = AppState.CurrentUser.Mnemonic_token;
        ContactTelInput.text = AppState.CurrentUser.Contact;

        UserSettingsView.SetActive(true);

        UpdatedUser = User.Get(AppState.CurrentUser.Id);

        UserPhoto.RenderProfilePic(AppState.CurrentUser.ProfilePic);
    }

    /// <summary>
    /// Display the new user form
    /// </summary>
    public void DisplayNewUserForm(){
        IsNewUser = true;
        UserName.text = "";
        UserNameInput.text = "";
        ContactTelInput.text = "";

        UserSettingsView.SetActive(false);
        UpdatedUser = new User();
        UpdatedUser.FromAPI = false;

        UserPhoto.RenderProfilePic(null);
    }

    // public events

    /// <summary>
    /// Save the profile
    /// </summary>
    public void SaveProfile(){
        // Save the profile

        UpdatedUser.Mnemonic_token = UserNameInput.text;
        UpdatedUser.Contact = ContactTelInput.text;        

        UpdateButton.RenderBusyState(true);

        PaganiniRestAPI.User.CreateOrUpdate(UpdatedUser.ToAPI(), SaveProfileSucceed, SaveProfileFailed);
    }

    /// <summary>
    /// Cancel the profile
    /// </summary>
    public void CancelProfile(){
        // Cancel the profile
        DisplayUserData();
        ErrorText.gameObject.SetActive(false);
    }

    // private events

    /// <summary>
    /// Handle the CreateOrUpdate callback, when the user profile is saved successfully
    /// </summary>
    /// <param name="user">The user resource returned by the api</param>
    private void SaveProfileSucceed(UserAPIResult user){

        if (IsNewUser)
        {
            UpdatedUser.Id = user.user_id;
            UpdatedUser.FromAPI = true;
        }         

        if (UpdatedUser.ProfilePic != null) {
            UpdatedUser.IsDirty = true;
        }

        UpdatedUser.Insert();        

        AppState.CurrentUser = UpdatedUser;
        DisplayUserData();

        ErrorText.gameObject.SetActive(false);
        UpdateButton.RenderBusyState(false);
    }

    /// <summary>
    /// Handle the photo selected event
    /// </summary>
    /// <param name="error">The error message returned by the api</param>
    private void SaveProfileFailed(string error){
        Debug.Log("Failed to save profile: " + error);
        ErrorText.gameObject.SetActive(true);
        UpdateButton.RenderBusyState(false);
        
        ToastMessageManager.Instance.Toast.RenderAlertToast("Fehler beim Speichern des Benutzerprofils", 
        "Überprüfen Sie die Internetverbindung und wenden Sie sich an den technischen Support, falls das Problem weiterhin besteht.");
    }

    /// <summary>
    /// Handle the photo selected event
    /// </summary>
    /// <param name="picTexture">The selected photo texture</param>
    private void PhotoChangedHandler(byte[] picBytes){
        if (picBytes != null) {
            
            UpdatedUser.ProfilePic = picBytes;
            if (!IsNewUser) {
                UpdatedUser.IsDirty = true;
                UpdatedUser.InsertDirty();
            }
        }

    }

    private void OnDestroy(){
        UserPhoto.OnProfilePicChanged.RemoveListener(PhotoChangedHandler);
    }

}
