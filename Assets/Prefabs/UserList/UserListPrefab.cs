using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserListPrefab : MonoBehaviour
{

    public GameObject ScrollView;

    public GameObject Content;

    public GameObject ItemPrefab;

    public GameObject BlankState;

    public UserItemEvent OnItemSelected;

    // Start is called before the first frame update
    void Start()
    {
        Clearlist();
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

    public void AddItem(User u)
    {

        var neu = Instantiate(ItemPrefab, Content.transform);

        UserItemPrefab item = neu.GetComponent<UserItemPrefab>();
        item.OnSelected = OnItemSelected;
        item.FillUser(u);

    }


}
