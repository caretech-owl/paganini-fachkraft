using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;
using System.Linq;

public class HotSpotController : MonoBehaviour
{
    public TMP_Text DebugLogger;
    //public static HotSpotController instance;

    //private void Awake()
    //{
    //    instance = this;
    //}
    ////Initialization, permission application should be made as early as possible
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //public static void Init()
    //{
    //    //First add the permission to the list, and then apply
    //    AndroidPermissionMgr.permissionList.Add("android.permission.CHANGE_WIFI_STATE");
    //    //AndroidPermissionMgr.permissionList.Add(Permission.ExternalStorageRead);
    //    //AndroidPermissionMgr.permissionList.Add(Permission.ExternalStorageWrite);
    //    //AndroidPermissionMgr.permissionList.Add(Permission.FineLocation);
    //    //AndroidPermissionMgr.permissionList.Add(Permission.CoarseLocation);
    //    AndroidPermissionMgr.StartCheckPermission(0.02f); //Start application

    //    Debug.Log("Permission application completed");
    //}

    //An array of strings for which you want permission
    string[] strs = new string[] {
        "android.permission.INTERNET",
        "android.permission.READ_PHONE_STATE",
        "android.permission.READ_EXTERNAL_STORAGE",
        "android.permission.WRITE_EXTERNAL_STORAGE",
        "android.permission.ACCESS_WIFI_STATE",
        "android.permission.ACCESS_NETWORK_STATE",
        "android.permission.GET_TASKS",
        "android.permission.REQUEST_INSTALL_PACKAGES",
        "android.permission.WAKE_LOCK",
        "android.permission.SYSTEM_ALERT_WINDOW",
        "android.permission.CHANGE_WIFI_STATE",
        "android.permission.CHANGE_NETWORK_STATE",
        "android.permission.ACCESS_COARSE_LOCATION",
        "android.permission.ACCESS_FINE_LOCATION",
        "android.permission.SYSTEM_OVERLAY_WINDOW",
        "android.permission.ACCESS_COARSE_UPDATES",
        "android.permission.WRITE_SETTINGS",
        "android.permission.BATTERY_STATS",
        "android.permission.MOUNT_UNMOUNT_FILESYSTEMS"
    };

    // Start is called before the first frame update
    void Start()
    {
        //Start dynamic request permission
        strs.ToList().ForEach(s => {
            Debug.Log("RequestUserPermission: " + s);
            if (!Permission.HasUserAuthorizedPermission(s))
            {
                Permission.RequestUserPermission(s);

                Debug.Log("add RequestUserPermission: " + s);
            }
            else
            {
                Debug.Log("it has RequestUserPermission: " + s);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setWifiEnabled(bool enabled)
    {
        DebugLogger.text = "";

        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    var isSetWifiEnabled = wifiManager.Call<bool>("setWifiEnabled", enabled);

                    Debug.Log("setWifiEnabled: call action" + enabled + " results in: " + isSetWifiEnabled);
                    DebugLogger.text = "setWifiEnabled: call action" + enabled + " results in: " + isSetWifiEnabled;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("setWifiEnabled: call action" + enabled + ": crashed");
                Debug.LogWarning(e.Message);

                DebugLogger.text = "setWifiEnabled: call action" + enabled + ": crashed";
                DebugLogger.text += "\n" + e.Message;
            }
        }
    }

    public void isWifiEnabled()
    {
        DebugLogger.text = "";

        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    var isWiFiEnabled = wifiManager.Call<bool>("isWifiEnabled");
                    Debug.Log("isWifiEnabled: " + isWiFiEnabled);
                    DebugLogger.text = "isWifiEnabled: " + isWiFiEnabled;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("isWifiEnabled: crashed");
                Debug.LogWarning(e.Message);

                DebugLogger.text = "isWifiEnabled: crashed";
                DebugLogger.text += "\n" + e.Message;
            }
        }
    }
}

//public static class AndroidPermissionMgr
//{
//    static int index;
//    public static List<string> permissionList = new List<string>();

//    public static void StartCheckPermission(float time)
//    {
//        Debug.Log("Start permission application");
//        if (permissionList.Count > 0)
//        {
//            Get(permissionList[index], time);
//        }
//    }

//    /// <summary>
//    ///External access method
//    /// </summary>
//    ///< param name = "type" > permission name < / param >
//    ///< param name = "time" > if you refuse, how long is the delay to apply again < / param >
//    static void Get(string type, float time)
//    {
//        if (!Permission.HasUserAuthorizedPermission(type))
//        {
//            Permission.RequestUserPermission(type);
//            Debug.Log("Obtaining permissions for:" + type);
//            HotSpotController.instance.StartCoroutine(Check(type, time));
//        }
//        else
//        {
//            Debug.Log("Permission obtained:" + type);

//            if (index < permissionList.Count - 1)
//            {
//                index += 1;
//                Get(permissionList[index], time);
//            }
//        }
//    }
//    static IEnumerator Check(string type, float time)
//    {
//        yield return new WaitForSeconds(time);
//        Get(type, time);
//    }
//}
