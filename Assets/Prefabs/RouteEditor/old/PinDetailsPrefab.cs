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
    [Header("Pin Data")]
    public TMPro.TMP_Text PinTitle;
    public TMPro.TMP_InputField PinDescription;
    public Toggle PinType;

    public PhotoGallery PhotoList;


    [Header("UI Controllers")]
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
            PhotoList.AddItem(photo, 0);
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


    public void SaveChanges()
    {
        CurrentPathpoint.Description = PinDescription.text;
        CurrentPathpoint.POIType = PinType.isOn ? Pathpoint.POIsType.Landmark : Pathpoint.POIsType.Reassurance;
        CurrentPathpoint.IsDirty = true;
        CurrentPathpoint.Insert();
    }
    


}
