using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PhotoFeedbackEvent : UnityEvent<PathpointPhoto.PhotoFeedback>
{
}

public class PhotoFeedbackToggle : MonoBehaviour
{
    public PhotoFeedbackEvent OnValueChanged;

    private ToggleGroup toggleGroup;


    // Start is called before the first frame update
    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();

        foreach (var item in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            item.onValueChanged.AddListener(delegate {
                if (item.isOn)
                {
                    OnValueChanged.Invoke(GetFeedback());
                }
                else if (!toggleGroup.AnyTogglesOn())
                {
                    OnValueChanged.Invoke(PathpointPhoto.PhotoFeedback.None);
                }

                Debug.Log("ValueChanged: "+ item);
            });
        }
    }


    public void SetFeedbackValue(PathpointPhoto.PhotoFeedback feedback) {

        toggleGroup = GetComponent<ToggleGroup>();

        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.name.Contains(feedback.ToString()))
            {
                toggle.isOn = true;
            } else {
                toggle.isOn = false;
            }
        }
    }


    PathpointPhoto.PhotoFeedback GetFeedback()
    {
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            if (toggle.name.Contains("Keep")) {
                return PathpointPhoto.PhotoFeedback.Keep;
            } else {
                return PathpointPhoto.PhotoFeedback.Delete;
            }
        }
        return PathpointPhoto.PhotoFeedback.None; 
    }

    
}
