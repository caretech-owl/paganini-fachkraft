using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErkundungsbegehungController : MonoBehaviour
{

    public UnityEngine.Video.VideoPlayer VideoManager;

    public OnlineMapsMarkerManager MapsMarkerManager;

    public Texture2D Marker;

    private List<Vector2> WayPoints = new List<Vector2>()
    {
        new Vector2((float)8.90259075164795, (float)52.2963333129883),
new Vector2((float)8.90258407592773, (float)52.2963218688965),
new Vector2((float)8.9025936126709, (float)52.2963066101074),
new Vector2((float)8.90259552001953, (float)52.2962951660156),
new Vector2((float)8.90261268615723, (float)52.2962913513184),
new Vector2((float)8.90263271331787, (float)52.2962875366211),
new Vector2((float)8.90265941619873, (float)52.2962799072266),
new Vector2((float)8.90268039703369, (float)52.2962760925293),
new Vector2((float)8.90270233154297, (float)52.2962684631348),
new Vector2((float)8.90272235870361, (float)52.2962646484375),
new Vector2((float)8.90274143218994, (float)52.296257019043 ),
new Vector2((float)8.90275764465332, (float)52.2962493896484),
new Vector2((float)8.90277576446533, (float)52.2962455749512),
new Vector2((float)8.90279197692871, (float)52.2962417602539),
new Vector2((float)8.90280818939209, (float)52.2962379455566),
new Vector2((float)8.90282821655273, (float)52.2962455749512),
new Vector2((float)8.90284824371338, (float)52.2962532043457),
new Vector2((float)8.90287113189697, (float)52.2962646484375),
new Vector2((float)8.9028959274292 , (float)52.2962684631348),
new Vector2((float)8.90291786193848, (float)52.296272277832 ),
new Vector2((float)8.90293312072754, (float)52.296272277832 ),
new Vector2((float)8.90293025970459, (float)52.2962646484375),
new Vector2((float)8.90292453765869, (float)52.2962532043457),
new Vector2((float)8.90291881561279, (float)52.2962455749512),
new Vector2((float)8.90291118621826, (float)52.2962341308594),
new Vector2((float)8.90290927886963, (float)52.2962226867676),
new Vector2((float)8.90291023254395, (float)52.2962074279785),
new Vector2((float)8.90291023254395, (float)52.2961921691895),
new Vector2((float)8.90290355682373, (float)52.2961769104004),
new Vector2((float)8.90289783477783, (float)52.2961654663086),
new Vector2((float)8.9028902053833 , (float)52.2961578369141),
new Vector2((float)8.90288639068604, (float)52.2961463928223),
new Vector2((float)8.9028787612915 , (float)52.2961387634277),
new Vector2((float)8.90287017822266, (float)52.2961273193359),
new Vector2((float)8.90286064147949, (float)52.2961196899414),
new Vector2((float)8.90285205841064, (float)52.2961082458496),
new Vector2((float)8.90284538269043, (float)52.2960968017578),
new Vector2((float)8.90283870697022, (float)52.296085357666 ),
new Vector2((float)8.90283393859863, (float)52.2960739135742),
new Vector2((float)8.90282821655273, (float)52.2960624694824),
new Vector2((float)8.90282249450684, (float)52.2960510253906),
new Vector2((float)8.90281677246094, (float)52.2960395812988),
new Vector2((float)8.90281200408936, (float)52.296028137207 ),
new Vector2((float)8.90280723571777, (float)52.2960166931152),
new Vector2((float)8.90280151367188, (float)52.2960052490234),
new Vector2((float)8.90279960632324, (float)52.2959938049316),
new Vector2((float)8.90279865264893, (float)52.2959823608398),
new Vector2((float)8.90279865264893, (float)52.2959709167481),
new Vector2((float)8.90279674530029, (float)52.295955657959 ),
new Vector2((float)8.90279197692871, (float)52.2959442138672),
new Vector2((float)8.90278816223145, (float)52.2959327697754),
new Vector2((float)8.90278339385986, (float)52.2959175109863),
new Vector2((float)8.90278244018555, (float)52.2959060668945),
new Vector2((float)8.90278244018555, (float)52.2958908081055),
new Vector2((float)8.90278339385986, (float)52.2958793640137),
new Vector2((float)8.90278816223145, (float)52.2958717346191),
new Vector2((float)8.90280055999756, (float)52.2958793640137),
new Vector2((float)8.90279960632324, (float)52.2958946228027),
new Vector2((float)8.90279960632324, (float)52.2959060668945),
new Vector2((float)8.90280246734619, (float)52.2959175109863),
new Vector2((float)8.90280532836914, (float)52.2959327697754),
new Vector2((float)8.90280914306641, (float)52.2959442138672),
new Vector2((float)8.90281200408936, (float)52.295955657959 ),
new Vector2((float)8.90281486511231, (float)52.2959709167481),
new Vector2((float)8.90281772613525, (float)52.2959823608398),
new Vector2((float)8.90282154083252, (float)52.2959938049316),
new Vector2((float)8.90282344818115, (float)52.2960090637207),
new Vector2((float)8.90282440185547, (float)52.2960205078125),
new Vector2((float)8.9028263092041 , (float)52.2960319519043),
new Vector2((float)8.90282917022705, (float)52.2960472106934),
new Vector2((float)8.90283203125   , (float)52.2960586547852),
new Vector2((float)8.90283679962158, (float)52.2960739135742),
new Vector2((float)8.90284252166748, (float)52.296085357666 ),
new Vector2((float)8.9028491973877 , (float)52.2961006164551),
new Vector2((float)8.90285682678223, (float)52.2961120605469),
new Vector2((float)8.90286350250244, (float)52.2961273193359),
new Vector2((float)8.90286827087402, (float)52.2961387634277),
new Vector2((float)8.90287399291992, (float)52.2961502075195),
new Vector2((float)8.90287971496582, (float)52.2961616516113),
new Vector2((float)8.9028844833374 , (float)52.2961730957031),
new Vector2((float)8.90288734436035, (float)52.2961845397949),
new Vector2((float)8.90289115905762, (float)52.2961959838867),
new Vector2((float)8.9028959274292 , (float)52.2962074279785),
new Vector2((float)8.9029016494751 , (float)52.2962188720703),
new Vector2((float)8.90290832519531, (float)52.2962303161621),
new Vector2((float)8.90291309356689, (float)52.2962417602539),
new Vector2((float)8.90292263031006, (float)52.296257019043 ),
new Vector2((float)8.90292835235596, (float)52.2962684631348),
new Vector2((float)8.90293312072754, (float)52.2962760925293),
    };

    // Start is called before the first frame update
    void Start()
    {

        foreach(var wp in WayPoints)
        {
            OnlineMapsMarker omm = new OnlineMapsMarker()
            {
                position = new Vector2(wp.x, wp.y),
                texture = Marker
            };


            MapsMarkerManager.Add(omm);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VideoPlay()
    {
        VideoManager.Play();
    }

    public void VideoPause()
    {
        VideoManager.Pause();
    }
}
