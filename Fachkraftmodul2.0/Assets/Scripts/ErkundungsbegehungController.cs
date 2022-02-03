using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using NinevaStudios.GoogleMaps;

public class ErkundungsbegehungController : MonoBehaviour
{

    public VideoPlayer VideoManager;

    //public OnlineMapsMarkerManager MapsMarkerManager;

    private static Texture2D StaticMarkerIcon;

    public Texture2D DefaultMarkerIcon;

    public Texture2D ActiveMarkerIcon;

    public Texture2D LeaveMarkerIcon;

    public GameObject PopupMarker;

    public GameObject PopupPhoto;

    public Slider SliderPhoto;

    public Text SliderPhotoText;

    public Slider SliderVideoGage;

    public GameObject PlayIcon;

    public GameObject PauseIcon;

    public RawImage RawImage;

    public Text TitleOfMarker;

    public Text DescriptionOfMarker;

    public GameObject ButtonModify;

    // Info

    public GameObject ButtonModifyPicture;

    public GameObject ButtonModifyInfo;

    public GameObject ButtonMovePositionOfPOI;

    public GameObject LabelPOIInfo;

    public GameObject TextPOIInfo;

    public GameObject POIInfo;

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

    // Start is called before the first frame update
    void Start()
    {
        StaticMarkerIcon = DefaultMarkerIcon;

        // init map
        var cameraPosition = new CameraPosition(
        new LatLng(TimeMarkerObjects[30].LatLng.y, TimeMarkerObjects[30].LatLng.x), 19, 0, 0);
        var options = new GoogleMapsOptions()
            .Camera(cameraPosition);

        Map = new GoogleMapsView(options);
        Map.Show(new Rect(830, 120, 1050, 750), OnMapReady);

        FirstTimeMarker = (int)TimeMarkerObjects[0].Timestamp;
        Debug.Log("FirstTimeMarker:" + FirstTimeMarker);

        Duration = (int)(TimeMarkerObjects[TimeMarkerObjects.Count - 1].Timestamp - TimeMarkerObjects[0].Timestamp);
        Debug.Log("Duration: " + Duration);

        StepSize = 1 / Duration;
        Debug.Log("StepSize: " + StepSize);

        SliderVideoGage.maxValue = Duration;
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
                mo.Icon(NewCustomDescriptor());

                var marker = Map.AddMarker(mo);
                tmo.Marker = marker;
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

    }

    private void OnMarkerClick(Marker marker)
    {
        VideoPause();

        HandlePOIInfo(true);

        // reset marker
        if (CurrentTouchedMarker != null)
        {
            StaticMarkerIcon = DefaultMarkerIcon;
            CurrentTouchedMarker.SetIcon(NewCustomDescriptor());
        }


        StaticMarkerIcon = ActiveMarkerIcon;
        marker.SetIcon(NewCustomDescriptor());
        CurrentTouchedMarker = marker;

        //    //VideoManager.transform.position *= 100;

        //    TitleOfMarker.gameObject.SetActive(true);
        //    DescriptionOfMarker.gameObject.SetActive(true);
        //    ButtonModify.SetActive(true);

        //    TimeMarkerObject currentTMO = TimeMarkerObjects.Find(tmo => tmo.Timestamp.Equals(obj.timestamp));

        //    SelectedTimeMarkerObject = currentTMO;

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
        }

        if (VideoManager.isPlaying)
        {
            var currentTimeStamp = DateTime.Now - StartTimeStamp;
            Debug.Log("Update - currentTimeStamp:" + currentTimeStamp.TotalSeconds.ToString());

            if ((int)currentTimeStamp.TotalSeconds < Duration)
            {
                // set current value for video gage
                SliderVideoGage.value = (int)currentTimeStamp.TotalSeconds;

                // mark actual and last marker in map
                MarkActualAndLastMarker((int)currentTimeStamp.TotalSeconds, false);
            }
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

            VideoManager.time = (((int)(DateTime.Now - StartTimeStamp).TotalSeconds));
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

    private void MarkActualAndLastMarker(int currentPositionAsSecond, bool refreshAllMarker)
    {

        Marker LastActualMarker = ActualMarker;

        foreach (var tmo in TimeMarkerObjects)
        {
            if ((tmo.Timestamp - FirstTimeMarker) < currentPositionAsSecond)
            {
                if (tmo.Marker != null)
                    ActualMarker = tmo.Marker;

                if (refreshAllMarker)
                {
                    StaticMarkerIcon = LeaveMarkerIcon;
                    tmo.Marker.SetIcon(NewCustomDescriptor());
                }
            }
            else
            {
                if (refreshAllMarker)
                {
                    StaticMarkerIcon = DefaultMarkerIcon;
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
            StaticMarkerIcon = LeaveMarkerIcon;

            if (!LastActualMarker.Equals(ActualMarker))
            {
                LastActualMarker.SetIcon(NewCustomDescriptor());
            }
        }

        // mark current marker as active marker
        StaticMarkerIcon = ActiveMarkerIcon;
        if (ActualMarker != null)
            ActualMarker.SetIcon(NewCustomDescriptor());

    }

    public void ResetVideoPositionToStandard()
    {
        VideoManager.transform.position /= 100;
    }

    public void ShowPhotoLayer()
    {
        RawImage.gameObject.SetActive(true);

        if (VideoManager.isPlaying)
            VideoPause();

        VideoManager.transform.position *= 100;
        PopupPhoto.SetActive(true);

        SliderPhoto.value = 0.2f;
        SliderPhotoText.text = "2:10 Min";

    }

    public void UpdateVideoFromGage()
    {
        if (isMouseDown)
            if (VideoManager.isPlaying)
                VideoPause();

        if (VideoManager.isPaused)
        {
            int differenceInSeconds = (int)SliderVideoGage.value - SliderVideoSecondsWhenPausePressed;

            Debug.Log("UpdateVideoFromGage - pre calc: " + PauseTimeSpan);

            if (differenceInSeconds < 0) //backward
                OffsetTimeSpan = PauseTimeSpan - new TimeSpan(0, 0, differenceInSeconds * (-1));
            else // forward
                OffsetTimeSpan = PauseTimeSpan + new TimeSpan(0, 0, differenceInSeconds);

            Debug.Log("UpdateVideoFromGage - post calc: " + PauseTimeSpan);

            MarkActualAndLastMarker((int)SliderVideoGage.value, true);
        }
    }

    public void OpenPOIPanel()
    {
        VideoManager.transform.position *= 100;

        if (SelectedTimeMarkerObject != null && SelectedTimeMarkerObject.Title.Length > 0)
            TitleOfMarker.text = SelectedTimeMarkerObject.Title;

        if (SelectedTimeMarkerObject != null && SelectedTimeMarkerObject.Description.Length > 0)
            DescriptionOfMarker.text = SelectedTimeMarkerObject.Description;

        //SliderPOIGage.value = SelectedTimeMarkerObject.Timestamp - TimeMarkerObjects[0].Timestamp;
        //SliderPhotoText.text = SliderPOIGage.value + " Sek.";

        PopupMarker.SetActive(true);
        //obj.scale = 1.5f;
        RawImage.gameObject.SetActive(false);
    }

    public void OnSceneChange()
    {
        Map.IsVisible = false;
    }
}
