
using UnityEngine;
using UnityEngine.UI;
using static PathpointPIM;

public class AdaptationHistoryItem : MonoBehaviour
{
    // to be assigned on the editor
    public SupportMode SelectionSupportMode;
    public Button SelectionButton;
    public TMPro.TMP_Text ModeName;

    private Toggle SelectionToggle;
    private Outline SelectionOutline;
    

    private void Awake()
    {
        SelectionToggle = gameObject.GetComponent<Toggle>();
        SelectionOutline = gameObject.GetComponent<Outline>();

        // default
        OnValueChanged(SelectionToggle.isOn);

        SelectionToggle.onValueChanged.AddListener(OnValueChanged);

    }
    private void Start()
    {
        //FillMode(SelectionSupportMode.ToString());
    }

    // public 
    public SupportMode GetSupportMode()
    {
        return SelectionSupportMode;
    }

    public void RenderAsSelected(bool selected)
    {
        // outline when active
        var outlineColor = SelectionOutline.effectColor;
        outlineColor.a = selected ? 1 : 0;

        SelectionOutline.effectColor = outlineColor;
        SelectionButton.gameObject.SetActive(!selected);
    }

    public void FillMode(string name)
    {
        ModeName.text = name;
    }

    private void OnValueChanged(bool selected)
    {

        RenderAsSelected(selected);
        // button selection when not active
        //SelectionButton.gameObject.SetActive(!selected);

    }

}
