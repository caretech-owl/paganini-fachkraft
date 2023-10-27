using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkIcon : MonoBehaviour
{
    public Color BackgroundColor = Color.white;

    public enum LandmarkType {
        Placeholder = -1,
        PinLandmark = -2,
        PinReassurance = -3,
        Train = 1,
        Coffee = 2,
        Work = 3,
        Home = 4,
        Bus = 5,
        Park = 6,
        Shopping = 7
    }
    public LandmarkType SelectedLandmarkType = LandmarkType.Placeholder;
    private LandmarkType activeLandmarkType;

    // Start is called before the first frame update
    void Start()
    {
        ApplyColorToBackground();
        displayLandmarkType(activeLandmarkType);
    }

    // Update is called once per frame
    void Update()
    {
        // If a new selected landmark has been set, we activate it
        if (activeLandmarkType != SelectedLandmarkType)
        {
            displayLandmarkType(SelectedLandmarkType);
        }

    }

    // activates the selected landmark type
    void displayLandmarkType(LandmarkType selected)
    {
        for (int i=0; i< gameObject.transform.childCount; i++)
        {
            Transform ch = gameObject.transform.GetChild(i);
            ch.gameObject.SetActive(ch.name == selected.ToString());
        }

        activeLandmarkType = selected;
    }

    public void SetSelectedLandmark(int typeCode)
    {
        SelectedLandmarkType = (LandmarkType)typeCode;
    }

    public Sprite GetIcon(LandmarkType iconType)
    {
        // Convert the LandmarkType to a string to find the SVGImage GameObject
        string name = iconType.ToString();

        // Find the GameObject with the SVGImage component by name
        Transform svgImageObject = transform.Find(name);

        // Check if the GameObject was found
        if (svgImageObject != null)
        {
            // Get the SVGImage component from the GameObject
            SVGImage svgImage = svgImageObject.GetComponent<SVGImage>();

            // Check if the SVGImage component exists
            if (svgImage != null)
            {
                // Get the sprite from the SVGImage component
                Sprite iconSprite = svgImage.sprite;

                // Check if the sprite exists
                if (iconSprite != null)
                {
                    // Return the sprite
                    return iconSprite;
                }
                else
                {
                    Debug.LogError("Sprite not found on SVGImage component.");
                }
            }
            else
            {
                Debug.LogError("SVGImage component not found on GameObject: " + name);
            }
        }
        else
        {
            Debug.LogError("GameObject not found: " + name);
        }

        // Return null if any errors occurred
        return null;
    }


    private void ApplyColorToBackground()
    {
        Image[] backgroundComponents = gameObject.GetComponentsInChildren<Image>(true);

        foreach (var component in backgroundComponents)
        {
            if (component.name == "Background")
            {
                component.color = BackgroundColor;                
            }
        }
    }

}
