using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class SyncOverviewElement : MonoBehaviour
{
    public TMPro.TMP_Text TextStart;
    public TMPro.TMP_Text TextFinish;
    public TMPro.TMP_Text TextDate;
    public TMPro.TMP_Text TextStatus;

    public GameObject ButtonSync;
    public GameObject ButtonWorkWith;

    private SynchronizationHandler SynchronizationHandler;

    int Id;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InstantiateSyncOverviewElement(int id, string start, string finish, string date, string status, SynchronizationHandler synchronizationHandler)
    {
        Id = id;

        TextStart.text = start;
        TextFinish.text = finish;
        TextDate.text = date;
        TextStatus.text = status;

        SynchronizationHandler = synchronizationHandler;

        if (status.Equals("Noch nicht bearbeitet"))
        {
            ButtonWorkWith.SetActive(true);
            ButtonSync.SetActive(false);
        }
    }

    public void TriggerSynchronizationInSynchronizationHandler()
    {
        SynchronizationHandler.SyncWayFromMobilePhone(Id);
    }

    public void OpenReviewForThisWay()
    {
        //try
        //{
        //    Debug.Log("OpenReviewForThisWay - deserialize coordinates");

        //    InternalDataModel.DataOfExploritoryRouteWalks erw = InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.FindLast(x => x.Id == Id);

        //    if (erw.Pathpoints == null)
        //    {
        //        List<PathpointAPI> ppoints = new List<Pathpoint>();

        //        if (File.Exists(FileManagement.persistentDataPath + "/RawDownloads/" + erw.Name + "-coordinates.xml"))
        //        {
        //            using (var xmlReader = new XmlTextReader(FileManagement.persistentDataPath + "/RawDownloads/" + erw.Name + "-coordinates.xml"))
        //            {
        //                var xmlSerializer = new XmlSerializer(typeof(List<PathpointAPI>));
        //                ppoints = (List<PathpointAPI>)xmlSerializer.Deserialize(xmlReader);
        //            }

        //            erw.Pathpoints = ppoints;

        //            Debug.Log("OpenReviewForThisWay - deserialize erw.Pathpoints:" + erw.Pathpoints[0].erw_id);

        //            InternalDataModelController.GetInternalDataModelController().CheckDirtyFlagsAndSave();

        //        }
        //    }
        //}
        //catch (IOException ex)
        //{
        //    // todo: call error handler here
        //    Debug.LogError("OpenReviewForThisWay - deserialize coordinates. \nError message: " + ex.Message);
        //}

        InternalDataModelController.GetInternalDataModelController().idm.currentIdOfWay = Id;
        SceneController.GetSceneController().LoadScene("ReviewLab");
    }
}
