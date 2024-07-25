using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using XCharts.Runtime;
using static StatCompute;

[DisallowMultipleComponent]
public class BarChartBase : MonoBehaviour
{
    private BarChart chart;
    private RingChart ringChart;
    private static string DateFormat = "MMM dd";


    private List<(RouteWalkEventLog log, double value)> _stats;

    private void OnEnable()
    {

    }


    public void RenderDuration(List<(RouteWalk walk, double? value)> stats)
    {
        AddSimpleBar("Duration","Duration");
        LoadValueData(stats);

    }

    public void RenderAggregatedStats(List<(RouteWalk walk, StatResults)> stats)
    {
        AddSimpleBar("Aggregated", "Aggregated");
        LoadAggregatedData(stats);
    }

    public void RenderPercentage(string label, double percentage)
    {
        int value = Mathf.RoundToInt((float)percentage * 100);
        AddRingChart(label, value);
    }


    private void AddSimpleBar(string title, string axisLabel)
    {
        chart = gameObject.GetComponent<BarChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<BarChart>();
            chart.Init();
        }
        chart.EnsureChartComponent<Title>().text = title;
        //chart.EnsureChartComponent<Title>().subText = subtitle;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.minCategorySpacing = 1;

        chart.RemoveData();
        var serie = chart.AddSerie<Bar>(axisLabel);
               
    }

    private void AddRingChart(string axisLabel, int value)
    {
        ringChart = gameObject.GetComponent<RingChart>();
        if (chart == null)
        {
            ringChart = gameObject.AddComponent<RingChart>();
            ringChart.Init();
        }
        //ringChart.EnsureChartComponent<Title>().text = title;
        //chart.EnsureChartComponent<Title>().subText = subtitle;

        ringChart.RemoveData();
        var serie = ringChart.AddSerie<Ring>();
        serie.roundCap = true;
        serie.gap = 10;
        serie.radius = new float[] { 0.3f, 0.35f };

        var label = serie.EnsureComponent<LabelStyle>();
        label.show = true;
        label.position = LabelStyle.Position.Center;
        label.formatter = "{d:f0}%";
        label.textStyle.autoColor = true;
        label.textStyle.fontSize = 28;

        //var titleStyle = serie.EnsureComponent<TitleStyle>();
        //titleStyle.show = false;
        //titleStyle.offset = new Vector2(0, 30);

        var background = ringChart.EnsureChartComponent<Background>();
        background.show = false;

        var title = ringChart.EnsureChartComponent<Title>();
        title.text = "";
        title.show = false;

        ringChart.theme.transparentBackground = true;

        var max = 100;
        ringChart.AddData(serie.index, value, max, axisLabel);

    }


    private void LoadValueData(List<(RouteWalk walk, double? value)> stats)
    {
        foreach (var logStat in stats)
        {
            string fmtDate = DateUtils.ConvertUTCToLocalString(logStat.walk.StartDateTime, DateFormat, CultureInfo.CurrentCulture);
            chart.AddXAxisData(fmtDate);
            chart.AddData(0, logStat.value == null? double.NaN : (double)logStat.value);
        }
    }

    private void LoadAggregatedData(List<(RouteWalk walk, StatResults value)> stats)
    {
        foreach (var logStat in stats)
        {
            string fmtDate = DateUtils.ConvertUTCDateToUTCString(logStat.walk.StartDateTime, DateFormat, CultureInfo.CurrentCulture);
            chart.AddXAxisData(fmtDate);
            chart.AddData(0, logStat.value == null ? double.NaN : (double)logStat.value.Sum);
        }
    }


}
