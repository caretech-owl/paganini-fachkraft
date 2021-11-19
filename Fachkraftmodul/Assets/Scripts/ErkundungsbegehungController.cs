using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErkundungsbegehungController : MonoBehaviour
{

    public UnityEngine.Video.VideoPlayer VideoManager;

    public OnlineMapsMarkerManager MapsMarkerManager;

    public Texture2D Marker;

    public Texture2D ActiveMarker;

    public Texture2D LeaveMarker;

    public GameObject TimeMarker;

    public GameObject PopupMarker;

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

    private int StepSize = 0;

    private int Duration = 0;

    private int FirstTimeMarker = 0;

    private DateTime StartTimeStamp;

    private bool IsVideoPaused = false;

    private TimeSpan PauseTimeSpan;

    // Start is called before the first frame update
    void Start()
    {
        var i = 0;

        foreach (var tmo in TimeMarkerObjects)
        {
            if (i % 1 == 0)
            {
                OnlineMapsMarker omm = new OnlineMapsMarker()
                {
                    position = new Vector2(tmo.LatLng.x, tmo.LatLng.y),
                    texture = Marker
                };

                omm.OnClick += OnMarkerClick;

                MapsMarkerManager.Add(omm);
                tmo.Marker = omm;
            }

            i++;
        }

        FirstTimeMarker = (int)TimeMarkerObjects[0].Timestamp;
        Debug.Log("FirstTimeMarker:" + FirstTimeMarker);

        Duration = (int)(TimeMarkerObjects[TimeMarkerObjects.Count - 1].Timestamp - TimeMarkerObjects[0].Timestamp);
        StepSize = (int)(1400 / Duration);

    }

    private void OnMarkerClick(OnlineMapsMarkerBase obj)
    {
        VideoManager.gameObject.SetActive(false);
        PopupMarker.SetActive(true);
        obj.scale = 1.5f;

    }

    // Update is called once per frame
    void Update()
    {
        if (VideoManager.isPlaying)
        {
            var currentTimeStamp = DateTime.Now - StartTimeStamp;
            //Debug.Log(currentTimeStamp.TotalSeconds.ToString());


            if ((int)currentTimeStamp.TotalSeconds < Duration)
            {
                var theBarRectTransform = TimeMarker.transform as RectTransform;

                int playtime = 1510 - (int)currentTimeStamp.TotalSeconds * StepSize;

                theBarRectTransform.offsetMax = new Vector2(playtime * (-1), theBarRectTransform.offsetMax.y); //= .xMax -= (int)currentTimeStamp.TotalSeconds * StepSize;

                //TimeMarkerObject tmo = 
                FindCurrentTimeObject((int)currentTimeStamp.TotalSeconds);

                //if (tmo != null)
                //{
                //    Debug.Log("Marker found");
                //    tmo.Marker.texture = ActiveMarker;
                //}
                //else
                    //Debug.Log("no Marker");


                //theBarRectTransform.sizeDelta = new Vector2((int)currentTimeStamp.TotalSeconds * StepSize, 50);

                //Vector3 temp = new Vector3((float)((currentTimeStamp.TotalSeconds * StepSize) / 2) / 100000, 0, 0);
                //Debug.Log((float)(currentTimeStamp.TotalSeconds * StepSize) / 2);

                //TimeMarker.transform.position += temp;
            }
        }
    }

    public void VideoPlay()
    {
        if (IsVideoPaused)
        {
            IsVideoPaused = false;
            StartTimeStamp = DateTime.Now - PauseTimeSpan;
        }
        else
        {
            StartTimeStamp = DateTime.Now;
        }

        VideoManager.Play();
        
    }

    public void VideoPause()
    {
        IsVideoPaused = true;
        PauseTimeSpan = DateTime.Now - StartTimeStamp;
        VideoManager.Pause();
    }

    private void FindCurrentTimeObject(int currentPositionAsSecond)
    {
        var actualMarker = LeaveMarker;

        foreach (var tmo in TimeMarkerObjects)
        {
            Debug.Log("find:" + currentPositionAsSecond + "<" + (tmo.Timestamp - FirstTimeMarker));

            if (currentPositionAsSecond < (tmo.Timestamp - FirstTimeMarker))
            {
                Debug.Log("FOUND MARKER");
                actualMarker = Marker;
                tmo.Marker.scale = 1.5f;
                break;
                //return tmo;
            }
            else
            {
                tmo.Marker.scale = 1.0f;
                //tmo.Marker.texture = Marker;
            }

            tmo.Marker.texture = actualMarker;

        }

        //return null;
    }
}
