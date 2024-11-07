using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Route;

public class RouteItemCellPrefab : MonoBehaviour
{
    public GameObject CellStatus;
    public TMPro.TMP_Text CellText;
    public LandmarkIcon Icon;
    public Slider ProgressSlider;

    [Header("Status Icons")]
    public  GameObject StatusCleaning;
    public  GameObject StatusDiscussion;
    public  GameObject StatusTraining;
    public  GameObject StatusDiscarded;
    public  GameObject StatusCompleted;

    [Header("Completion Colors")]
    public Color WalkCompletedColor;
    public Color WalkIncompletedColor;

    void Awake () {
        
    }

    public void FillCell(string text)
    {
        //CellStatus.SetActive(false);
        //Icon.transform.gameObject.SetActive(false);
        SetCellText(text);

    }

    public void FillCell(string text, LandmarkIcon.LandmarkType iconType)
    {
        SetCellText(text);
        SetCellIcon(iconType);
    }

    public void FillCell(string text, RouteStatus status )
    {
        SetCellText(text);
        SetCellStatus(status);
    }

    public void SetCellText(string text)
    {
        CellText.text = text;
    }

    public void SetProgress(double progress)
    {
        ProgressSlider.value = (float)progress;
        SetCellText(Math.Round(progress * 100).ToString());
    }

    public void SetCellStatus(RouteStatus status)
    {
        StatusCleaning.SetActive(status == RouteStatus.New);
        StatusDiscussion.SetActive(status == RouteStatus.DraftPrepared);
        StatusTraining.SetActive(status == RouteStatus.Training);
        StatusCompleted.SetActive(status == RouteStatus.Completed);
        StatusDiscarded.SetActive(status == RouteStatus.Discarded);
    }    

    public void SetCompletedStatus(bool completed, bool renderEmpty = false)
    {
        Image image = CellStatus.GetComponent<Image>();

        if (renderEmpty)
        {
            image.gameObject.SetActive(false);
            return;
        }

        image.color = completed? WalkCompletedColor : WalkIncompletedColor;
    }

    public void SetCellIcon(LandmarkIcon.LandmarkType iconType)
    {
        Icon.SelectedLandmarkType = iconType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupColors(RouteStatus status) {
        
        StatusCleaning.SetActive(status == RouteStatus.New);
        StatusDiscussion.SetActive(status == RouteStatus.DraftPrepared);
        StatusTraining.SetActive(status == RouteStatus.Training);
        StatusCompleted.SetActive(status == RouteStatus.Completed);
        StatusDiscarded.SetActive(status == RouteStatus.Discarded);
    }


}
