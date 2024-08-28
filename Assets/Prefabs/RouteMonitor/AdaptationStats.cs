using System.Collections.Generic;
using MathNet.Numerics;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Linq.Expressions;
using NetTopologySuite.Triangulate;
using Unity.VisualScripting;
using static StatCompute;
using static PathpointPIM;

public class AdaptationStats : MonoBehaviour
{
    public GameObject SegmentAdapt;
    public GameObject POIAdapt;

    [Header("POI")]
    public AdaptationEnactment POIAdaptation;
    public AdaptationHistory POIHistory;
    public StatCard POIPracticedMode;
    public StatCard POICurrentMode;
    public AdaptationPractice POIPracticeOutcome;
    public ToggleGroup POICardGroup;

    [Header("Segment")]
    public AdaptationEnactment SegAdaptation;
    public AdaptationHistory SegHistory;
    public StatCard SegPracticedMode;
    public StatCard SegCurrentMode;
    public AdaptationPractice SegPracticeOutcome;
    public ToggleGroup SegCardGroup;


    [Header("Adaptation details")]
    public TMPro.TMP_Text SegDetailsCardTitle;
    public TMPro.TMP_Text SegDetailsCardSubtitle;
    public TMPro.TMP_Text POIDetailsCardTitle;
    public TMPro.TMP_Text POIDetailsCardSubtitle;

    private int CurrentIndex;
    private Pathpoint CurrentPOI;
    private RouteWalkItemEvent.RouteWalkEventType CurrentEventType;
    private Way CurrentWay;
    private RouteWalkSharedData WalkSharedData;
    private RouteSharedData SharedData;
    private StatCompute WalkStatCompute;

    private StatsSharedData StatsData;

    // Start is called before the first frame update
    void Awake()
    {
        WalkSharedData = RouteWalkSharedData.Instance;
        SharedData = RouteSharedData.Instance;
        WalkStatCompute = StatCompute.Instance;
        StatsData = StatsSharedData.Instance;

        CurrentIndex = -1;
        CurrentPOI = null;
    }

    void Start()
    {
        POIAdaptation.OnAdaptationValueChanged += POIAdaptation_OnAdaptationValueChanged;
        SegAdaptation.OnAdaptationValueChanged += SegAdaptation_OnAdaptationValueChanged;
    }

    private void SegAdaptation_OnAdaptationValueChanged(object sender, System.EventArgs e)
    {
        LoadNextModeCard(CurrentPOI.CurrentInstructionMode, SegCurrentMode, isPOI: false);
        AppState.MonitoringView.UpdatedModeInTimeline = true;
        AppState.MonitoringView.UpdatedModePOI = CurrentPOI;
    }

    private void POIAdaptation_OnAdaptationValueChanged(object sender, System.EventArgs e)
    {
        LoadNextModeCard(CurrentPOI.CurrentInstructionMode, POICurrentMode, isPOI: true);
        AppState.MonitoringView.UpdatedModeInTimeline = true;
        AppState.MonitoringView.UpdatedModePOI = CurrentPOI;
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    public void OnAdaptPOICardSelected(bool value)
    {
        if (!value) return;

        foreach ( var activeToggle in POICardGroup.ActiveToggles())
        {
            var card = activeToggle.GetComponent<StatCard>();
            Debug.Log(activeToggle.name + " - " + card.GetCardTitle());
            RenderPOIDetails(activeToggle.name, card.GetCardTitle());
        }               
    }

    public void OnAdaptSegmentCardSelected(bool value)
    {
        if (!value) return;

        foreach (var activeToggle in SegCardGroup.ActiveToggles())
        {
            var card = activeToggle.GetComponent<StatCard>();
            Debug.Log(activeToggle.name + " - " + card.GetCardTitle());
            RenderSegmentDetails(activeToggle.name, card.GetCardTitle());
        }
    }

    private void RenderPOIDetails(string name, string title)
    {
        if (CurrentPOI == null) // fired before initialisaiton
            return;

        RenderPOITitle(name);

        if (name == "PracticedModeCard")
        {            
            ShowDetail(POIHistory.gameObject);
            //POIDetailsViz.RenderPicture(CurrentPOI);
        }
        else if (name == "CurrentModeCard")
        {
            ShowDetail(POIAdaptation.gameObject);
            POIAdaptation.LoadAdaptation(CurrentPOI, isPOI: true);
        }
    }

    private void RenderSegmentDetails(string name, string title)
    {
        if (CurrentPOI == null) // fired before initialisaiton
            return;

        RenderSegmentTitle(name);

        if (name == "PracticedModeCard")
        {
            ShowDetail(SegHistory.gameObject);
            //POIDetailsViz.RenderPicture(CurrentPOI);
        }
        else if (name == "CurrentModeCard")
        {
            ShowDetail(SegAdaptation.gameObject);
            SegAdaptation.LoadAdaptation(CurrentPOI, isPOI: false);
        }
    }

    public void RenderSegmentTitle(string name)
    {
        RenderCardTitle(name, SegDetailsCardTitle, SegDetailsCardSubtitle);
    }

    public void RenderPOITitle(string name)
    {
        RenderCardTitle(name, POIDetailsCardTitle, POIDetailsCardSubtitle);
    }

    private void RenderCardTitle(string name, TMPro.TMP_Text title, TMPro.TMP_Text subtitle)
    {
        if (name == "PracticedModeCard")
        {
            title.text = "Historische Trainingsmodi";
            subtitle.text = "Anpassungsaufgabe in vorherigen Sitzungen durchgeführt";
        }
        else if (name == "CurrentModeCard")
        {
            title.text = "Nächster Trainingsmodus";
            subtitle.text = "Anpassungsaufgabe für nächste Sitzung festlegen";
        }
    }


    public void RenderAdaptPOIStats(Pathpoint currentPOI, List<RouteWalkEventLog> poiEvents)
    {
        CurrentPOI = currentPOI;

        ShowPanel(POIAdapt);
        RenderPOITitle(POICurrentMode.name);
        POIAdaptation.LoadAdaptation(currentPOI, isPOI: true);

        // Practiced mode
        RouteWalkEventLog adaptation = GetRelevantAdaptation(poiEvents);

        LoadPracticedModeCard(adaptation, POIPracticedMode, POIPracticeOutcome);

        LoadNextModeCard(currentPOI.CurrentInstructionMode, POICurrentMode, isPOI : true);

        LoadPOIHistory(currentPOI, adaptation);

    }


    public void RenderAdaptSegmentStats(Pathpoint startPOI, Pathpoint destPOI, List<RouteWalkEventLog> segEvents)
    {
        CurrentPOI = destPOI;

        RenderSegmentTitle(SegCurrentMode.name);
        ShowPanel(SegmentAdapt);
        SegAdaptation.LoadAdaptation(destPOI, isPOI: false);

        RouteWalkEventLog adaptation = GetRelevantAdaptation(segEvents);

        LoadPracticedModeCard(adaptation, SegPracticedMode, SegPracticeOutcome);

        LoadNextModeCard(destPOI.CurrentInstructionMode, SegCurrentMode, isPOI: false);

        LoadSegHistory(startPOI, adaptation);
    }

    private void LoadPOIHistory(Pathpoint currentPOI, RouteWalkEventLog adaptation)
    {
        var stats = WalkStatCompute.CalculatePOIAdaptHistory(currentPOI.Id, adaptation);
        POIHistory.LoadHistory(stats);
    }

    private void LoadSegHistory(Pathpoint currentPOI, RouteWalkEventLog adaptation)
    {
        var stats = WalkStatCompute.CalculateSegAdaptHistory(currentPOI.Id, adaptation);
        SegHistory.LoadHistory(stats);
    }

    private void LoadPracticedModeCard(RouteWalkEventLog adaptation, StatCard modeCard, AdaptationPractice adaptationPractice)
    {
        if (adaptation != null)
        {
            SupportMode mode = adaptation.AdaptationSupportMode ?? SupportMode.Instruction;
            modeCard.FillModes(new List<string> { mode.ToString() });
            adaptationPractice.RenderAdaptationPracticed(adaptation);
        }
        else
        {
            modeCard.FillModes(new List<string> { });
            adaptationPractice.HideView();
        }
    }

    private void LoadNextModeCard(PathpointPIM currentInstructionMode, StatCard modeCard, bool isPOI)
    {
        // Current / Next mode
        SupportMode nextMode = SupportMode.Instruction;
        if (currentInstructionMode != null)
        {
            nextMode = isPOI? currentInstructionMode.AtPOIMode : currentInstructionMode.ToPOIMode;
        }
        modeCard.FillModes(new List<string> { nextMode.ToString() });
    }

    private RouteWalkEventLog GetRelevantAdaptation(List<RouteWalkEventLog> logEvents)
    {
        if (logEvents == null) return null;

        var adapList = logEvents.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Adaptation);
        RouteWalkEventLog adaptation = null;
        if (adapList.Count > 0)
        {
            adaptation = adapList.OrderBy(a => a.StartTimestamp).ToList().First();
        }
        else if (adapList.Count == 1)
        {
            adaptation = adapList.First();
        }

        return adaptation;
    }



    public void ShowPanel(GameObject view)
    {
        POIAdapt.SetActive(POIAdapt == view);
        SegmentAdapt.SetActive(SegmentAdapt == view);
    }

    public void ShowDetail(GameObject view)
    {
        if (POIAdapt.activeSelf)
        {
            POIHistory.gameObject.SetActive(POIHistory.gameObject == view);
            POIAdaptation.gameObject.SetActive(POIAdaptation.gameObject == view);
        }
        else
        {
            SegHistory.gameObject.SetActive(SegHistory.gameObject == view);
            SegAdaptation.gameObject.SetActive(SegAdaptation.gameObject == view);
        }
        
        
    }

    private void OnDestroy()
    {
        POIAdaptation.OnAdaptationValueChanged -= POIAdaptation_OnAdaptationValueChanged;
        SegAdaptation.OnAdaptationValueChanged -= SegAdaptation_OnAdaptationValueChanged;
    }

}
