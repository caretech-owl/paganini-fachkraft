using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncStatusView : MonoBehaviour
{
    public TMPro.TMP_Text TabletName;
    public TMPro.TMP_Text PhoneName;

    // Start is called before the first frame update
    void Start()
    {
        TabletName.text = SyncState.Instance.TabletName ?? "";
        PhoneName.text = SyncState.Instance.PhoneName ?? "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class SyncState : PersistentLazySingleton<SyncState>
{
    public string TabletName;
    public string PhoneName;

    public void ClearValues()
    {
        TabletName = null;
        PhoneName = null;
    }


}
