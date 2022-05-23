using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneController : MonoBehaviour
{

    // Static members are 'eagerly initialized', that is, 
    // immediately when class is loaded for the first time.
    // .NET guarantees thread safety for static initialization
    private static readonly SceneController instance = new SceneController();

    public static SceneController GetSceneController()
    {
        return instance;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
