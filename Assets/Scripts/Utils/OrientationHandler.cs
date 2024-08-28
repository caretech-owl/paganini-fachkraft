using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationHandler : MonoBehaviour
{
    public ScreenOrientation orientation;

    void Awake()
    {
        Screen.orientation = orientation;
    }

#if UNITY_STANDALONE_OSX
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        AdjustAspectRatio();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustAspectRatio();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private float targetAspectRatio = 16.0f / 10.0f;

    void AdjustAspectRatio()
    {
        int screenHeight = Screen.height;
        int screenWidth = (int)(screenHeight * targetAspectRatio);

        if (screenWidth > Screen.width)
        {
            screenWidth = Screen.width;
            screenHeight = (int)(screenWidth / targetAspectRatio);
        }

        Screen.SetResolution(screenWidth, screenHeight, false);
    }
#endif
}
