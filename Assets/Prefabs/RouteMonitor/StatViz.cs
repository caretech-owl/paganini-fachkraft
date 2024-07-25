using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using UnityEngine;
using UnityEngine.UI;
using static StatCompute;

public class StatViz : MonoBehaviour
{
    [Header("Header")]
    //public LandmarkIcon PinIcon;
    public TMPro.TMP_Text CardTitle;
    public TMPro.TMP_Text CardSubtitle;

    [Header("Content")]
    public GameObject PictureContent;
    public BarChartBase SimpleBarChart;
    //public GameObject DecisionCard;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCardTitle(string title)
    {
        CardTitle.text = title;
    }

    public void RenderPicture(Pathpoint pathpoint)
    {
        
        var image = PictureContent.GetComponentInChildren<RawImage>();

        if (pathpoint.Photos != null && pathpoint.Photos.Count > 0)
        {
            var data = pathpoint.Photos[0].Data;
            LoadPicture(image, data.Photo);
        }

        ShowContent(PictureContent);
    }

    public void RenderChart(List<(RouteWalk walk, double? value)> stats)
    {
        SimpleBarChart.RenderDuration(stats);
        ShowContent(SimpleBarChart.gameObject);
    }

    public void RenderChartAggregated(List<(RouteWalk walk, StatResults value)> stats)
    {
        SimpleBarChart.RenderAggregatedStats(stats);
        ShowContent(SimpleBarChart.gameObject);
    }

    public void RenderChartPercentage(string label, double percentage)
    {
        SimpleBarChart.RenderPercentage(label, percentage);
        ShowContent(SimpleBarChart.gameObject);
    }

    private void LoadPicture(RawImage image, byte[] imageBytes)
    {
        if (image.texture != null)
        {
            DestroyImmediate(image.texture, true);
        }

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        image.texture = texture;
        image.gameObject.SetActive(true);

        
    }

    private void ShowContent(GameObject view)
    {
        if (PictureContent != null) PictureContent.SetActive(PictureContent == view);
        if (SimpleBarChart != null) SimpleBarChart.gameObject.SetActive(SimpleBarChart.gameObject == view);
    }

}
