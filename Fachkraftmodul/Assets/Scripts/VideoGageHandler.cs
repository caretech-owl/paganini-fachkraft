using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGageHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check for mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Test");

            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    Debug.Log(Input.mousePosition.ToString());
                    //Our custom method. 
                    //CurrentClickedGameObject(raycastHit.transform.gameObject);
                }
            }
        }
    }
}
