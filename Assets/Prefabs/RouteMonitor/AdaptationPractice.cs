using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;


public class AdaptationPractice : MonoBehaviour
{

    public GameObject AcceptanceView;
    public GameObject PerformanceView;

    [Header("Performance")]
    public GameObject IconCorrectPerformance;
    public GameObject IconIncorrectPerformance;
    public GameObject IconNAPerformance;

    [Header("Acceptance")]
    public Image AcceptanceBackground;
    public GameObject IconAccepted;
    public GameObject IconNotAccepted;

    public Color PositiveColor;
    public Color NegativeColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderAdaptationPracticed(RouteWalkEventLog adaptLog)
    {
        AcceptanceView.SetActive(adaptLog.AdaptationIntroShown == true);
        if (adaptLog.AdaptationIntroShown == true)
        {
            RenderAcceptance(adaptLog.AdaptationTaskAccepted == true);
        }

        PerformanceView.SetActive(true);
        RenderAcceptance(adaptLog.AdaptationTaskCorrect);
    }

    public void HideView()
    {
        AcceptanceView.SetActive(false);
        PerformanceView.SetActive(false);
    }

    private void RenderAcceptance(bool accepted)
    {
        AcceptanceBackground.color = accepted ? PositiveColor : NegativeColor;
        IconAccepted.SetActive(accepted);
        IconNotAccepted.SetActive(!accepted);
    }

    private void RenderAcceptance(bool? correct)
    {
        IconCorrectPerformance.SetActive(correct == true);
        IconIncorrectPerformance.SetActive(correct == false);
        IconNAPerformance.SetActive(correct == null);
    }

    private void ShowView(GameObject view)
    {
        //TrendContinous.SetActive(TrendContinous == view);
        //TrendDiscrete.SetActive(TrendDiscrete == view);
        //NoData.SetActive(NoData == view);
    }

}
