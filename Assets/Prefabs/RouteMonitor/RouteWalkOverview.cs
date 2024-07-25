using UnityEngine;

public class RouteWalkOverview : MonoBehaviour
{

    [Header("UI Elements")]
    public GameObject DataView;
    public GameObject NoDataView;

    public StatViz TrainingProgressChart;
    public StatViz TrainingPerformanceChart;
    public StatViz WalkCompletenessChart;
    public StatViz WalkPerformanceChart;

    private RouteSharedData SharedData;
    private RouteWalkSharedData WalkSharedData;
    private StatCompute WalkStatCompute;

    // Start is called before the first frame update
    void Awake()
    {
        SharedData = RouteSharedData.Instance;
        WalkSharedData = RouteWalkSharedData.Instance;
        WalkStatCompute = StatCompute.Instance;
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/


    public void LoadView()
    {
        double score = WalkStatCompute.CalculatePKITrainingProgress();

        TrainingProgressChart.RenderChartPercentage("Autonomy goal", score); // percentage

    }

  

}
