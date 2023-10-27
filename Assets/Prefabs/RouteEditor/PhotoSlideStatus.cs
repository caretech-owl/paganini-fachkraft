using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;

public class PhotoSlideStatus : MonoBehaviour
{
    public GameObject DotPrefab; // Reference to the dot prefab
    public Color ActiveSlideColor;
    public Color InactiveSlideColor;

    private List<SVGImage> dotList = new();


    private void Start() {
    }    

    public void GenerateDots(int numberOfDots)
    {
        ClearDots();

        for (int i = 0; i < numberOfDots; i++)
        {
            var dot = Instantiate(DotPrefab, transform);
            dot.SetActive(true);
            dotList.Add(dot.GetComponent<SVGImage>());
        }

        //dotList.Reverse();
    }

    public void SetActiveSlide(int index)
    {
        if (index >= 0 && index < dotList.Count)
        {
            dotList.ForEach(p => p.color = InactiveSlideColor);
            dotList[index].color = ActiveSlideColor;
        }
    }

    public void ClearDots()
    {
        if (dotList != null)
        {
            foreach (var dot in dotList)
            {
                Destroy(dot);
            }

            dotList.Clear();
        }
    }

    private void OnDestroy()
    {
        ClearDots();
    }

}
