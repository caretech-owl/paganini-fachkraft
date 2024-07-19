using System;
using UnityEngine;
using UnityEngine.UI;


public class MaskedIcon : MonoBehaviour
{

    private Image fillMask;

    // Start is called before the first frame update
    void Start()
    {
        fillMask = gameObject.GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFillColor(Color color)
    {
        fillMask.color = color;
    }

    public string GetIconName()
    {
        return gameObject.name;
    }

    public void SelectIcon(bool selected)
    {
        gameObject.SetActive(selected);
    }



}
