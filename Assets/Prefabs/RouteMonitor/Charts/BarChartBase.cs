using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using XCharts.Runtime;
using static StatCompute;

[DisallowMultipleComponent]
public class BarChartBase : MonoBehaviour
{
    public GameObject BarChartObj;
    public GameObject RingChartObj;

    private BarChart chart;
    private RingChart ringChart;
    private static string DateFormat = "MMM dd";


    private List<(RouteWalkEventLog log, double value)> _stats;

    private void OnEnable()
    {

    }


    public void RenderDuration(string label, List<(RouteWalk walk, double? value)> stats, string units = null)
    {
        string fmt = units != null ? $" ({units})" : "";
        AddSimpleBar(label + fmt);
        LoadValueData(stats);
    }

    public void RenderAggregatedStats(string label, List<(RouteWalk walk, StatResults)> stats, string units = null)
    {
        string fmt = units != null ? $" ({units})" : "";
        AddSimpleBar(label + fmt);
        LoadAggregatedData(stats);
    }

    public void RenderPercentage(string label, double percentage)
    {
        int value = Mathf.RoundToInt((float)percentage * 100);
        AddRingChart(label, value);
    }


    private void AddSimpleBar(string axisLabel)
    {
        ShowView(BarChartObj);
        chart = BarChartObj.GetComponent<BarChart>();
        if (chart == null)
        {
            chart = BarChartObj.AddComponent<BarChart>();
            chart.Init();
        }
        //chart.EnsureChartComponent<Title>().text = title;
        //chart.EnsureChartComponent<Title>().subText = subtitle;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;
        yAxis.ClearData();

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.minCategorySpacing = 1;
        xAxis.ClearData();

        //chart.RemoveData();

        var serie = chart.GetSerie<Bar>();
        if (serie == null)
        {
            serie = chart.AddSerie<Bar>(axisLabel);           
        }
        serie.ClearData();
        serie.serieName = axisLabel;
        ApplyCommonStyle(chart);
    }

    private void ClearSerie(Bar bar)
    {        
        int count = bar.dataCount;
        for(int i=0; i< count; i++)
        {
            bar.RemoveData(i);
        }
    }

    private void AddRingChart(string axisLabel, int value)
    {
        ShowView(RingChartObj);
        ringChart = RingChartObj.GetComponent<RingChart>();
        if (ringChart == null)
        {
            ringChart = RingChartObj.AddComponent<RingChart>();
            ringChart.Init();
        }
        //ringChart.EnsureChartComponent<Title>().text = title;
        //chart.EnsureChartComponent<Title>().subText = subtitle;

        //ringChart.RemoveData();

        var serie = ringChart.GetSerie<Ring>();
        if (serie == null)
        {
            serie = ringChart.AddSerie<Ring>(axisLabel);
            serie.roundCap = true;
            serie.gap = 10;
            serie.radius = new float[] { 0.3f, 0.35f };

            var label = serie.EnsureComponent<LabelStyle>();
            label.show = true;
            label.position = LabelStyle.Position.Center;
            label.formatter = "{d:f0}%";
            label.textStyle.color = Color.black;
            label.textStyle.fontSize = 48;

        }
        serie.ClearData();


        //var titleStyle = serie.EnsureComponent<TitleStyle>();
        //titleStyle.show = false;
        //titleStyle.offset = new Vector2(0, 30);

        //var background = ringChart.EnsureChartComponent<Background>();
        //background.show = false;

        //var title = ringChart.EnsureChartComponent<Title>();
        //title.text = "";
        //title.show = false;

        //ringChart.theme.transparentBackground = true;

        ApplyCommonStyle(ringChart);

        var max = 100;
        ringChart.AddData(serie.index, value, max, axisLabel);

    }

    private void ApplyCommonStyle(BaseChart baseChart)
    {
        var background = baseChart.EnsureChartComponent<Background>();
        background.show = false;

        var title = baseChart.EnsureChartComponent<Title>();
        title.text = "";
        title.show = false;

        baseChart.theme.transparentBackground = true;
    }

    private void LoadValueData(List<(RouteWalk walk, double? value)> stats)
    {
        foreach (var logStat in stats)
        {
            string fmtDate = DateUtils.ConvertUTCToLocalString(logStat.walk.StartDateTime, DateFormat, CultureInfo.CurrentCulture);
            chart.AddXAxisData(fmtDate);
            chart.AddData(0, logStat.value == null? double.NaN : (double)Math.Round((float)logStat.value,2));
        }
    }

    private void LoadAggregatedData(List<(RouteWalk walk, StatResults value)> stats)
    {
        foreach (var logStat in stats)
        {
            string fmtDate = DateUtils.ConvertUTCDateToUTCString(logStat.walk.StartDateTime, DateFormat, CultureInfo.CurrentCulture);
            chart.AddXAxisData(fmtDate);
            chart.AddData(0, logStat.value == null ? double.NaN : (double) Math.Round((float)logStat.value.Sum,2));
        }
    }


    public void ShowView(GameObject view)
    {
        BarChartObj.SetActive(BarChartObj == view);
        RingChartObj.SetActive(RingChartObj == view);
    }


}
