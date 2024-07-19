using System.Collections.Generic;
using UnityEngine;

public class DynamicIconList : MonoBehaviour
{
    
    private List<GameObject> IconList;

    private void Awake()
    {
        // the icons components are already in the view
        IconList = new List<GameObject>();
        foreach (Transform child in transform)
        {
            IconList.Add(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderIcons(List<string> iconNames)
    {
        foreach(var icon in IconList)
        {
            icon.SetActive(iconNames.Contains(icon.name));
        }

    }

    public void RenderNoData()
    {
        foreach (var icon in IconList)
        {
            icon.SetActive(icon.name == "NoData");
        }

    }


}
