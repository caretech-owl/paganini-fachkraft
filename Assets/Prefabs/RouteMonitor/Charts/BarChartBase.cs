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
