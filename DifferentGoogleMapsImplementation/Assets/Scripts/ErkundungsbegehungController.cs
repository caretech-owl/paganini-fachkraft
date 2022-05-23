using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using NinevaStudios.GoogleMaps;
using TMPro;

public class ErkundungsbegehungController : MonoBehaviour
{

    public VideoPlayer VideoManager;

    //public OnlineMapsMarkerManager MapsMarkerManager;

    private static Texture2D StaticMarkerIcon;

    public Texture2D DefaultMarkerIcon;

    public Texture2D ActiveMarkerIcon;

    public Texture2D LeaveMarkerIcon;

    public Texture2D DefaultMarker50PercentIcon;

    public Texture2D POIMarkerIcon;

    public Texture2D POILandmarkMarkerIcon;

    public Texture2D POIReassuranceMarkerIcon;

    public Texture2D SimpleViewDefaultMarkerIcon;

    public Texture2D SimpleViewActiveMarkerIcon;

    public Slider SliderVideoGage;

    public GameObject PlayIcon;

    public GameObject PauseIcon;

    // Info

    public GameObject ButtonModifyPicture;

    public GameObject ButtonModifyInfo;

    public GameObject ButtonMovePositionOfPOI;

    public GameObject LabelPOIInfo;

    public GameObject TextPOIInfo;

    public GameObject POIInfo;

    public GameObject POIMove;

    public GameObject POIImages;

    public TextMeshProUGUI POIInfoText;

    public HorizontalChooseImage ImageSlider;

    public GameObject PhotoViewer;

    public TMP_InputField InputPOIInfoTitle;

    public TMP_InputField InputPOIInfoDescription;

    // Image
    public GameObject ImageLinePlaceholder;

    public GameObject ImageItemPlaceholder;

    public Color32 ColorHighlighted;

    public Slider SliderReassurance;

    // PRIVATES

    private List<TimeMarkerObject> TimeMarkerObjects = new List<TimeMarkerObject>()
    {
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90259075164795, (float)52.2963333129883), Timestamp = 1633424732},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90258407592773, (float)52.2963218688965), Timestamp = 1633424733},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9025936126709, (float)52.2963066101074), Timestamp = 1633424735},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90259552001953, (float)52.2962951660156), Timestamp = 1633424736},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90261268615723, (float)52.2962913513184), Timestamp = 1633424737},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90263271331787, (float)52.2962875366211), Timestamp = 1633424738},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90265941619873, (float)52.2962799072266), Timestamp = 1633424739},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90268039703369, (float)52.2962760925293), Timestamp = 1633424740},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90270233154297, (float)52.2962684631348), Timestamp = 1633424741},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90272235870361, (float)52.2962646484375), Timestamp = 1633424742},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90274143218994, (float)52.296257019043 ), Timestamp = 1633424743},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90275764465332, (float)52.2962493896484), Timestamp = 1633424744},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90277576446533, (float)52.2962455749512), Timestamp = 1633424745},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279197692871, (float)52.2962417602539), Timestamp = 1633424746},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280818939209, (float)52.2962379455566), Timestamp = 1633424748},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282821655273, (float)52.2962455749512), Timestamp = 1633424750},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90284824371338, (float)52.2962532043457), Timestamp = 1633424751},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90287113189697, (float)52.2962646484375), Timestamp = 1633424752},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028959274292 , (float)52.2962684631348), Timestamp = 1633424753},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291786193848, (float)52.296272277832 ), Timestamp = 1633424754},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90293312072754, (float)52.296272277832 ), Timestamp = 1633424755},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90293025970459, (float)52.2962646484375), Timestamp = 1633424759},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90292453765869, (float)52.2962532043457), Timestamp = 1633424760},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291881561279, (float)52.2962455749512), Timestamp = 1633424761},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291118621826, (float)52.2962341308594), Timestamp = 1633424762},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90290927886963, (float)52.2962226867676), Timestamp = 1633424764},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291023254395, (float)52.2962074279785), Timestamp = 1633424766},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291023254395, (float)52.2961921691895), Timestamp = 1633424768},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90290355682373, (float)52.2961769104004), Timestamp = 1633424770},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90289783477783, (float)52.2961654663086), Timestamp = 1633424771},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028902053833 , (float)52.2961578369141), Timestamp = 1633424772},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90288639068604, (float)52.2961463928223), Timestamp = 1633424773},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028787612915 , (float)52.2961387634277), Timestamp = 1633424774},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90287017822266, (float)52.2961273193359), Timestamp = 1633424775},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90286064147949, (float)52.2961196899414), Timestamp = 1633424776},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90285205841064, (float)52.2961082458496), Timestamp = 1633424777},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90284538269043, (float)52.2960968017578), Timestamp = 1633424778},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90283870697022, (float)52.296085357666 ), Timestamp = 1633424779},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90283393859863, (float)52.2960739135742), Timestamp = 1633424780},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282821655273, (float)52.2960624694824), Timestamp = 1633424781},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282249450684, (float)52.2960510253906), Timestamp = 1633424782},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90281677246094, (float)52.2960395812988), Timestamp = 1633424783},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90281200408936, (float)52.296028137207 ), Timestamp = 1633424784},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280723571777, (float)52.2960166931152), Timestamp = 1633424785},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280151367188, (float)52.2960052490234), Timestamp = 1633424786},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279960632324, (float)52.2959938049316), Timestamp = 1633424787},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279865264893, (float)52.2959823608398), Timestamp = 1633424788},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279865264893, (float)52.2959709167481), Timestamp = 1633424789},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279674530029, (float)52.295955657959 ), Timestamp = 1633424790},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279197692871, (float)52.2959442138672), Timestamp = 1633424791},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278816223145, (float)52.2959327697754), Timestamp = 1633424792},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278339385986, (float)52.2959175109863), Timestamp = 1633424794},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278244018555, (float)52.2959060668945), Timestamp = 1633424796},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278244018555, (float)52.2958908081055), Timestamp = 1633424798},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278339385986, (float)52.2958793640137), Timestamp = 1633424799},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90278816223145, (float)52.2958717346191), Timestamp = 1633424801},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280055999756, (float)52.2958793640137), Timestamp = 1633424804},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279960632324, (float)52.2958946228027), Timestamp = 1633424805},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90279960632324, (float)52.2959060668945), Timestamp = 1633424806},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280246734619, (float)52.2959175109863), Timestamp = 1633424807},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280532836914, (float)52.2959327697754), Timestamp = 1633424808},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90280914306641, (float)52.2959442138672), Timestamp = 1633424809},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90281200408936, (float)52.295955657959 ), Timestamp = 1633424810},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90281486511231, (float)52.2959709167481), Timestamp = 1633424811},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90281772613525, (float)52.2959823608398), Timestamp = 1633424812},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282154083252, (float)52.2959938049316), Timestamp = 1633424813},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282344818115, (float)52.2960090637207), Timestamp = 1633424814},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282440185547, (float)52.2960205078125), Timestamp = 1633424815},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028263092041 , (float)52.2960319519043), Timestamp = 1633424816},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90282917022705, (float)52.2960472106934), Timestamp = 1633424817},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90283203125   , (float)52.2960586547852), Timestamp = 1633424818},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90283679962158, (float)52.2960739135742), Timestamp = 1633424819},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90284252166748, (float)52.296085357666 ), Timestamp = 1633424820},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028491973877 , (float)52.2961006164551), Timestamp = 1633424821},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90285682678223, (float)52.2961120605469), Timestamp = 1633424822},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90286350250244, (float)52.2961273193359), Timestamp = 1633424823},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90286827087402, (float)52.2961387634277), Timestamp = 1633424824},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90287399291992, (float)52.2961502075195), Timestamp = 1633424825},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90287971496582, (float)52.2961616516113), Timestamp = 1633424826},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028844833374 , (float)52.2961730957031), Timestamp = 1633424827},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90288734436035, (float)52.2961845397949), Timestamp = 1633424828},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90289115905762, (float)52.2961959838867), Timestamp = 1633424829},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9028959274292 , (float)52.2962074279785), Timestamp = 1633424830},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.9029016494751 , (float)52.2962188720703), Timestamp = 1633424831},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90290832519531, (float)52.2962303161621), Timestamp = 1633424832},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90291309356689, (float)52.2962417602539), Timestamp = 1633424833},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90292263031006, (float)52.296257019043 ), Timestamp = 1633424835},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90292835235596, (float)52.2962684631348), Timestamp = 1633424836},
        new TimeMarkerObject(){LatLng = new Vector2((float)8.90293312072754, (float)52.2962760925293), Timestamp = 1633424837},

    };

    private float StepSize = 0;

    private int Duration = 0;

    private int FirstTimeMarker = 0;

    private DateTime StartTimeStamp;

    private bool IsVideoPaused = false;

    private TimeSpan PauseTimeSpan;

    private TimeSpan OffsetTimeSpan;

    private bool isMouseDown = false;

    private float currentTimeStamp;

    private int SliderVideoSecondsWhenPausePressed = 0;

    private TimeMarkerObject SelectedTimeMarkerObject;

    private GoogleMapsView Map;

    private Marker CurrentTouchedMarker;

    private Marker ActualMarker;

    private Marker LastMarker;

    private bool IsSimpleView = false;

    // Start is called before the first frame update
    void Start()
    {
        SelectedTimeMarkerObject = new TimeMarkerObject();
        StaticMarkerIcon = DefaultMarkerIcon;

        // Demo Marker
        TimeMarkerObjects[15].IsPOI = true;
        TimeMarkerObjects[15].Title = "Dies ist ein Haus";
        TimeMarkerObjects[15].Description = "Beschreibung für Marker";
        TimeMarkerObjects[15].ImagesForPOI.Add("1.png");
        TimeMarkerObjects[15].ImagesForPOI.Add("2.png");
        TimeMarkerObjects[15].ImagesForPOI.Add("3.png");
        TimeMarkerObjects[15].ImagesForPOI.Add("4.png");

        // init map
        int moveMapForPhone = 0;
        var cameraPosition = new CameraPosition(
        new LatLng(TimeMarkerObjects[30].LatLng.y, TimeMarkerObjects[30].LatLng.x), 19, 0, 0);
        var options = new GoogleMapsOptions()
            .Camera(cameraPosition);

        Map = new GoogleMapsView(options);
        Map.Show(new Rect(830 + moveMapForPhone, 120, 1050 + moveMapForPhone, 750), OnMapReady);

        FirstTimeMarker = (int)TimeMarkerObjects[0].Timestamp;
        Debug.Log("FirstTimeMarker:" + FirstTimeMarker);

        Duration = (int)(TimeMarkerObjects[TimeMarkerObjects.Count - 1].Timestamp - TimeMarkerObjects[0].Timestamp);
        Debug.Log("Duration: " + Duration);

        StepSize = 1 / Duration;
        Debug.Log("StepSize: " + StepSize);

        SliderVideoGage.maxValue = Duration;

        // load images
        var _path = Application.streamingAssetsPath + "/1.png";
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.downloadHandler.isDone)
        {
        }

        //create a texture and load byte array to it
        // Texture size does not matter 
        Texture2D sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        bool isLoaded = sampleTexture.LoadImage(www.downloadHandler.data);
        // apply this texure as per requirement on image or material
        GameObject img = ImageItemPlaceholder.transform.Find("RawImage").gameObject;
        if (isLoaded)
        {
            ImageItemPlaceholder.name = "1.png";
            img.GetComponentInChildren<RawImage>().texture = sampleTexture;
        }

        GameObject newImage = Instantiate(ImageItemPlaceholder, ImageLinePlaceholder.transform, true);

        _path = Application.streamingAssetsPath + "/2.png";
        www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.downloadHandler.isDone)
        {
        }

        //create a texture and load byte array to it
        // Texture size does not matter 
        sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        isLoaded = sampleTexture.LoadImage(www.downloadHandler.data);
        // apply this texure as per requirement on image or material
        img = newImage.transform.Find("RawImage").gameObject;
        if (isLoaded)
        {
            newImage.name = "2.png";
            img.GetComponentInChildren<RawImage>().texture = sampleTexture;
        }

        GameObject newLine = Instantiate(ImageLinePlaceholder, ImageLinePlaceholder.transform.parent.transform, true);

        newImage = newLine.transform.GetChild(0).gameObject;
        _path = Application.streamingAssetsPath + "/3.png";
        www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.downloadHandler.isDone)
        {
        }

        //create a texture and load byte array to it
        // Texture size does not matter 
        sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        isLoaded = sampleTexture.LoadImage(www.downloadHandler.data);
        // apply this texure as per requirement on image or material
        img = newImage.transform.Find("RawImage").gameObject;
        if (isLoaded)
        {
            newImage.name = "3.png";
            img.GetComponentInChildren<RawImage>().texture = sampleTexture;
        }

        newImage = newLine.transform.GetChild(1).gameObject;
        _path = Application.streamingAssetsPath + "/4.png";
        www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.downloadHandler.isDone)
        {
        }

        //create a texture and load byte array to it
        // Texture size does not matter 
        sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        isLoaded = sampleTexture.LoadImage(www.downloadHandler.data);
        // apply this texure as per requirement on image or material
        img = newImage.transform.Find("RawImage").gameObject;
        if (isLoaded)
        {
            newImage.name = "4.png";
            img.GetComponentInChildren<RawImage>().texture = sampleTexture;
        }

    }

    private void OnMapReady()
    {
        Debug.Log("The map is ready!");

        StaticMarkerIcon = DefaultMarkerIcon;

        var i = 0;

        foreach (var tmo in TimeMarkerObjects)
        {
            if (i % 1 == 0)
            {
                var mo = new MarkerOptions()
                    .Position(new LatLng(tmo.LatLng.y, tmo.LatLng.x));

                if (tmo.IsPOI)
                    StaticMarkerIcon = POIMarkerIcon;
                else
                    StaticMarkerIcon = DefaultMarkerIcon;

                mo.Icon(NewCustomDescriptor());

                tmo.Marker = Map.AddMarker(mo);
                tmo.IdFromMarker = tmo.Marker.Id;
                //marker.Timestamp = tmo.Timestamp;


            }

            i++;
        }

        Map.SetOnMarkerClickListener(marker => OnMarkerClick(marker), false);
    }

    private void HandlePOIInfo(bool ShowOrHide)
    {
        ButtonModifyPicture.SetActive(ShowOrHide);
        ButtonModifyInfo.SetActive(ShowOrHide);
        ButtonMovePositionOfPOI.SetActive(ShowOrHide);
        LabelPOIInfo.SetActive(ShowOrHide);
        TextPOIInfo.SetActive(ShowOrHide);

        // check for info text and images
        if (ShowOrHide)
        {
            if (SelectedTimeMarkerObject.Title != null && SelectedTimeMarkerObject.Title.Length > 0
                || SelectedTimeMarkerObject.Description != null && SelectedTimeMarkerObject.Description.Length > 0)
                POIInfoText.text = SelectedTimeMarkerObject.Title + "\r\n" + SelectedTimeMarkerObject.Description;
            else
                POIInfoText.text = "Keine Informationen vorhanden";

            if (SelectedTimeMarkerObject.ImagesForPOI != null && SelectedTimeMarkerObject.ImagesForPOI.Count > 0)
            {
                ImageSlider.objs = SelectedTimeMarkerObject.ImagesForPOI;

                PhotoViewer.SetActive(ShowOrHide); //show
                VideoManager.gameObject.SetActive(!ShowOrHide); //hide
            }
            else
            {
                ImageSlider.objs = new List<string>();
                PhotoViewer.SetActive(!ShowOrHide); //hide
                VideoManager.gameObject.SetActive(ShowOrHide); //show
            }
        }
        else
        {
            POIInfoText.text = "Keine Informationen vorhanden";
            ImageSlider.objs = new List<string>();
            PhotoViewer.SetActive(ShowOrHide); // hide
            VideoManager.gameObject.SetActive(!ShowOrHide); //show
        }
    }

    private void OnMarkerClick(Marker marker)
    {
        VideoPause();

        // reset marker
        if (CurrentTouchedMarker != null)
        {
            if (SelectedTimeMarkerObject.IsPOI)
                StaticMarkerIcon = POIMarkerIcon;
            else
                StaticMarkerIcon = DefaultMarkerIcon;

            CurrentTouchedMarker.SetIcon(NewCustomDescriptor());
        }

        SelectedTimeMarkerObject = TimeMarkerObjects.Find(tmo => tmo.IdFromMarker.Equals(marker.Id));

        StaticMarkerIcon = ActiveMarkerIcon;
        marker.SetIcon(NewCustomDescriptor());
        CurrentTouchedMarker = marker;

        // show POI info objects
        HandlePOIInfo(true);

        //    //VideoManager.transform.position *= 100;

        //    TitleOfMarker.gameObject.SetActive(true);
        //    DescriptionOfMarker.gameObject.SetActive(true);
        //    ButtonModify.SetActive(true);



        //    if (currentTMO != null && currentTMO.Title.Length > 0)
        //        TitleOfMarker.text = currentTMO.Title;

        //    if (currentTMO != null && currentTMO.Description.Length > 0)
        //        DescriptionOfMarker.text = currentTMO.Description;

        //    //PopupMarker.SetActive(true);
        //    //obj.scale = 1.5f;
        //    //RawImage.gameObject.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {

        //Check for mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
        }

        if (VideoManager.isPaused)
        {
            if (SelectedTimeMarkerObject.Timestamp > 0)
                VideoManager.time = SelectedTimeMarkerObject.Timestamp - FirstTimeMarker;
        }

        if (VideoManager.isPlaying)
        {
            //var currentTimeStamp = DateTime.Now - StartTimeStamp;
            //Debug.Log("Update - currentTimeStamp:" + currentTimeStamp.TotalSeconds.ToString());

            //if ((int)currentTimeStamp.TotalSeconds < Duration)
            //{
            //    // set current value for video gage
            //    SliderVideoGage.value = (int)currentTimeStamp.TotalSeconds;

            //    // mark actual and last marker in map
            //    MarkActualAndLastMarker((int)currentTimeStamp.TotalSeconds, false, false);
            //}

            //var currentTimeStamp = FirstTimeMarker + SliderVideoGage.value;
            // Debug.Log("Update - currentTimeStamp:" + currentTimeStamp.ToString());

            // if ((int)currentTimeStamp < Duration)
            // {
            // set current value for video gage
            SliderVideoGage.value = (int)VideoManager.time;

            // mark actual and last marker in map
            MarkActualAndLastMarker((int)VideoManager.time, false, false);
            // }
        }
    }

    public void VideoPlay()
    {
        PauseIcon.SetActive(true);
        PlayIcon.SetActive(false);

        HandlePOIInfo(false);

        // reset marker
        if (CurrentTouchedMarker != null)
        {
            StaticMarkerIcon = DefaultMarkerIcon;
            CurrentTouchedMarker.SetIcon(NewCustomDescriptor());
        }

        if (IsVideoPaused)
        {
            IsVideoPaused = false;
            StartTimeStamp = DateTime.Now - OffsetTimeSpan;
            Debug.Log("VideoPlay - IsVideoPaused: " + StartTimeStamp);
            Debug.Log("VideoPlay - IsVideoPaused: " + DateTime.Now);

            //VideoManager.m_video.time = ((int)(DateTime.Now - StartTimeStamp).TotalSeconds);

            //VideoManager.time = (((int)(DateTime.Now - StartTimeStamp).TotalSeconds));

            if (SelectedTimeMarkerObject.Timestamp > 0)
                VideoManager.time = SelectedTimeMarkerObject.Timestamp - FirstTimeMarker;
            else
                VideoManager.time = 0;
        }
        else
        {
            StartTimeStamp = DateTime.Now;
        }

        VideoManager.Play();
    }

    public void HandlePOIInfoPanel(bool showOrHide)
    {
        Map.IsVisible = !showOrHide;
        POIInfo.SetActive(showOrHide);

        SliderVideoGage.gameObject.SetActive(!showOrHide);
        PlayIcon.SetActive(!showOrHide);

        if (!showOrHide)
            PauseIcon.SetActive(showOrHide);

        // check for info text and images
        if (showOrHide)
        {
            if (SelectedTimeMarkerObject.Title != null && SelectedTimeMarkerObject.Title.Length > 0
                || SelectedTimeMarkerObject.Description != null && SelectedTimeMarkerObject.Description.Length > 0)
            {
                InputPOIInfoTitle.text = SelectedTimeMarkerObject.Title;
                InputPOIInfoDescription.text = SelectedTimeMarkerObject.Description;
            }

            else
            {
                InputPOIInfoTitle.text = "";
                InputPOIInfoDescription.text = "";
            }

        }
        else
        {
            InputPOIInfoTitle.text = "";
            InputPOIInfoDescription.text = "";
        }
    }

    public void HandlePOIMovePanel(bool showOrHide)
    {
        POIMove.SetActive(showOrHide);

        SliderVideoGage.gameObject.SetActive(!showOrHide);
        PlayIcon.SetActive(!showOrHide);

        if (!showOrHide)
            PauseIcon.SetActive(showOrHide);

        // check for info text and images
        if (showOrHide)
        {
            MarkActualAndLastMarker(SelectedTimeMarkerObject.Timestamp - TimeMarkerObjects[0].Timestamp, true, true);

        }
        else
        {
            InputPOIInfoTitle.text = "";
            InputPOIInfoDescription.text = "";
        }
    }

    public void VideoPause()
    {
        PlayIcon.SetActive(true);
        PauseIcon.SetActive(false);

        IsVideoPaused = true;
        OffsetTimeSpan = PauseTimeSpan = DateTime.Now - StartTimeStamp;
        SliderVideoSecondsWhenPausePressed = (int)SliderVideoGage.value;
        VideoManager.Pause();
    }

    static ImageDescriptor NewCustomDescriptor()
    {
        return ImageDescriptor.FromTexture2D(StaticMarkerIcon);
    }

    private void MarkActualAndLastMarker(int currentPositionAsSecond, bool refreshAllMarker, bool isMoveEnabled)
    {

        Marker LastActualMarker = ActualMarker;

        foreach (var tmo in TimeMarkerObjects)
        {
            if ((tmo.Timestamp - FirstTimeMarker) < currentPositionAsSecond)
            {

                Debug.Log("MarkActualAndLastMarker: " + (tmo.Timestamp - FirstTimeMarker));

                if (tmo.Marker != null)
                {
                    SelectedTimeMarkerObject = tmo;
                    ActualMarker = tmo.Marker;
                }


                //if (refreshAllMarker)
                //{
                if (isMoveEnabled)
                    StaticMarkerIcon = DefaultMarker50PercentIcon;
                else if (IsSimpleView)
                    StaticMarkerIcon = SimpleViewDefaultMarkerIcon;
                else
                    StaticMarkerIcon = LeaveMarkerIcon;

                tmo.Marker.SetIcon(NewCustomDescriptor());

                Debug.Log("MarkActualAndLastMarker: mo.Marker.SetIcon(NewCustomDescriptor());");
                // }
            }
            else
            {
                if (refreshAllMarker)
                {
                    if (tmo.IsPOI)
                    {
                        if (isMoveEnabled)
                            StaticMarkerIcon = DefaultMarker50PercentIcon;
                        else
                            StaticMarkerIcon = POIMarkerIcon;
                    }
                    else
                    {
                        if (isMoveEnabled)
                            StaticMarkerIcon = DefaultMarker50PercentIcon;
                        else if (IsSimpleView)
                            StaticMarkerIcon = SimpleViewDefaultMarkerIcon;
                        else
                            StaticMarkerIcon = DefaultMarkerIcon;
                    }

                    tmo.Marker.SetIcon(NewCustomDescriptor());
                }
                else
                {
                    break;
                }

            }
        }

        // mark last marker as leave marker
        if (LastActualMarker != null)
        {
            if (isMoveEnabled)
                StaticMarkerIcon = DefaultMarker50PercentIcon;
            else if (IsSimpleView)
                StaticMarkerIcon = SimpleViewDefaultMarkerIcon;
            else
                StaticMarkerIcon = LeaveMarkerIcon;

            if (!LastActualMarker.Equals(ActualMarker))
            {
                LastActualMarker.SetIcon(NewCustomDescriptor());
            }
        }

        // mark current marker as active marker
        StaticMarkerIcon = ActiveMarkerIcon;
        if (IsSimpleView)
            StaticMarkerIcon = SimpleViewActiveMarkerIcon;

        if (ActualMarker != null)
        {
            ActualMarker.SetIcon(NewCustomDescriptor());
        }

    }

    public void ResetVideoPositionToStandard()
    {
        VideoManager.transform.position /= 100;
    }

    public void ShowPhotoLayer()
    {

    }

    public void UpdateVideoFromGage()
    {
        if (isMouseDown)
            if (VideoManager.isPlaying)
                VideoPause();

        if (VideoManager.isPaused)
        {
            //int differenceInSeconds = (int)SliderVideoGage.value - SliderVideoSecondsWhenPausePressed;

            //Debug.Log("UpdateVideoFromGage - pre calc: " + PauseTimeSpan);

            //if (differenceInSeconds < 0) //backward
            //    OffsetTimeSpan = PauseTimeSpan - new TimeSpan(0, 0, differenceInSeconds * (-1));
            //else // forward
            //    OffsetTimeSpan = PauseTimeSpan + new TimeSpan(0, 0, differenceInSeconds);

            Debug.Log("UpdateVideoFromGage - post calc: " + (int)SliderVideoGage.value);
            VideoManager.time = (int)SliderVideoGage.value;
            MarkActualAndLastMarker((int)SliderVideoGage.value, true, false);
        }
    }

    public void OpenPOIPanel()
    {
        VideoManager.transform.position *= 100;
    }

    public void OnSceneChange()
    {
        Map.IsVisible = false;
    }

    public void AddOrRemoveImageToPOI(GameObject obj)
    {
        //SelectedTimeMarkerObject = TimeMarkerObjects[0];

        if (SelectedTimeMarkerObject.ImagesForPOI != null && SelectedTimeMarkerObject.ImagesForPOI.Count > 0)
        {
            if (SelectedTimeMarkerObject.ImagesForPOI.Contains(obj.name)) // remove from list and set button
            {
                SelectedTimeMarkerObject.ImagesForPOI.Remove(obj.name);
                obj.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "HINZUFÜGEN";
                obj.GetComponent<Image>().color = new Color32(46, 204, 113, 255);
            }
            else //add to list and set button
            {
                SelectedTimeMarkerObject.IsPOI = true;
                SelectedTimeMarkerObject.ImagesForPOI.Add(obj.name);
                obj.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "ENTFERNEN";
                obj.GetComponent<Image>().color = new Color32(231, 76, 60, 255);
            }
        }
        else
        {
            //add to list and set button
            SelectedTimeMarkerObject.IsPOI = true;
            SelectedTimeMarkerObject.ImagesForPOI.Add(obj.name);
            obj.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "ENTFERNEN";
            obj.GetComponent<Image>().color = new Color32(231, 76, 60, 255);
        }

        ImageSlider.objs = SelectedTimeMarkerObject.ImagesForPOI;
        ImageSlider.index = 0;
    }

    public void HandlePOIImages(bool showOrHide)
    {
        Map.IsVisible = !showOrHide;
        VideoManager.gameObject.SetActive(!showOrHide);

        var _path = Application.streamingAssetsPath + "/placeholder.png";
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.downloadHandler.isDone)
        {
        }

        ImageSlider.image.sprite.texture.LoadImage(www.downloadHandler.data);
        ImageSlider.index = 0;


        POIImages.SetActive(showOrHide);

        SliderVideoGage.gameObject.SetActive(!showOrHide);
        PlayIcon.SetActive(!showOrHide);

        if (!showOrHide)
            PauseIcon.SetActive(showOrHide);

        if (showOrHide)
            PhotoViewer.SetActive(showOrHide);
    }

    public void SavePOIInfoTitle(GameObject titelObj)
    {
        string title = "";

        title = titelObj.GetComponent<TMP_InputField>().text;


        Debug.Log("SavePOIInfoTitle: " + title);


        SelectedTimeMarkerObject.Title = title;
        SelectedTimeMarkerObject.IsPOI = true;

        if (SelectedTimeMarkerObject.Title != null && SelectedTimeMarkerObject.Title.Length > 0
    || SelectedTimeMarkerObject.Description != null && SelectedTimeMarkerObject.Description.Length > 0)
            POIInfoText.text = SelectedTimeMarkerObject.Title + "\r\n" + SelectedTimeMarkerObject.Description;
        else
            POIInfoText.text = "Keine Informationen vorhanden";
    }

    public void SavePOIInfoDesription(GameObject descObj)
    {
        string desc = "";

        desc = descObj.GetComponent<TMP_InputField>().text;

        Debug.Log("SavePOIInfoDesription: " + desc);

        SelectedTimeMarkerObject.Description = desc;
        SelectedTimeMarkerObject.IsPOI = true;

        if (SelectedTimeMarkerObject.Title != null && SelectedTimeMarkerObject.Title.Length > 0
    || SelectedTimeMarkerObject.Description != null && SelectedTimeMarkerObject.Description.Length > 0)
            POIInfoText.text = SelectedTimeMarkerObject.Title + "\r\n" + SelectedTimeMarkerObject.Description;
        else
            POIInfoText.text = "Keine Informationen vorhanden";
    }


    public void HandleSimpleView(bool isActive)
    {
        if (IsSimpleView)
            IsSimpleView = false;
        else
            IsSimpleView = true;

        if (IsSimpleView)
        {
            Map.SetOnCircleClickListener(circle =>
            {
                Debug.Log("Circle clicked: " + circle);
                SliderReassurance.gameObject.SetActive(true);
            });

            Map.SetOnLongMapClickListener(point =>
            {
                Debug.Log("Map long clicked: " + point);
                //Map.AddCircle(DemoUtils.RandomColorCircleOptions(point));

                var mo = new MarkerOptions()
                        .Position(point);

                StaticMarkerIcon = POIReassuranceMarkerIcon;


                mo.Icon(NewCustomDescriptor());

                Map.AddMarker(mo);

                Map.AddCircle(DemoUtils.RandomColorCircleOptions(point));

            });

           
        }
    }
}
