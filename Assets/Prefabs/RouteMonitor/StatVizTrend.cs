using System;
using NetTopologySuite.Geometries;
using UnityEngine;
using UnityEngine.UI;


public class StatVizTrend : MonoBehaviour
{

    public GameObject TrendContinous;
    public GameObject TrendDiscrete;
    public GameObject NoData;

    [Header("Trend continious")]
    public GameObject TrendDownIcon;
    public GameObject TrendUpIcon;
    public TMPro.TMP_Text ValueChange;

    [Header("Trend discrete")]
    public TMPro.TMP_Text StatValueDiscrete;

    private string valueTemplate = null;

    public Color TrendPositiveColor;
    public Color TrendNegativeColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FillNoData()
    {
        ShowView(NoData);
    }

    public void FillTrendContinous(int valueChange, bool isLowerBetter)
    {
        ShowView(TrendContinous);

        // trend color        
        int sign = (isLowerBetter ? -1 : 1) * valueChange;
        Color color = sign < 0 ? TrendNegativeColor : TrendPositiveColor;

        if (valueChange < 0)
        {
            RenderTrendIcon(TrendDownIcon, color);
        }
        else
        {
            RenderTrendIcon(TrendUpIcon, color);
        }

        ValueChange.text = Math.Abs(valueChange).ToString() + "%";
        ValueChange.color = color;
    }

    public void FillTrendDiscrete(int valueChange, bool isLowerBetter)
    {
        ShowView(TrendDiscrete);

        if (valueTemplate == null)
        {
            valueTemplate = StatValueDiscrete.text;
        }

        // trend color        
        int sign = (isLowerBetter ? -1 : 1) * valueChange;
        Color color = sign < 0 ? TrendNegativeColor : TrendPositiveColor;

        //if (valueChange < 0)
        //{
        //    RenderTrendIcon(TrendDownIcon, color);
        //}

        //ValueChange.text = Math.Abs(valueChange).ToString() + "%";


        StatValueDiscrete.text = valueTemplate.Replace("{0}", Math.Abs(valueChange).ToString());
        StatValueDiscrete.color = color;
    }


    // Utilities

    private void RenderTrendIcon(GameObject icon, Color color)
    {
        TrendDownIcon.SetActive(TrendDownIcon == icon);
        TrendUpIcon.SetActive(TrendUpIcon == icon);

        RenderTrendColor(icon, color);
    }

    private void RenderTrendColor(GameObject icon, Color color)
    {
        var fillImage = icon.GetComponentInChildren<Image>();
        fillImage.color = color;
    }


    private void ShowView(GameObject view)
    {
        TrendContinous.SetActive(TrendContinous == view);
        TrendDiscrete.SetActive(TrendDiscrete == view);
        NoData.SetActive(NoData == view);
    }

}
