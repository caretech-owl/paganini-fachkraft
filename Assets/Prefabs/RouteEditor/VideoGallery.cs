using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SocketsAPI;

public class VideoGallery : MonoBehaviour
{
    public VideoPlayerPrefab VideoPlayer;

    // Time of playback before POI
    public double BeforePOIVideoPlayback = 10;

    private Pathpoint POIStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadVideo(Pathpoint start )
    {
        POIStart = start;
        VideoPlayer.LoadVideo(FileManagement.persistentDataPath + "/" + AppState.CurrentRoute.LocalVideoFilename);
    }

    /// <summary>
    /// Moves the VideoPlayback to the pathpoint timestamp, so as to
    /// see the video around the current pathpoint
    /// </summary>
    /// <param name = "pathpoint" > Pathpoint to synchronize with</param>
    public void LimitPlaybackTimeframe(Pathpoint currentPOI, Pathpoint nextPOI)
    {
        double startTime = (currentPOI.Timestamp - POIStart.Timestamp) / 1000;
        double endTime = (nextPOI.Timestamp - POIStart.Timestamp) / 1000;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(currentPOI.Photos[0].Photo);
        VideoPlayer.SetupPlayback(startTime - BeforePOIVideoPlayback, endTime, texture);
    }
}
