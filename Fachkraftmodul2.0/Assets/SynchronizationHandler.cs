using Assets;
using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
//using UnityEditorInternal.VersionControl;
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
    public GameObject UI_Overwrite;



    bool alternate = false;
    bool alternateSync = false;
    bool wasCalled2 = true;
    bool wasSendPollRequestCalled = false;


    List<DetailedWayExportFiles> FileListToSyncronize;
    List<DetailedWayExportFiles> FileListProcessed;
    FTSCore.FileRequest fileInfo;

    string currentWayFolderName;

    DataOfExploritoryRouteWalks erw;
    DetailedWayExport selectedDwe;

    // Start is called before the first frame update
    void Start()
    {

        // Reset overview list
        ResetOverviewLists();

        // Initialise folder
        ResetSynchronisationFolders();

        // load ways
        FillOverviewList();

        Debug.Log("SynchronizationHandler - Start() called!");
    }

    private void ResetSynchronisationFolders()
    {
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
        File.Create(FileManagement.persistentDataPath + "/" + FTS._sharedFolder + "/REQUEST-ERW-READY").Close();
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
        FileListProcessed = null;

        ResetSynchronisationFolders();
    }

    public void SyncWayFromMobilePhone(int id)
    {
 
        UI_PanelFileTransferrunning.SetActive(true);

        List<DetailedWayExport> listOfWays = ParseDWEFile("waysForExport.xml");        

        erw = InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.Find(x => x.Id.Equals(id));

        selectedDwe = listOfWays.Find(x => x.Id.Equals(id));

        if (erw == null) //NEW AND INSERT
        {
            erw = new DataOfExploritoryRouteWalks();
            InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.Add(erw);
            RequestSelectedERW();
        }
        else //UPDATE
        {
            currentWayFolderName = erw.Folder;

            UI_Overwrite.SetActive(true);
            UI_SyncOverview.SetActive(false);
        }

    }

    private List<DetailedWayExport> ParseDWEFile(string fileName)
    {
        List<DetailedWayExport> listOfWays = new List<DetailedWayExport>();

        if (File.Exists((FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName)))
        {
            Debug.Log(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName + " exist!");

            using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<DetailedWayExport>));
                listOfWays = (List<DetailedWayExport>)xmlSerializer.Deserialize(xmlReader);
            }
        }
        else
        {
            Debug.LogWarning(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName +  " does not exist!");
            ErrorHandlerSingleton.GetErrorHandler().AddNewError("SynchronizationHandler:ParseDWEFile(): ", FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName + " does not exist!");
        }

        return listOfWays;
    }


    private void RequestSelectedERW()
    {
        string msg = $"REQUEST-ERW-{selectedDwe.Id}";
        FTS.RequestFile(0, msg);
    }

    private void PrepareFilesToDownload()
    {

        FileListToSyncronize = new List<DetailedWayExportFiles>();
        FileListProcessed = new List<DetailedWayExportFiles>();

        List<DetailedWayExport> list = ParseDWEFile($"{selectedDwe.Name}-manifest.xml");
        DetailedWayExport dwe = list.First();

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
            FileListToSyncronize.Add(file);
        }

        // get coordinates
        FileInfo fiPoints = new FileInfo(dwe.Points);
        FTS.RequestFile(0, fiPoints.Name);

        DetailedWayExportFiles dwePoints = new DetailedWayExportFiles();
        dwePoints.File = fiPoints.Name;
        FileListProcessed.Add(dwePoints);

        

       // InternalDataModelController.GetInternalDataModelController().CheckDirtyFlagsAndSave();

    }

    public void OverwriteRoute()
    {        
        Directory.Delete(FileManagement.persistentDataPath + "/" + currentWayFolderName, true);
        RequestSelectedERW();
    }

    public void CancelSynchronisation()
    {
        Debug.Log("CancelSynchronisation: Requesting ENDOFSYNC");
        FTS.RequestFile(0, "ENDOFSYNC");
    }

    private void RequestNextFile()
    {
        if (FileListToSyncronize.Count > 0) { 
            DetailedWayExportFiles firstItem = FileListToSyncronize[0];
            FileInfo fi = new FileInfo(firstItem.File);
            FTS.RequestFile(0, fi.Name);
            FileListProcessed.Add(firstItem);
            FileListToSyncronize.RemoveAt(0);
        }
        else
        {
            Debug.Log("RequestNextFile: Nothing left; Requesting ENDOFSYNC");
            FTS.RequestFile(0, "ENDOFSYNC");
        }

    }


    public void ProcessDownloadedData()
    {
        // check for base folder
        if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName))
            Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName);

        // check for video folder
        if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video"))
            Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video");

        // check for photo folder
        if (!Directory.Exists(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto"))
            Directory.CreateDirectory(FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto");

        // Reassemble file chunks
        ReassembleFilesInFolder(FileManagement.persistentDataPath + "/" + FTS._downloadFolder);

        foreach (DetailedWayExportFiles file in FileListProcessed)
        {
            FileInfo fi = new FileInfo(file.File);
            string fileName = fi.Name;

            Debug.Log("ProcessDownloadedData - fileName: " + fileName);

            // get the original name, the chunk belongs to
            if (fileName.EndsWith(".chunk"))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
                string[] fileNameComponents = fileName.Split('.');
                fileName = String.Join(".", fileNameComponents.Take(fileNameComponents.Length - 1));
            }            

            if (fileName.EndsWith(".mp4"))
            {
                // Check whether we already processed the video
                if (erw.Videos.Contains(fileName)) continue;

                erw.Videos.Add(fileName);
                File.Copy(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName,
                    FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Video/" + fileName);

            }
            else if (fileName.EndsWith(".jpg"))
            {
                erw.Photos.Add(fileName);
                File.Copy(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName,
                    FileManagement.persistentDataPath + "/" + currentWayFolderName + "/Foto/" + fileName);

            }
            else if (fileName.EndsWith("-coordinates.xml"))
            {
                ProcessPathpoints(fileName);
            }
        }

        InternalDataModelController.GetInternalDataModelController().CheckDirtyFlagsAndSave();

    }


    private void ProcessPathpoints(string fileName)
    {
        try
        {
            List<Pathpoint> ppoints = new List<Pathpoint>();

            if (File.Exists(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName))
            {
                using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/" + FTS._downloadFolder + "/" + fileName))
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Pathpoint>));
                    ppoints = (List<Pathpoint>)xmlSerializer.Deserialize(xmlReader);
                }

                erw.Pathpoints = ppoints;

                Debug.Log("ProcessPathpoints - deserialize erw.Pathpoints: " + erw.Pathpoints[0].Erw_id);

            }
        }
        catch (IOException ex)
        {
            // todo: call error handler here
            Debug.LogError("ProcessPathpoints - deserialize coordinates:" + fileName + "Error message: " + ex.Message);
        }
    }


    private void ReassembleFilesInFolder(string folder)
    {

        HashSet<string> processedFiles = new HashSet<string>();

        int ChunkSize = 50 * 1024 * 1024;
        // Get a list of all chunk files in the folder
        string[] chunkFiles = Directory.GetFiles(folder, "*.chunk");

        foreach (string chunkFile in chunkFiles)
        {
            // Get the original file name and chunk count from the chunk file name
            string fileName = Path.GetFileNameWithoutExtension(chunkFile);
            string[] fileNameComponents = fileName.Split('.');
            string originalFileName = String.Join(".", fileNameComponents.Take(fileNameComponents.Length - 1));
            Debug.Log("filenameComponents:" + fileNameComponents.Last());
            int chunkCount = int.Parse(fileNameComponents.Last().Split('-').Last());
            
            if (!processedFiles.Contains(originalFileName))
            {
                // Reassemble the file
                ReassembleFile(folder, originalFileName, chunkCount, ChunkSize);

                // Add the file name to the processed files set
                processedFiles.Add(originalFileName);
            }

        }
    }


    private void ReassembleFile(string folder, string file,  int chunkCount, double ChunkSize)
    {
        Debug.Log("filename to ressemble: " + file);

        // Create a buffer for reading chunks
        byte[] buffer = new byte[(int)ChunkSize];

        // Construct the full path of the output file
        string outputFile = Path.Combine(folder, file);

        // Create a new FileStream for the output file
        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
        {
            for (int i = 0; i < chunkCount; i++)
            {
                // Construct the chunk file name
                string chunkFileName = Path.Combine(folder, $"{new FileInfo(file).Name}.part{i + 1}-{chunkCount}.chunk");

                // Open the chunk file for reading
                using (FileStream inputStream = new FileStream(chunkFileName, FileMode.Open))
                {
                    // Read the chunk into the buffer
                    int chunkSize = inputStream.Read(buffer, 0, buffer.Length);

                    // Write the chunk to the output file
                    outputStream.Write(buffer, 0, chunkSize);
                }
            }
        }

        // Delete the chunk files
        for (int i = 0; i < chunkCount; i++)
        {
            string chunkFileName = Path.Combine(folder, $"{new FileInfo(file).Name}.part{i + 1}-{chunkCount}.chunk");
            File.Delete(chunkFileName);
        }
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

        // The phone app accepted the connection
        // so we can proceed downloading the list of ways-ERW
        if (fileUpload.GetName().Equals("CONNECTIONALLOWED"))
        {
            FTS.RequestFile(0, "waysForExport.xml");

            TextOverviewPanelAskForConnection.GetComponent<TMP_Text>().text = "Die Geräte sind verbunden.";
            SVGImageSearchPanelAskForConnection.SetActive(false);

        }
        // The ERW manifest we requested is ready to be downloaded
        // This includes the files to be downloaded
        else if (fileUpload.GetName().Equals("REQUEST-ERW-READY"))
        {            
            string msg = $"{selectedDwe.Name}-manifest.xml";
            FTS.RequestFile(0, msg);
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
            ProcessDownloadedData();
        }
        else if (file._sourceName.StartsWith("REQUEST-ERW-"))
        {
            //RequestAllFiles();
            // We don't do anything. We wait for the client to
            // prepare the ERW data, and inform us via a
            // a request (REQUEST-ERW-READY)
        }
        else if (file._sourceName.EndsWith("-manifest.xml"))
        {
            PrepareFilesToDownload();            
        }
        else if (file._sourceName.EndsWith(".mp4") ||
            file._sourceName.EndsWith(".chunk") ||
            file._sourceName.EndsWith(".jpg") ||
            file._sourceName.EndsWith("-coordinates.xml"))
        {
            RequestNextFile();
        }

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
