
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static PathpointPIM;

public class AdaptationHistoryItem : MonoBehaviour
{
    private static string DateFormat = "MMM dd";
    public AdaptationPractice ModePractice;
    public StatCard ModeIcon;
    public TMPro.TMP_Text ModeNameText;
    public TMPro.TMP_Text RouteWalkText;


    // SupportModeLabel dictionary to map enum to string labels
    private static readonly Dictionary<SupportMode, string> SupportModeLabel = new Dictionary<SupportMode, string>
    {
        { SupportMode.None, "Nicht zutreffend" },
        { SupportMode.Instruction, "Normalmodus" },
        { SupportMode.Trivia, "Quiz" },
        { SupportMode.Challenge, "Herausforderung" },
        { SupportMode.Mute, "Versteckmodus" }
    };

    private void Awake()
    {
        // Any initialization if needed
    }

    private void Start()
    {
        // FillMode(SelectionSupportMode.ToString());
    }

    // Method to fill the mode practiced
    public void FillModePracticedMode(RouteWalk walk, RouteWalkEventLog adaptLog)
    {
        //if (adaptLog != null)
        //{
        //    var mode = adaptLog.AdaptationSupportMode ?? SupportMode.Instruction;
        //    ModePractice.RenderAdaptationPracticed(adaptLog);
        //    ModeIcon.FillModes(new List<string> { mode.ToString() });            
        //    ModeNameText.text = SupportModeLabel[mode];
        //}
        //else
        //{
        //    // NA
        //    ModeIcon.FillModes(new List<string> { });
        //    ModePractice.HideView();
        //    ModeNameText.text = SupportModeLabel[SupportMode.None];
        //}

        SupportMode mode = SupportMode.Instruction;

        if (adaptLog != null)
        {
            mode = adaptLog.AdaptationSupportMode ?? SupportMode.Instruction;
            ModePractice.RenderAdaptationPracticed(adaptLog);
        }
        else
        {
            ModePractice.HideView();
        }
        
        ModeIcon.FillModes(new List<string> { mode.ToString() });
        ModeNameText.text = SupportModeLabel[mode];

        string fmtDate = DateUtils.ConvertUTCDateToUTCString(walk.StartDateTime, DateFormat, CultureInfo.CurrentCulture);
        RouteWalkText.text = fmtDate;
    }
}