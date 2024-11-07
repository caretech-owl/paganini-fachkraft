using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Updates the local user list with the provided list of users from the API.
/// /// This class manages the users from an organization. It handles the initialization of the user list,
/// user profile views, and manages the navigation between these views.
/// </summary>
public class UserManagerController : MonoBehaviour
{
    [Header("Main views")]
    public GameObject UI_LIST; // The GameObject representing the user list view.
    public GameObject UI_USER_PROFILE; // The GameObject representing the user profile view.
    public GameObject UI_SW_PROFILE; // The GameObject representing the social worker profile view.

    [Header("UI Components")]
    public UserSelectionView UserListView; // The component handling the user list logic.

    public UserProfileView ProfileView; // The component handling the user profile logic.
    public SWProfileView SWProfileView; // The component handling the social worker profile logic.

    // Deep link options
    public const string DEEP_LINK_USER_SELECTION = "UserSelectionView";
    public const string DEEP_LINK_USER_PROFILE = "UserProfileView";
    public const string DEEP_LINK_NEW_USER = "NewUserView";
    public const string DEEP_LINK_SW_PROFILE = "SWProfileView";

    private void Awake()
    {
        DBConnector.Instance.Startup();
    }

    /// <summary>
    /// Start is called before the first frame update. It sets the organization title, initializes the user list,
    /// fetches all users from the API, and renders the user list view.
    /// </summary>
    void Start()
    {
        switch (AppState.CurrentDeepLink)
        {
            case DEEP_LINK_USER_PROFILE:
                ShowUserProfile();
                break;
            case DEEP_LINK_SW_PROFILE:
                ShowSWProfile();
                break;
            case DEEP_LINK_NEW_USER:
                ShowNewUser();
                break;                
            case DEEP_LINK_USER_SELECTION:
            default:
                ShowUserList();
                break;
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// This function shows the user list view.   
    /// </summary>
    public void ShowUserList(){
        RenderView(UI_LIST);
        UserListView.InitialiseView();
    }

    /// <summary>
    /// This function shows the user profile view.  
    /// </summary>
    public void ShowUserProfile(){
        ProfileView.DisplayUserData();
        RenderView(UI_USER_PROFILE);
    }

    /// <summary>
    /// This function shows the new user view.
    /// </summary>
    public void ShowNewUser(){
        ProfileView.DisplayNewUserForm();
        RenderView(UI_USER_PROFILE);
    }

    /// <summary>
    /// This function shows the social worker profile view.
    /// </summary>
    public void ShowSWProfile(){
        SWProfileView.DisplaySWData();
        RenderView(UI_SW_PROFILE);
    }

    /// <summary>
    /// This function renders the view passed as a parameter and hides the other views.
    /// </summary>
    /// <param name="view">The view to be rendered.</param>
    public void RenderView(GameObject view)
    {
        UI_LIST.SetActive(UI_LIST == view);
        UI_USER_PROFILE.SetActive(UI_USER_PROFILE == view);    
        UI_SW_PROFILE.SetActive(UI_SW_PROFILE == view);    
    }

}
