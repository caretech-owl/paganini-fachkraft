using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class POIFeedbackToggle : MonoBehaviour
{
    public Toggle IrrelevanceToggle;
    public Toggle FamiliarityToggle;
    public Toggle CleaningToggle;

    public Pathpoint CurrentPOI;

    [Header("Events")]
    public UnityEvent OnPOIUpdate;

    private bool enableEvents = false;

    private void Start()
    {
        IrrelevanceToggle?.onValueChanged.AddListener(onRelevanceFeedbackChanged);
        FamiliarityToggle?.onValueChanged.AddListener(onFamiliarityFeedbackChanged);
        CleaningToggle?.onValueChanged.AddListener(onCleaningFeedbackChanged);
    }

    public void FillDiscussionFeedback(Pathpoint poi) {
        CurrentPOI = poi;

        enableEvents = false;

        if (IrrelevanceToggle) IrrelevanceToggle.isOn = poi.RelevanceFeedback == Pathpoint.POIFeedback.No;
        if (FamiliarityToggle) FamiliarityToggle.isOn = poi.FamiliarityFeedback == Pathpoint.POIFeedback.Yes;
        if (CleaningToggle) CleaningToggle.isOn = poi.CleaningFeedback == Pathpoint.POIFeedback.No;

        enableEvents = true;
    }

    private void onRelevanceFeedbackChanged(bool irrelevant)
    {
        if (!enableEvents) return;

        CurrentPOI.RelevanceFeedback = irrelevant ? Pathpoint.POIFeedback.No : Pathpoint.POIFeedback.Yes;
        CurrentPOI.InsertDirty();

        OnPOIUpdate?.Invoke();
    }

    private void onFamiliarityFeedbackChanged(bool value)
    {
        if (!enableEvents) return;

        CurrentPOI.FamiliarityFeedback = value ? Pathpoint.POIFeedback.Yes : Pathpoint.POIFeedback.No;
        CurrentPOI.InsertDirty();

        OnPOIUpdate?.Invoke();
    }

    private void onCleaningFeedbackChanged(bool remove)
    {
        if (!enableEvents) return;

        CurrentPOI.CleaningFeedback = remove ? Pathpoint.POIFeedback.No : Pathpoint.POIFeedback.Yes;
        CurrentPOI.InsertDirty();

        OnPOIUpdate?.Invoke();
    }

}
