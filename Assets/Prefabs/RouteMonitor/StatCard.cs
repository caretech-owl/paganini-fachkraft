using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class StatCard : MonoBehaviour
{
    [Header("Header")]
    //public LandmarkIcon PinIcon;
    public TMPro.TMP_Text CardTitle;
    public TMPro.TMP_Text CardSubtitle;

    [Header("Start cards")]
    public GameObject TrendValueCard;
    public GameObject DecisionCard;
    public GameObject ModesCard;

    [Header("Trend value Card")]
    public StatVizTrend VizTrend;
    public TMPro.TMP_Text StatValue;
    public TMPro.TMP_Text StatExplanation;
    public GameObject StatIcon;

    [Header("Decision card")]
    public GameObject WrongTurnPanel;
    public GameObject MissedTurnPanel;
    public GameObject WrongDirection;
    public GameObject DeviationPanel;
    public GameObject CorrectPanel;

    [Header("Modes Card")]
    public DynamicIconList IconModesList;


    [Header("Interactive")]
    public Outline SelectionOutline;
    public Toggle SelectionToggle;

    private Pathpoint PathpointItem;
    private string tmplExplanation;


    // Start is called before the first frame update
    private void Awake()
    {
        tmplExplanation = StatExplanation.text ?? "";
    }

    void Start()
    {
        SelectionToggle.onValueChanged.AddListener(OnSelectionChangeHanle);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetCardTitle()
    {
        TMP_Text title = null;

        // Find the child with the specific name
        foreach (TMP_Text text in gameObject.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.name == "TitleText")
            {
                if (text.gameObject.activeInHierarchy)
                {
                    title = text;
                    break;
                }
            }
        }

        // If title is found, return its text; otherwise return an empty string or an appropriate message
        return title != null ? title.text : "";
    }


    // Types of cards

    public void FillCardTime(double milliseconds, StatCompute.StatResults prevStats)
    {
        string timeString = formatTime(milliseconds);

        FillTrendCard(timeString);
        RenderTrendContinuous(milliseconds, prevStats, true);
    }

    public void FillCardTimeAggregated(List<RouteWalkEventLog> currentEvents, StatCompute.StatResults prevStats,
        bool isLowerBetter = true)
    {
        double totalDuration = currentEvents.Sum(e => e.DurationEvent);
        string timeString = formatTime(totalDuration);
        string count = currentEvents.Count().ToString();

        //FillCardTime(totalDuration, prevStats);
        FillTrendCard(timeString, count);

        if (prevStats.Count > 0)
        {
            RenderTrendContinuous(totalDuration, prevStats, isLowerBetter);
        }
        else
        {
            VizTrend.FillTrendDiscrete(0, isLowerBetter);
        }
    }

    public void FillCardBadCountNumber(int count, StatCompute.StatResults prevStats)
    {
        FillTrendCard(count.ToString());
        RenderTrendDiscrete(count, prevStats, true);
    }

    public void FillModes(List<string> modeList)
    {
        if (modeList.Count > 0)
        {
            IconModesList.RenderIcons(modeList);
        }
        else
        {
            IconModesList.RenderNoData();
        }
        
    }

    public void FillCardGoodValueNumber(double value, StatCompute.StatResults prevStats, string valueUnit = "", int decimals = 0, string fmtText = null)
    {
        if (fmtText == null)
        {
            fmtText = Math.Round(value, decimals).ToString() + $"<size=50%>{valueUnit}</size>" ;
        }
        FillTrendCard(fmtText);
        RenderTrendContinuous(value, prevStats, false);
    }

    public void FillCardDecision(RouteWalkEventLog decisionEvent)
    {
        CorrectPanel.SetActive(decisionEvent.IsCorrectDecision == true);
        WrongTurnPanel.SetActive(decisionEvent.NavIssue == LocationTools.NavigationIssue.WrongTurn);
        MissedTurnPanel.SetActive(decisionEvent.NavIssue == LocationTools.NavigationIssue.MissedTurn);
        WrongDirection.SetActive(decisionEvent.NavIssue == LocationTools.NavigationIssue.WrongDirection);
        DeviationPanel.SetActive(decisionEvent.NavIssue == LocationTools.NavigationIssue.Deviation);

        RenderCardView(DecisionCard);
    }


    public void FillTrendCard(string formattedValue, string explanation = null)
    {
        StatValue.text = formattedValue;
        StatExplanation.gameObject.SetActive(explanation != null);
        if (explanation != null)
        {
            StatExplanation.gameObject.SetActive(true);
            StatExplanation.text = tmplExplanation.Replace("{0}", explanation);
        }

        RenderCardView(TrendValueCard);
    }

    // Utils

    private void RenderTrendContinuous(double value, StatCompute.StatResults prevStats, bool isLowerBetter)
    {
        // percentage
        if (prevStats.Count > 0 && prevStats.Mean > 0)
        {
            int change = (int)Math.Abs(prevStats.GetValueChange(value) * 100);
            change = change * (value < prevStats.Mean ? -1 : 1);

            VizTrend.FillTrendContinous(change, isLowerBetter);
        }
        else
        {
            VizTrend.FillNoData();
        }
    }

    private void RenderTrendDiscrete(int count, StatCompute.StatResults prevStats, bool isLowerBetter)
    {

        if (prevStats.Count > 0)
        {
            int value = (int)prevStats.Sum * (count < prevStats.Mean ? -1 : 1);

            VizTrend.FillTrendDiscrete(value, isLowerBetter);
        }
        else
        {
            VizTrend.FillNoData();            
        }
    }

    private string formatTime(double milliseconds)
    {
        string timeString;
        if (milliseconds < 60000)
        {
            // Less than a minute, show in seconds
            var seconds = Math.Round(milliseconds / 1000);
            timeString = seconds.ToString() + "<size=50%>Sek</size>";
        }
        else
        {
            // One minute or more, show in minutes and seconds
            var totalSeconds = Math.Round(milliseconds / 1000);
            var minutes = (int)(totalSeconds / 60);
            var seconds = (int)(totalSeconds % 60);
            timeString = minutes.ToString() + "<size=50%>Min</size> " + seconds.ToString() + "<size=50%>Sek</size>";
        }
        return timeString;
    }

    // Event handlers

    private void OnSelectionChangeHanle(bool value)
    {
        var outlineColor = SelectionOutline.effectColor;
        outlineColor.a = value ? 1 : 0;

        SelectionOutline.effectColor = outlineColor;
    }

    private void OnDestroy()
    {
        SelectionToggle.onValueChanged.RemoveListener(OnSelectionChangeHanle);
    }

    private void RenderCardView(GameObject card)
    {
        TrendValueCard.SetActive(TrendValueCard == card);
        DecisionCard.SetActive(DecisionCard == card);
    }

}
