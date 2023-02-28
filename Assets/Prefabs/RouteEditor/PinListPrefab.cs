using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PinItemPrefab;

public class PinListPrefab : MonoBehaviour
{

    public GameObject ScrollView;

    public GameObject Content;

    public GameObject ItemPrefab;

    public GameObject DetailsPrefab;

    public GameObject BlankState;

    public PathpointItemEvent OnItemSelected;

    // Start is called before the first frame update
    void Start()
    {
        //Clearlist();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clearlist()
    {
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

    public void AddItem(Pathpoint p)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        PinItemPrefab item = neu.GetComponent<PinItemPrefab>();
        item.OnSelected = OnItemSelected;
        item.FillPathpoint(p);

    }

    public void LoadDetails(Pathpoint p)
    {
        var neu = Instantiate(DetailsPrefab, this.gameObject.transform.parent);
        PinDetailsPrefab item = neu.GetComponent<PinDetailsPrefab>();
        item.LoadPin(p);
        item.OnCloseDetails.AddListener(CloseDetailsCallback);
    }

    private void CloseDetailsCallback(Pathpoint p)
    {
        this.gameObject.SetActive(true);

    }

}
