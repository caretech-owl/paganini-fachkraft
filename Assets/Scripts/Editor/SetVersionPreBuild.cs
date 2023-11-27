using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Build.Reporting;

public class SetVersionPreBuild : UnityEditor.Build.IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        string buildDateString = DateTime.Now.ToString("yyyyMMddHHmmss");        

        PlayerSettings.bundleVersion = PlayerSettings.bundleVersion + "." + buildDateString;
        Debug.Log("Setting bundle version to: " + PlayerSettings.bundleVersion);
    }
}
