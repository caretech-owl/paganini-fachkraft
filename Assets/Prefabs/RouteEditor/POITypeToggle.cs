using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class POITypeToggleEvent : UnityEvent<Pathpoint.POIsType>
{
}

public class POITypeToggle : MonoBehaviour
{
    public POITypeToggleEvent OnValueChanged;

    private ToggleGroup toggleGroup;

    private bool enableEvents = false;

    // Start is called before the first frame update
    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();

        foreach (var item in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            item.onValueChanged.AddListener(delegate {
                if (!enableEvents) return;
                if (item.isOn)
                {
                    OnValueChanged.Invoke(GetSelectedPOIType());
                }
                //else if (!toggleGroup.AnyTogglesOn())
                //{
                //    OnValueChanged.Invoke(null);
                //}
            });
        }
    }


    public void SetSelectedPOIType(Pathpoint.POIsType pOIsType) {

        enableEvents = false;

        toggleGroup = GetComponent<ToggleGroup>();

        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.name.Contains(pOIsType.ToString()))
            {
                toggle.isOn = true;
            } else {
                toggle.isOn = false;
            }
        }

        enableEvents = true;
    }


    Pathpoint.POIsType GetSelectedPOIType()
    {
        Pathpoint.POIsType t = Pathpoint.POIsType.Landmark;
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            if (toggle.name == "ReassuranceRadio")
            {
                t = Pathpoint.POIsType.Reassurance;
            }
            else
            {
                t = Pathpoint.POIsType.Landmark;
            }

        }
        return t;
    }

    
}
