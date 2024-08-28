using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the users from a organisation.
/// </summary>
public class UserManagerController : MonoBehaviour
{
    public GameObject UserListView;
    public TMPro.TMP_Text OrganisationTitle;
    private UserListPrefab UserList;

    private void Awake()
    {
        DBConnector.Instance.Startup();
    }

    // Start is called before the first frame update
    void Start()
    {
        OrganisationTitle.text = AppState.CurrenSocialWorker.WorksName;
        UserList = UserListView.GetComponent<UserListPrefab>();

        PaganiniRestAPI.User.GetAll(GetUserSucceeded, GetUserFailed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="users">The list of users returned by the API.</param>
    private void GetUserSucceeded(UserAPIList users)
    {
        List<User> list = User.ToModelList(users);        

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

    public void Logout()
    {
        AuthToken.DeleteAll();
        AppState.ResetValues();
        SceneSwitcher.LoadLogin();
    }

}
