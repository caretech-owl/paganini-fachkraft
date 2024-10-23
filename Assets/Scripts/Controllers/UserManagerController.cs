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

    [Header("UI Components")]
    public UserSelectionView UserListView; // The component handling the user list logic.

    public UserProfileView ProfileView; // The component handling the user profile logic.

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
        ShowUserList();
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
    ///  <param name="user">The user whose profile is to be displayed.</param>
    public void ShowUserProfile(User user){
        AppState.CurrentUser = user;
        ProfileView.DisplayUserData();
        RenderView(UI_USER_PROFILE);
    }

    /// <summary>
    /// This function renders the view passed as a parameter and hides the other views.
    /// </summary>
    /// <param name="view">The view to be rendered.</param>
    public void RenderView(GameObject view)
    {
        UI_LIST.SetActive(UI_LIST == view);
        UI_USER_PROFILE.SetActive(UI_USER_PROFILE == view);        
    }

}
