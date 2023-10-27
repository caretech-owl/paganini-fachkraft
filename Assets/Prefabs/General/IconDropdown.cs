using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IconDropdownEvent : UnityEvent<LandmarkIcon.LandmarkType>
{
}

public class IconDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    public LandmarkIcon LandmarkIconTemplate;

    public IconDropdownEvent OnValueChanged;

    private int defaultIndex = -1;
    List<TMP_Dropdown.OptionData> customOptions = new();

    private readonly List<LandmarkIcon.LandmarkType> displayOrder = new List<LandmarkIcon.LandmarkType>
    {
        LandmarkIcon.LandmarkType.Home,
        LandmarkIcon.LandmarkType.Work,
        LandmarkIcon.LandmarkType.Park,
        LandmarkIcon.LandmarkType.Shopping,
        LandmarkIcon.LandmarkType.Train,
        LandmarkIcon.LandmarkType.Bus,
        LandmarkIcon.LandmarkType.Coffee
    };

    void Start()
    {
        GenerateOptions();

        dropdown.onValueChanged.AddListener(handleValueChanged);
    }

    public void GenerateOptions()
    {
        // Clear existing options
        dropdown.ClearOptions();        

        customOptions = new List<TMP_Dropdown.OptionData>();
        foreach (LandmarkIcon.LandmarkType icon in displayOrder)
        {
            if (icon != LandmarkIcon.LandmarkType.Placeholder)
            {
                Sprite sprite = LandmarkIconTemplate.GetIcon(icon);

                TMP_Dropdown.OptionData customOption = new TMP_Dropdown.OptionData(icon.ToString(), sprite);
                customOptions.Add(customOption);
            }
        }

        dropdown.AddOptions(customOptions);
        if (defaultIndex >= 0)
        {
            dropdown.value = defaultIndex;
        }
    }

    public void SetDefaultOption(LandmarkIcon.LandmarkType defaultOption)
    {
        defaultIndex = displayOrder.IndexOf(defaultOption);
        if (customOptions.Count > 0)
        {
            dropdown.value = defaultIndex;
        }
    }

    private void handleValueChanged(int index)
    {
        OnValueChanged?.Invoke(displayOrder[index]);
    }

    private void OnDestroy()
    {
        dropdown.ClearOptions();
    }
}
