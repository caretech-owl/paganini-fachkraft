using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static string UserManageScene = "UserManager";
    public static string RouteExplorerScene = "RouteExplorer";
    public static string SynchronisationScene = "SyncManager";
    public static string RouteEditorScene = "RouteEditor";
    public static string LoginScene = "Login";

    public static void LoadUserManager()
    {
        SceneManager.LoadScene(UserManageScene);

        // Restore default screen dimming timeout
        Screen.sleepTimeout = AppState.ScreenSleepTimeout;
    }

    public static void LoadRouteExplorer()
    {
        SceneManager.LoadScene(RouteExplorerScene);

        // Restore default screen dimming timeout
        Screen.sleepTimeout = AppState.ScreenSleepTimeout;
    }

    public static void LoadSynchronisation()
    {
        SceneManager.LoadScene(SynchronisationScene);

        // Restore default screen dimming timeout
        Screen.sleepTimeout = AppState.ScreenSleepTimeout;
    }

    public static void LoadRouteEditor()
    {
        SceneManager.LoadScene(RouteEditorScene);

        // Restore default screen dimming timeout
        Screen.sleepTimeout = AppState.ScreenSleepTimeout;
    }

    public static void LoadLogin()
    {
        SceneManager.LoadScene(LoginScene);

        // Restore default screen dimming timeout
        Screen.sleepTimeout = AppState.ScreenSleepTimeout;
    }

}
