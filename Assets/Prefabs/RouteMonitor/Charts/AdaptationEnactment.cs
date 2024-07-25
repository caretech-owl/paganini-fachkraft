using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using static PathpointPIM;

public class AdaptationEnactment : MonoBehaviour
{
    public ToggleGroup ModesToggleGroup;

    public event EventHandler OnAdaptationValueChanged;

    private List<Toggle> toggles;
    private Pathpoint CurrentPOI;
    private bool IsPOI;

    private void Awake()
    {
        // Get all the toggles associated with the toggle group
        toggles = new List<Toggle>(ModesToggleGroup.GetComponentsInChildren<Toggle>());

        // Register OnToggleSelected on each of the toggle components
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleSelected);
        }
    }    

    // public 

    public void LoadAdaptation(Pathpoint poi, bool isPOI = true)
    {
        CurrentPOI = poi;
        IsPOI = isPOI;        

        // default when no instruction is present
        if (poi.CurrentInstructionMode == null)
        {
            RenderInstructionMode(SupportMode.Instruction);
            return;
        }

        var pim = poi.CurrentInstructionMode;

        var cached = PathpointPIM.GetAll(p=> p.Id == pim.Id);
        if (cached.Count > 0)
        {
            pim = cached.First();
        }
        CurrentPOI.CurrentInstructionMode = pim;

        if (isPOI)
        {
            RenderInstructionMode(pim.AtPOIMode);
        }
        else
        {
            RenderInstructionMode(pim.ToPOIMode);
        }
    }


    // private utils

    private void OnToggleSelected(bool isSelected)
    {
        // This should be triggered only once, when the value changes
        if (isSelected)
        {
            var toggle = ModesToggleGroup.ActiveToggles().First();

            Debug.Log("Toggle selected: " + toggle.name);

            var item = toggle.GetComponent<AdaptationItem>();

            UpdateInstructionMode(item.GetSupportMode());

            OnAdaptationValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void RenderInstructionMode(SupportMode supportMode)
    {
        foreach (Toggle toggle in toggles)
        {
            var item = toggle.GetComponent<AdaptationItem>();
            if (item.GetSupportMode() == supportMode)
            {
                toggle.SetIsOnWithoutNotify(true);
                item.RenderAsSelected(true);
            }
            else
            {
                item.RenderAsSelected(false);
            }
        }
    }

    private void UpdateInstructionMode(SupportMode supportMode)
    {
        Debug.Log("Update Mode to: " + supportMode);

        if (CurrentPOI == null) return;

        var current_pim = CurrentPOI.CurrentInstructionMode;

        if (current_pim == null)
        {
            current_pim = new PathpointPIM();
            current_pim.Id = - CurrentPOI.Id;
            current_pim.PathpointId = CurrentPOI.Id;
            current_pim.ActiveSinceDateTime = DateUtils.DateTimeNow();
            current_pim.IsAtPOINewToUser = true;
            current_pim.IsToPOINewToUser = true;
            current_pim.ToPOIMode = SupportMode.Instruction;
            current_pim.AtPOIMode = SupportMode.Instruction;

        }

        current_pim.RouteId = CurrentPOI.RouteId;

        if (IsPOI)
        {                        
            // we are unmuting, so we should remove the challenge from the ToPOI
            if (current_pim.AtPOIMode == SupportMode.Mute)
            {
                current_pim.ToPOIMode = SupportMode.Trivia;
                current_pim.IsToPOINewToUser = true;
            }
            // mute mode set the segment at challenge mode
            else if (supportMode == SupportMode.Mute)
            {
                current_pim.ToPOIMode = SupportMode.Challenge;
                current_pim.IsToPOINewToUser = true;
            }

            current_pim.AtPOIMode = supportMode;
            current_pim.IsAtPOINewToUser = true;
        }
        else
        {

            // we rollback a challenge, so we unmute the AtPOI
            if (current_pim.ToPOIMode == SupportMode.Challenge)
            {
                current_pim.AtPOIMode = SupportMode.Challenge;
                current_pim.IsAtPOINewToUser = true;
            }
            // mute mode set the segment at challenge mode
            else if (supportMode == SupportMode.Challenge)
            {
                current_pim.AtPOIMode = SupportMode.Mute;
                current_pim.IsAtPOINewToUser = true;
            }

            current_pim.ToPOIMode = supportMode;
            current_pim.IsToPOINewToUser = true;

        }

        CurrentPOI.CurrentInstructionMode = current_pim;

        current_pim.InsertDirty();
    }

    private void OnDestroy()
    {
        // Remove the OnToggleSelected from the toggles
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.RemoveListener(OnToggleSelected);
        }
    }

}
