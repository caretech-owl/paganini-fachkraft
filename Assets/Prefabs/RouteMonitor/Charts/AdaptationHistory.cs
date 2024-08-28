
using UnityEngine;
using UnityEngine.UI;
using static StatCompute;

public class AdaptationHistory : MonoBehaviour
{
    [Header("UI states")]
    public GameObject DataState;
    public GameObject BlankState;

    [Header("Data UI")]
    public GameObject Content;
    public GameObject ItemPrefab;

    

    private void Awake()
    {


    }

    private void Start()
    {
        //FillMode(SelectionSupportMode.ToString());
    }

    public void LoadHistory(AdaptStats stats)
    {
        Clearlist();

        if (stats.InstanceEvents.Count == 0)
        {
            ShowView(BlankState);
            return;
        }

        for (int i= stats.InstanceEvents.Count -1; i >=0; i--)
        {
            (var walk, var adapt) = stats.InstanceEvents[i];
            AddItem(walk, adapt);
        }

        //foreach ( (var walk, var adapt) in stats.InstanceEvents)
        //{
        //    AddItem(walk, adapt);
        //}

        ShowView(DataState);
    }

    public void Clearlist()
    {
        if (Content == null) return;

        Transform content = Content.GetComponent<Transform>();

        // Remove all children
        for (int i = 0; i < content.childCount; i++)
        {
            // Get the child game object
            GameObject child = content.GetChild(i).gameObject;

            // Destroy the child game object
            Destroy(child);
        }
    }


    public void AddItem(RouteWalk routeWalk, RouteWalkEventLog adaptLog)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        AdaptationHistoryItem item = neu.GetComponent<AdaptationHistoryItem>();
        item.FillModePracticedMode(routeWalk, adaptLog);

    }

    public void ShowView(GameObject view)
    {
        DataState.SetActive(DataState == view);
        BlankState.SetActive(BlankState == view); 
    }


}
