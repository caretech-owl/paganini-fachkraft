using NinevaStudios.GoogleMaps;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropUIElement : MonoBehaviour
{
    [SerialisedField]
    public Canvas canvas;

    public void DragHandler(BaseEventData data)
    {

        PointerEventData pointerEventData = (PointerEventData)data;

        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerEventData.position,
            canvas.worldCamera,
            out position);
       
        transform.position = canvas.transform.TransformPoint(position);
    }
}
