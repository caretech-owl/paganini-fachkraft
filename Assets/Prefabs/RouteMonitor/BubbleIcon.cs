using System;
using UnityEngine;
using UnityEngine.UI;


public class BubbleIcon : MonoBehaviour
{
    [Header("Data components")]
    //public LandmarkIcon PinIcon;
    public TMPro.TMP_Text PinTitle;
    public TMPro.TMP_Text PinSubtitle;

    [Header("Icons")]
    public LandmarkIcon LocationIcon;
    //public GameObject CheckmarkIcon;
    public GameObject EditIcon;
    public GameObject RemovedIcon;
    public GameObject LandmarkIcon;
    public GameObject ReassuranceIcon;

    [Header("Direction Icons")]
    public GameObject DirectionPanel;
    public GameObject TurnLeftIcon;
    public GameObject TurnRightIcon;
    public GameObject StraightIcon;


    private Pathpoint PathpointItem;

    // Start is called before the first frame update
    void Start()
    {    
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void FillPathpoint(Pathpoint pathpoint, Way way, int index)
    {
        PathpointItem = pathpoint;

        if (pathpoint.POIType == Pathpoint.POIsType.WayStart)
        {
            PinTitle.text = "Start";
            RenderLocationIcon(way.StartType);
        }
        else if (pathpoint.POIType == Pathpoint.POIsType.WayDestination)
        {
            PinTitle.text = "Ziel";
            RenderLocationIcon(way.StartType);
        }
        else
        {
            PinTitle.text = "Pin " + index;
            RenderPOIType();
        }


        PinSubtitle.text = pathpoint.Description ?? "" ;

    }


    private void RenderPOIType()
    {
        LocationIcon.gameObject.SetActive(false);
        ReassuranceIcon.SetActive(PathpointItem.POIType == Pathpoint.POIsType.Reassurance);
        LandmarkIcon.SetActive(PathpointItem.POIType == Pathpoint.POIsType.Landmark);

        bool isLandmark = PathpointItem.POIType == Pathpoint.POIsType.Landmark;
        DirectionPanel.SetActive(isLandmark);
        if (isLandmark)
        {
            RenderDirection();
        }
    }

    private void RenderLocationIcon(string landmarkType)
    {
        LocationIcon.SetSelectedLandmark(Int32.Parse(landmarkType));

        LocationIcon.gameObject.SetActive(true);
        LandmarkIcon.SetActive(false);
        ReassuranceIcon.SetActive(false);
        EditIcon.SetActive(false);
        DirectionPanel.SetActive(false);
    }

    private void RenderDirection()
    {
        StraightIcon.SetActive(PathpointItem.Instruction.ToString() == "Straight");
        TurnLeftIcon.SetActive(PathpointItem.Instruction.ToString() == "LeftTurn");
        TurnRightIcon.SetActive(PathpointItem.Instruction.ToString() == "RightTurn");
    }

}
