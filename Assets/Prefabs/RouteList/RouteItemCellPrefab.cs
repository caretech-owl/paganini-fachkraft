using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using static Route;

public class RouteItemCellPrefab : MonoBehaviour
{
    public GameObject CellStatus;
    public TMPro.TMP_Text CellText;
    public LandmarkIcon Icon;



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
        //CellStatus.SetActive(status);
    }

    public void SetCellIcon(LandmarkIcon.LandmarkType iconType)
    {
        Icon.SelectedLandmarkType = iconType;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
