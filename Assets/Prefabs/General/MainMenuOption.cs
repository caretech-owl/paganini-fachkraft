using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuOption : MonoBehaviour
{
    private Button OptionButton;
    private bool IsInteractableChanged = false;

    [Header("Events")]
    public UnityEvent<string> OnOptionSelected;

    // Start is called before the first frame update
    void Start()
    {
        OptionButton = GetComponent<Button>();
        OptionButton.onClick.AddListener(OptionSelected);
    }

    void Update()
    {
        if (OptionButton != null && !OptionButton.interactable && !IsInteractableChanged)
        {
            SetChildrenOpacity(0.5f); // Set opacity to 0.5 when disabled
            IsInteractableChanged = true;
        }
        else if (OptionButton != null && OptionButton.interactable && IsInteractableChanged)
        {
            SetChildrenOpacity(1f); // Reset opacity to 1 when enabled
            IsInteractableChanged = false;
        }
    }

    public void EnableOption(bool enable){
        OptionButton.interactable = enable;
    }

    private void SetChildrenOpacity(float opacity)
    {
        // Iterate through all child components and change their opacity
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
        {
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, opacity);
        }
    }

    private void OptionSelected()
    {
        OnOptionSelected?.Invoke(name);
    }
}
