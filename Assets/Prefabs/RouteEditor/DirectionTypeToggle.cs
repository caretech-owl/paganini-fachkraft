using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DirectionTypeToggleEvent : UnityEvent<string>
{
}

public class DirectionTypeToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public DirectionTypeToggleEvent OnValueChanged;

    private ToggleGroup toggleGroup;
    private Toggle prevActive;

    // Start is called before the first frame update
    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();

        foreach (var item in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            item.onValueChanged.AddListener(delegate {
                if (item.isOn)
                {
                    OnValueChanged.Invoke(GetSelectedDirectionType());
                    prevActive = item;
                }
                else if (!toggleGroup.AnyTogglesOn() && prevActive == item)
                {
                    OnValueChanged.Invoke("");
                }
            });
        }
        
    }

    public void SetSelectedDirectionType(string direction)
    {

        toggleGroup = GetComponent<ToggleGroup>();

        if (direction == null || direction.Trim() == "")
        {
            toggleGroup.SetAllTogglesOff(false);
            return;
        }

        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.name.Contains(direction))
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }

    string GetSelectedDirectionType()
    {

        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            if (toggle.name.Contains("LeftTurn"))
            {
                return "LeftTurn";
            }
            else if (toggle.name.Contains("Straight"))
            {
                return "Straight";
            }
            else if (toggle.name.Contains("RightTurn"))
            {
                return "RightTurn";
            }

        }

        return null;
    }

}
