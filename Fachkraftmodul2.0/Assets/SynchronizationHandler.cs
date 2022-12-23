using Assets;
using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FTSCore;
using static InternalDataModel;

public class SynchronizationHandler : MonoBehaviour
{
    public GameObject OverviewListContentPrefab;
    public GameObject OverviewListContentAlternatePrefab;
    public GameObject SyncOverviewListContentPrefab;
    public GameObject SyncOverviewListContentAlternatePrefab;
    public GameObject SmartphoneFoundPrefab;
    public GameObject SmartphoneAskForConnectionText;
    public GameObject OverviewList;
    public GameObject SyncOverviewList;
    public GameObject TextOverviewPanelAskForConnection;
    public GameObject SVGImageSearchPanelAskForConnection;



    public FileTransferServer FTS;
    public GameObject UI_Overview;
    public GameObject UI_PanelStart;
    public GameObject UI_PanelSearch;
    public GameObject UI_PanelFound;
    public GameObject UI_PanelAskForConnection;
    public GameObject UI_SyncOverview;
    public GameObject UI_PanelFileTransferrunning;
    public GameObject UI_PanelEnd;



    bool alternate = false;
    bool alternateSync = false;
    bool wasCalled2 = true;
    bool wasSendPollRequestCalled = false;

    List<string> FileListToSyncronize;
    FTSCore.FileRequest fileInfo;

    string currentWayFolderName;

    // Start is called before the first frame update
    void Start()
    {

        // Reset overview list
        ResetOverviewLists();

        // Check existence of shared folder
        if (!Directory.Exists(FileManagement.persistentDataPath + "/" + FTS._sharedFolder))
        {
            Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + FTS._sharedFolder);
        }

        // Delete all files in sharing folder
        try
        {
            foreach (var file in Directory.GetFiles(FileManagement.persistentDataPath + "/" + FTS._sharedFolder))
            {
                File.Delete(file);
            }
        }
        catch (Exception ex)
        {
            ErrorHandlerSingleton.GetErrorHandler().AddNewError("SynchronizationHandler:Start(): Error in 'Delete all files in sharing folder'", ex.Message, false);
        }

        File.Create(FileManagement.persistentDataPath + "/" + FTS._sharedFolder + "/CONNECTIONALLOWED").Close();

        // load ways
        FillOverviewList();

        Debug.Log("SynchronizationHandler - Start() called!");
    }

    // Update is called once per frame
    void Update()
    {
        if (UI_PanelSearch.activeInHierarchy && !wasSendPollRequestCalled)
        {
            wasSendPollRequestCalled = true;
            FTS.SendPollRequest();
        }

        if (UI_PanelFileTransferrunning.activeInHierarchy && !wasCalled2)
        {
            wasCalled2 = true;
        }

    }

    public void InitSyncUI()
    {
        UI_PanelStart.SetActive(true);
        UI_PanelSearch.SetActive(false);
        UI_PanelFound.SetActive(false);
        UI_PanelAskForConnection.SetActive(false);
        UI_SyncOverview.SetActive(false);
        UI_PanelFileTransferrunning.SetActive(false);
        UI_PanelEnd.SetActive(false);

        TextOverviewPanelAskForConnection.GetComponent<TMP_Text>().text = @"Verbindung wird aufgebaut ...

Bestätige die Synchronisierung auf dem Smartphone.";

        SVGImageSearchPanelAskForConnection.SetActive(true);

        FileListToSyncronize = null;
    }

    public void SyncWayFromMobilePhone(int id)
    {
        UI_PanelFileTransferrunning.SetActive(true);

        List<DetailedWayExport> listOfWays = new List<DetailedWayExport>();
        FileListToSyncronize = new List<string>();

        if (File.Exists((FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml")))
        {
            Debug.Log(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml exist!");

            using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml"))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<DetailedWayExport>));
                listOfWays = (List<DetailedWayExport>)xmlSerializer.Deserialize(xmlReader);
            }
        }
        else
        {
            Debug.LogWarning(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml does not exist!");
            ErrorHandlerSingleton.GetErrorHandler().AddNewError("SynchronizationHandler:SyncWayFromMobilePhone(): ", FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml does not exist!");
        }

        DataOfExploritoryRouteWalks erw = InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.Find(x => x.Id.Equals(id));

        if (erw == null) //NEW AND INSERT
        {
            erw = new DataOfExploritoryRouteWalks();

            DetailedWayExport dwe = listOfWays.Find(x => x.Id.Equals(id));

            // fill ERW
            erw.Description = dwe.Description;
            erw.Destination = dwe.Destination;
            erw.DestinationType = dwe.DestinationType;
            erw.Folder = dwe.Folder;
            erw.Id = dwe.Id;
            erw.Name = dwe.Name;
            erw.Start = dwe.Start;
            erw.StartType = dwe.StartType;
            erw.Status = dwe.Status;
            erw.Videos = new List<string>();
            erw.Photos = new List<string>();

            currentWayFolderName = erw.Folder;

            foreach (var file in dwe.Files)
            {
                FileInfo fi = new FileInfo(file.File);
                FTS.RequestFile(0, fi.Name);

                Debug.Log("Video file extension is: " + fi.Extension);
                if (fi.Extension.Equals(".mp4"))
                {
                    Debug.Log("Video file is: " + fi.Name);
                    erw.Videos.Add(fi.Name);
                }
                else
                {
                    erw.Photos.Add(fi.Name);
                }

                FileListToSyncronize.Add(fi.Name);
            }

            // get coordinates
            FileInfo fiPoints = new FileInfo(dwe.Points);
            FTS.RequestFile(0, fiPoints.Name);
            FileListToSyncronize.Add(fiPoints.Name);

            InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.Add(erw);
        }
        else //UPDATE
        {
            currentWayFolderName = erw.Folder;
        }

        InternalDataModelController.GetInternalDataModelController().CheckDirtyFlagsAndSave();

        

    }

    public void FillOverviewList()
    {
        // Reset overview list
        ResetOverviewLists();

        foreach (var erw in InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks)
        {
            if (!alternate)
            {
                GameObject go = Instantiate(SyncOverviewListContentPrefab, OverviewList.transform);
                go.GetComponent<SyncOverviewElement>().InstantiateSyncOverviewElement(erw.Id, erw.Start, erw.Destination, DateTime.Now.ToString(), "Noch nicht bearbeitet", this);
                alternate = true;
            }
            else
            {
                GameObject go = Instantiate(SyncOverviewListContentAlternatePrefab, OverviewList.transform);
                go.GetComponent<SyncOverviewElement>().InstantiateSyncOverviewElement(erw.Id, erw.Start, erw.Destination, DateTime.Now.ToString(), "Noch nicht bearbeitet", this);
                alternate = false;
            }
        }
    }

    private void ResetOverviewLists()
    {
        // Reset all overview lists with objects of type 'SyncOverviewElement'
        while (GameObject.FindObjectsOfType(typeof(SyncOverviewElement)).Length > 0)
        {
            DestroyImmediate(((SyncOverviewElement)FindObjectsOfType(typeof(SyncOverviewElement))[0]).transform.gameObject, true);
        }

    }

    public void SearchSmartphone()
    {
        FTS.gameObject.SetActive(true);
    }

    public void AskForConnection()
    {
        Debug.Log("AskForConnection");
        FTS.RequestFile(0, "HANDSHAKE");
    }

    public void OnFileUpload(FileUpload fileUpload)
    {
        Debug.Log("OnFileUpload:fileUpload: " + fileUpload.GetName());

        if (fileUpload.GetName().Equals("CONNECTIONALLOWED"))
        {
            FTS.RequestFile(0, "waysForExport.xml");

            TextOverviewPanelAskForConnection.GetComponent<TMP_Text>().text = "Die Geräte sind verbunden.";
            SVGImageSearchPanelAskForConnection.SetActive(false);

        }
    }

    private void FillSyncOverviewList()
    {

        List<DetailedWayExport> listOfWays = new List<DetailedWayExport>();

        if (File.Exists((FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml")))
        {
            Debug.Log(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml exist!");

            using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml"))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<DetailedWayExport>));
                listOfWays = (List<DetailedWayExport>)xmlSerializer.Deserialize(xmlReader);
            }
        }
        else
        {
            Debug.LogWarning(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml does not exist!");
            ErrorHandlerSingleton.GetErrorHandler().AddNewError("SynchronizationHandler:FillSyncOverviewList(): ", FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + "waysForExport.xml does not exist!");
        }

        // Reset overview list
        ResetOverviewLists();

        // Sync overview list
        foreach (DetailedWayExport dwe in listOfWays)
        {
            if (!alternateSync)
            {
                GameObject go = Instantiate(SyncOverviewListContentPrefab, SyncOverviewList.transform);
                go.GetComponent<SyncOverviewElement>().InstantiateSyncOverviewElement(dwe.Id, dwe.Start, dwe.Destination, dwe.RecordingDate.ToString(), "Daten verfügbar", this);
                alternateSync = true;
            }
            else
            {
                GameObject go = Instantiate(SyncOverviewListContentAlternatePrefab, SyncOverviewList.transform);
                go.GetComponent<SyncOverviewElement>().InstantiateSyncOverviewElement(dwe.Id, dwe.Start, dwe.Destination, dwe.RecordingDate.ToString(), "Daten verfügbar", this);
                alternateSync = false;
            }
        }
    }

    // FTS event: On Device List Update ()
    public void UpdateDevicesList(List<FTSCore.RemoteDevice> devices)
    {

        List<string> list = FTS.GetDeviceNamesList();
        if (list.Count > 0)
        {
            SmartphoneFoundPrefab.GetComponentInChildren<TMP_Text>().text = list[0];
            SmartphoneAskForConnectionText.GetComponent<TMP_Text>().text = list[0];
            UI_PanelFound.SetActive(true);
        }
        else
        {
            wasSendPollRequestCalled = false;
        }
    }

    // FTS event: On File Download ()
    public void OnFileDownload(FileRequest file)
    {
        Debug.Log("CheckForMetadataFile: " + file._sourceName + "with status: " + file.GetStatus().ToString());

        if (file._sourceName.Equals("HANDSHAKE"))
        {
            UI_PanelAskForConnection.SetActive(true);
        }
        else if (file._sourceName.Equals("waysForExport.xml"))
        {
            fileInfo = file;

            UI_SyncOverview.SetActive(true);

            FillSyncOverviewList();

            wasCalled2 = false;
        }
        else if (file._sourceName.Equals("ENDOFSYNC"))
        {
            UI_PanelEnd.SetActive(true);
        }
        else if (file._sourceName.EndsWith(".mp4"))
        {
            // check for base folder
            if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName))
                Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName);

            // check for video folder
            if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video"))
                Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video");

            // copy video file
            File.Copy(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + file._sourceName, FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video/" + file._sourceName);

            Debug.Log("OnFileDownload - copy video file:" + file._sourceName);
        }
        else if (file._sourceName.EndsWith(".jpg"))
        {
            // check for base folder
            if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName))
                Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName);

            // check for photo folder
            if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto"))
                Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto");

            // copy photo file
            File.Copy(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + file._sourceName, FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto/" + file._sourceName);

            Debug.Log("OnFileDownload - copy photo file:" + file._sourceName);
        }
        else if (file._sourceName.EndsWith("-coordinates.xml"))
        {
            try
            {
                Debug.Log("OnFileDownload - deserialize coordinates:" + file._sourceName);

                List<Pathpoint> ppoints = new List<Pathpoint>();

                if (File.Exists(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + file._sourceName))
                {
                    using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + file._sourceName))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(List<Pathpoint>));
                        ppoints = (List<Pathpoint>)xmlSerializer.Deserialize(xmlReader);
                    }

                    InternalDataModel.DataOfExploritoryRouteWalks erw = InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.FindLast(x => x.Id == ppoints[0].Erw_id);

                    erw.Pathpoints = ppoints;


                    Debug.Log("OnFileDownload - deserialize erw.Pathpoints:" + erw.Pathpoints[0].Erw_id);

                    InternalDataModelController.GetInternalDataModelController().CheckDirtyFlagsAndSave();

                }
            }
            catch (IOException ex)
            {
                // todo: call error handler here
                Debug.LogError("OnFileDownload - deserialize coordinates:" + file._sourceName + "Error message: " + ex.Message);
            }
        }
        //else
        //{
            Debug.Log("CheckForMetadataFile: " + FileListToSyncronize.Count + " = FileListToSyncronize");

            if (FileListToSyncronize != null)
            {

                if (FileListToSyncronize.Contains(file._sourceName))
                    FileListToSyncronize.Remove(file._sourceName);

                if (FileListToSyncronize.Count == 0)
                {
                    UI_PanelEnd.SetActive(true);
                    FTS.RequestFile(0, "ENDOFSYNC");
                }
            }
        //}
    }

    public class DetailedWayExport
    {
        public string Folder { get; set; }
        public List<DetailedWayExportFiles> Files { get; set; }
        public string Points { get; set; }
        public int Id { set; get; }
        public string Start { set; get; }
        public string Destination { set; get; }
        public string StartType { set; get; }
        public string DestinationType { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }

        public System.DateTime RecordingDate { set; get; }
        public string RecordingName { set; get; }

        public int Status { set; get; }
    }

    public class DetailedWayExportFiles
    {
        public string File { get; set; }
        public byte[] Checksum { get; set; }
    }
}
