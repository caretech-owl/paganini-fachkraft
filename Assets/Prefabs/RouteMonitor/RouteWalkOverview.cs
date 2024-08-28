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
        EnsureSetupComponents();
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnsureSetupComponents()
    {
        if (SharedData != null) return;

        SharedData = RouteSharedData.Instance;
        WalkSharedData = RouteWalkSharedData.Instance;
        WalkStatCompute = StatCompute.Instance;
    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/


    public void LoadView()
    {
        EnsureSetupComponents();

        double progressScore = WalkStatCompute.CalculatePKITrainingProgress();
        TrainingProgressChart.RenderChartPercentage("Trainingsfortschritt", progressScore); // percentage

        double performanceScore = WalkStatCompute.CalculatePKITrainingPerformance();
        TrainingPerformanceChart.RenderChartPercentage("Gelöste Aufgaben", performanceScore);

        double walkCompletness = WalkStatCompute.CalculatePKITrainingCompleteness();
        WalkCompletenessChart.RenderChartPercentage("Zurückgelegte Strecke", walkCompletness);

        double walkPerformance = WalkStatCompute.CalculatePKITrainingAccuracy();
        WalkPerformanceChart.RenderChartPercentage("Navigationsgenauigkeit", walkPerformance);

    }

  

}
