using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseHandler : MonoBehaviour
{
    public bool isOver = false;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enter");
        isOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exit");
        isOver = false;
    }

}
