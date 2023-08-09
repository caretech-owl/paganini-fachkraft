using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;
using SFB = SimpleFileBrowser;
using SimpleFileBrowser;
using Unity.Entities.UniversalDelegates;

public class FileImportHandler : MonoBehaviour
{
    // Change this to the desired directory path on the external volume.

    private string SourceFolderPath;
    private string SourceFolderPathName;

    public SynchronizationController SyncProcess;

    private List<SynchronizationController.DetailedWayExport> RouteList;
    const string TempPathName = "RawImport";
    private string TempPath;

    // Route to import

    SynchronizationController.DetailedWayExport SelectedRoute;


    [Header("UI Configuration")]

    [SerializeField] private GameObject VolumeBrowsePanel;
    [SerializeField] private GameObject VolumeSelectedPanel;
    [SerializeField] private GameObject VolumeCopyingPanel;
    [SerializeField] private GameObject ImportOverviewPanel;
    [SerializeField] private GameObject ImportOverwritePanel;
    [SerializeField] private GameObject ImportProcessingPanel;
    [SerializeField] private GameObject ImportErrorPanel;
    [SerializeField] private GameObject ImportEndPanel;

    [Header("Volume Data UI")]
    [SerializeField] private TMPro.TMP_Text VolumeNameText;
    [SerializeField] private TMPro.TMP_Text ErrorMessageText;
    [SerializeField] private TMPro.TMP_Text LogText;
    [SerializeField] private RouteListPrefab AvailableRoutes;



    void Start()
    {
        // Start a coroutine to detect volume insertion.

        TempPath = FileManagement.persistentDataPath + "/" + TempPathName;

    }

    public void Initialise()
    {
        SourceFolderPath = null;

        StartCoroutine(CheckPathCoroutine());

        try
        {
            SFB.FileBrowser.DirectNativeSAF();
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }

        //StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator CheckPathCoroutine()
    {

        Debug.Log("CheckPathCoroutine(): " + (SFB.FileBrowser.GetCurrentPath() == null));
        yield return new WaitForSeconds(1.0f);

        if (SFB.FileBrowser.GetCurrentPath() != null) {
            SourceFolderPathName = SFB.FileBrowser.GetCurrentPathName();
            SourceFolderPath = SFB.FileBrowser.GetCurrentPath();


            Debug.Log($"DestinationFolderName: {SourceFolderPathName} | DestinationFolderPath: {SourceFolderPath} ");

            VolumeNameText.text = SourceFolderPathName;

            DisplayScreenPanel(VolumeSelectedPanel);

        }
        else
        {
            yield return null;
        }

    }


    public void DisplayImportOverview()
    {
        DisplayScreenPanel(VolumeCopyingPanel);
        StartCoroutine(CopyAndShowAvailableRoute());
    }


    IEnumerator CopyAndShowAvailableRoute() {

        SelectedRoute = null;
        string overviewFilename = "waysForExport.xml";

        // create temp directory
        if (Directory.Exists(TempPath))
            Directory.Delete(TempPath, true);
        Directory.CreateDirectory(TempPath);
        try
        {

            SFB.FileBrowserHelpers.CopyDirectory(SourceFolderPath, TempPath);

            LogText.text += "SourceFolderPath: " + SourceFolderPath + "\n";

            //var ffs = SFB.FileBrowserHelpers.GetEntriesInDirectory(SourceFolderPath, true);

            //foreach (var f in ffs)
            //{
            //    Debug.Log($"file:{f.Name} {f.Path}");
            //    if (f.Name == overviewFilename) {
            //        overviewPath = f.Path;
            //    }
            //}

            //if (overviewPath == null)
            //    throw new Exception($"{overviewFilename} does not exists in selected import folder {SourceFolderPath}");


            //SFB.FileBrowserHelpers.CopyFile(overviewPath, tempPath);

            RouteList = SynchronizationController.DetailedWayExportFiles.ParseDWEFile(overviewFilename, TempPath);

            DisplayScreenPanel(ImportOverviewPanel);

            LogText.text += "Count: " + RouteList.Count + "\n";

            foreach (var item in RouteList)
            {
                Debug.Log($"item:{item.Name}");
                LogText.text = LogText.text + $"item:{item.Name}";

                //Way w = new Way();
                //w.Id = item.Id;
                //w.Name = item.Name;
                //w.StartType = item.StartType;
                //w.DestinationType = item.DestinationType;
                //w.Start = item.Start;
                //w.Destination = item.Destination;
                //w.Description = item.Description;
                //w.UserId = item.UserId;

                //Route r = new Route();
                //r.Id = item.Id;
                //r.Date = item.RecordingDate;
                //r.Name = item.RecordingName;

                //w.Routes = new List<Route>();
                //w.Routes.Add(r);

                AvailableRoutes.AddItem(item);
            }


        }
        catch (Exception e)
        {
            ErrorMessageText.text = e.Message;

            LogText.text += "\n" + e.StackTrace;

            DisplayScreenPanel(ImportErrorPanel);

            Debug.LogError(e.StackTrace);
        }

        yield return null;

    }


    public void OnRouteSelected(Way w, Route r) {

        // Do we need to prompt an overwrite dialog?
        bool alreadyExists = SyncProcess.CheckIfOverwriteRequired(r.Id);

        foreach (var item in RouteList)
        {
            if (item.Id == r.Id) {
                SelectedRoute = item;
            }
        }

        if (alreadyExists)
        {
            DisplayOverwrite();
            return;
        }

        if (SelectedRoute != null) {
            StartFilesImport(r.Id, SelectedRoute.RecordingName);
        }
        else
        {
            Debug.Log($"Import folder '{SelectedRoute.RecordingName}' not found.");
        }

        
    }


    private void DisplayScreenPanel(GameObject panel) {
        VolumeBrowsePanel.SetActive(VolumeBrowsePanel == panel);
        VolumeSelectedPanel.SetActive(VolumeSelectedPanel == panel);
        VolumeCopyingPanel.SetActive(VolumeCopyingPanel == panel);
        ImportOverviewPanel.SetActive(ImportOverviewPanel == panel);
        ImportOverwritePanel.SetActive(ImportOverwritePanel == panel);
        ImportProcessingPanel.SetActive(ImportProcessingPanel == panel);
        ImportErrorPanel.SetActive(ImportErrorPanel == panel);
        ImportEndPanel.SetActive(ImportEndPanel == panel);
    }


    private void DisplayOverwrite() {
        DisplayScreenPanel(ImportOverwritePanel);
    }


    private void SuccessullyFinished() {
        DisplayScreenPanel(ImportEndPanel);
    }


    public void ConfirmOverwrite() {
        StartFilesImport(SelectedRoute.Id, SelectedRoute.RecordingName);
    }

    public void CancelOverwrite()
    {
        ErrorMessageText.text = "Import cancelled.";

        DisplayScreenPanel(ImportErrorPanel);
    }


    public void StartFilesImport(int id, string folder)
    {
        Debug.Log($"Selected Id: '{id}' Folder: {folder}.");

        DisplayScreenPanel(ImportProcessingPanel);

        try {
            SyncProcess.SyncFromImportedFolder(id, TempPath, folder);
            SuccessullyFinished();
        }
        catch (Exception e) {
            ErrorMessageText.text = "Import Error: " + e.Message;

            DisplayScreenPanel(ImportErrorPanel);

            Debug.LogError(e.StackTrace);

        }
        
    }

}
