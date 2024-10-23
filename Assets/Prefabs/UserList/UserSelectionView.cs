using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Updates the local user list with the provided list of users from the API.
/// /// This class manages the users from an organization. It handles the initialization of the user list,
/// user profile views, and manages the navigation between these views.
/// </summary>
public class UserSelectionView : MonoBehaviour
{

    public MainMenu LocalMenu;

    [Header("UI Components")]
    public GameObject UserListView; // The GameObject containing the user list.
    public TMPro.TMP_Text OrganisationTitle; // The TextMeshPro component displaying the organization title.
    private UserListPrefab UserList; // The component handling the user list logic.


    /// <summary>
    /// Start is called before the first frame update. It sets the organization title, initializes the user list,
    /// fetches all users from the API, and renders the user list view.
    /// </summary>
    void Start()
    {
        LocalMenu.SetCurrentOption(MainMenu.MenuOptions.SWITCH_USER);        
    }

    void Update()
    {
        
    }

    public void InitialiseView(){
        OrganisationTitle.text = AppState.CurrenSocialWorker.AtWorkshop.Name;
        UserList = UserListView.GetComponent<UserListPrefab>();

        PaganiniRestAPI.User.GetAll(GetUserSucceeded, GetUserFailed);
        
        UserList.Clearlist();
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

}
