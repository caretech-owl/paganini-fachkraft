using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhotoListPrefab : MonoBehaviour
{

    public GameObject Content;

    public GameObject ItemPrefab;

    public GameObject BlankState;

    //public PathpointItemEvent OnItemSelected;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void AddItem(PathpointPhoto p)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        var item = neu.GetComponent<PhotoElementPrefab>();
        //item.OnSelected = OnItemSelected;
        item.FillPhoto(p);

    }


}
