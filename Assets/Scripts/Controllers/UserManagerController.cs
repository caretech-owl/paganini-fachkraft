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
    public GameObject UserListView; // The GameObject containing the user list.
    public TMPro.TMP_Text OrganisationTitle; // The TextMeshPro component displaying the organization title.
    private UserListPrefab UserList; // The component handling the user list logic.

    public UserProfilePrefab ProfileView; // The component handling the user profile logic.

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
        OrganisationTitle.text = AppState.CurrenSocialWorker.AtWorkshop.Name;
        UserList = UserListView.GetComponent<UserListPrefab>();

        PaganiniRestAPI.User.GetAll(GetUserSucceeded, GetUserFailed);
        
        RenderView(UI_LIST);
        UserList.Clearlist();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="users">The list of users returned by the API.</param>
    private void GetUserSucceeded(UserAPIList users)
    {
        UpdateLocalUserList(users);        
        DisplayUserList();    
    }

    /// <summary>
    /// This function updates the local user list with the provided list of users from the API.
    /// </summary>
    /// <param name="users"></param>             
    private void UpdateLocalUserList(UserAPIList users)
    {        
        foreach (var userRes in users.users)
        {
            var localUser = User.Get(userRes.user_id);
            var apiUser = new User(userRes);

            if (localUser == null)
            {
                apiUser.WorkshopId = AppState.CurrenSocialWorker.AtWorkshop.Id;
                apiUser.Insert();
            }
            else
            {
                if (localUser.ProfilePic != null)
                {
                    apiUser.WorkshopId = AppState.CurrenSocialWorker.AtWorkshop.Id;
                    apiUser.ProfilePic = localUser.ProfilePic;
                    apiUser.Insert();
                }
            }
        }
    }    

    /// <summary>
    /// This function displays the user list in the user list view.
    /// </summary>
    private void DisplayUserList()
    {
        List<User> list = User.GetAll( u => u.WorkshopId == AppState.CurrenSocialWorker.AtWorkshop.Id);      

        foreach (var item in list)
        {
            UserList.AddItem(item);
        }        
    }

    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace fails.
    /// </summary>
    /// <param name="errorMessage">The error message returned by the API.</param>
    private void GetUserFailed(string errorMessage)
    {

    }

    /// <summary>
    /// This function processes the selected user, sets the current user in the user session, and load the next
    /// scene.
    /// </summary>
    /// <param name="user">The selected user.</param>
    public void LoadUser (User user)
    {
        AppState.CurrentUser = user;
        SceneSwitcher.LoadRouteExplorer();
    }
    /// <summary>
    /// This function shows the user list view.   
    /// </summary>
    public void ShowUserList(){
        RenderView(UI_LIST);
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
