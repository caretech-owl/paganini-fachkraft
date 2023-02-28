using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PinDetailsEvent : UnityEvent<Pathpoint>
{
}

public class PinDetailsPrefab : MonoBehaviour
{

    public TMPro.TMP_Text PinTitle;
    public TMPro.TMP_InputField PinDescription;
    public PhotoListPrefab PhotoList;


    public Button ButtonClose;
    public PinDetailsEvent OnCloseDetails;

    private Pathpoint CurrentPathpoint;

    // Start is called before the first frame update
    void Start()
    {
        ButtonClose.onClick.AddListener(DetailsClosed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPin(Pathpoint point)
    {
        CurrentPathpoint = point;

        PhotoList.Clearlist();
        foreach(var photo in point.Photos)
        {
            PhotoList.AddItem(photo);
        }

    }

    public void DetailsClosed()
    {
        if (OnCloseDetails != null)
        {
            OnCloseDetails.Invoke(CurrentPathpoint);
            Destroy(this.gameObject);
        }
    }


}
