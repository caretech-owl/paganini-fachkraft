using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Route;

public class RouteItemCellPrefab : MonoBehaviour
{
    public GameObject CellStatus;
    public TMPro.TMP_Text CellText;
    public LandmarkIcon Icon;

    public  Color StatusCleaning;
    public  Color StatusDiscussion;
    public  Color StatusAdaptation;
    public  Color StatusTraining;
    public  Color StatusDiscarded;
    public  Color StatusCompleted;

    private Dictionary<RouteStatus, Color> StatusColor;

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

    public void SetCellStatus(RouteStatus status)
    {
        SetupColors();
        //CellStatus.SetActive(status);
        Image image = CellStatus.GetComponent<Image>();
        image.color = StatusColor[status];
    }

    public void SetCellIcon(LandmarkIcon.LandmarkType iconType)
    {
        Icon.SelectedLandmarkType = iconType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupColors() {
        if (StatusColor != null) return;

        StatusColor = new Dictionary<RouteStatus, Color>()
            {
                { RouteStatus.New,  StatusCleaning},
                { RouteStatus.DraftPrepared, StatusDiscussion},
                { RouteStatus.DraftNegotiated, StatusAdaptation },
                { RouteStatus.Training, StatusTraining},
                { RouteStatus.Completed, StatusCompleted },
                { RouteStatus.Discarded, StatusDiscarded},
            };
    }


}
