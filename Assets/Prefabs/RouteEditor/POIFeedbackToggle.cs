using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class POIFeedbackToggle : MonoBehaviour
{
    public Toggle IrrelevanceToggle;
    public Toggle FamiliarityToggle;

    public Pathpoint CurrentPOI;

    private void Start()
    {
        IrrelevanceToggle.onValueChanged.AddListener(onRelevanceFeedbackChanged);
        FamiliarityToggle.onValueChanged.AddListener(onFamiliarityFeedbackChanged);
    }

    public void FillDiscussionFeedback(Pathpoint poi) {
        CurrentPOI = poi;

        IrrelevanceToggle.isOn = poi.RelevanceFeedback == Pathpoint.POIFeedback.No;
        FamiliarityToggle.isOn = poi.FamiliarityFeedback == Pathpoint.POIFeedback.Yes;
    }

    private void onRelevanceFeedbackChanged(bool irrelevant)
    {
        CurrentPOI.RelevanceFeedback = irrelevant ? Pathpoint.POIFeedback.No : Pathpoint.POIFeedback.Yes;
        CurrentPOI.InsertDirty();
    }

    private void onFamiliarityFeedbackChanged(bool value)
    {
        CurrentPOI.FamiliarityFeedback = value ? Pathpoint.POIFeedback.Yes : Pathpoint.POIFeedback.No;
        CurrentPOI.InsertDirty();
    }


    
}
