using System.Collections;
using System.Collections.Generic;
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
}
