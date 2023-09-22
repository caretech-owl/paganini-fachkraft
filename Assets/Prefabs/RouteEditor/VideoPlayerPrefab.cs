using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerPrefab : MonoBehaviour
{
    public VideoPlayer VideoManager;

    [Header("UI States")]
    public GameObject DataState;
    public GameObject BlankState;

    [Header("Video Controllers")]
    public GameObject PlayControl;
    public GameObject ReplayControl;
    public GameObject FwdControl;
    public GameObject BackControl;
    public GameObject PreviewWrapper;
    public GameObject ControlWrapper;
    public RawImage Preview;

    private double StartTimestamp;
    private double EndTimestamp;
    private bool awaitingPlaybackAction;


    // Start is called before the first frame update
    void Start()
    {
      //  VideoManager.prepareCompleted += OnVideoPrepared;
      //  VideoManager.seekCompleted += OnSeekCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        // We wait until the replay or skip action is performed by the video player,
        // otherwise, it would bring up the end of video screen again in the next frame
        if (awaitingPlaybackAction && VideoManager.time < EndTimestamp) {
            awaitingPlaybackAction = false;
        }
        else if (awaitingPlaybackAction)
        {
            return;
        }

        if (VideoManager.time >= EndTimestamp && VideoManager.isPlaying)
        {
            EnableReplayControl(true);
            EnableVideoControls();
        }

    }


    public void SetupPlayback(double startTime, double endTime, Texture2D preview = null)
    {
        Debug.Log("SkipToVideoFrame from: " + VideoManager.time + "o Timestamp: " + startTime);
       
        VideoManager.time = startTime;

        StartTimestamp = startTime;
        EndTimestamp = endTime;

        awaitingPlaybackAction = false;
        videoJustLoaded = true;

        // Default controls when we load the video
        EnableReplayControl(false);
        FwdControl.SetActive(false);
        BackControl.SetActive(false);

        if (!VideoManager.isPrepared)
        {
            VideoManager.prepareCompleted += OnVideoPrepareCompleted;
            VideoManager.Play();
        }

        if (preview != null)
        {
            PreviewWrapper.SetActive(true);
            Preview.gameObject.SetActive(true);
            Preview.texture = preview;
        }
        else
        {
            Preview.gameObject.SetActive(false);
        }        
    }

    private bool videoJustLoaded = true;
    public void EnableVideoControls() {
        VideoManager.Pause();
        ControlWrapper.SetActive(true);
        
        FwdControl.SetActive(VideoManager.time < EndTimestamp && !videoJustLoaded);
        BackControl.SetActive(VideoManager.time > StartTimestamp && !videoJustLoaded);

        EnableReplayControl(VideoManager.time >= EndTimestamp);

        videoJustLoaded = false;
    }

    public void ResumePlayback() {
        VideoManager.Play();
        PreviewWrapper.SetActive(false);
        ControlWrapper.SetActive(false);
    }

    public void Replay()
    {
        // Takes some time (frames) to actually play
        awaitingPlaybackAction = true;
        VideoManager.time = StartTimestamp;        
        ControlWrapper.SetActive(false);

        EnableReplayControl(false);

        VideoManager.Play();

    }

    public void SkipForward() {
        //double playtime = VideoManager.clip.length;
        SkipTimeAndPlay(10);
    }

    public void SkipBackwards() {
        SkipTimeAndPlay(-10);
    }

    private void SkipTimeAndPlay(double skipTime)
    {
        awaitingPlaybackAction = true;
        double targetTime = VideoManager.time + skipTime;
        // Let's check the boundaries of our playback
        if (skipTime>0) {
            skipTime = targetTime > EndTimestamp ?  EndTimestamp - VideoManager.time : skipTime;
        } else {
            skipTime = targetTime < StartTimestamp ? StartTimestamp - VideoManager.time  : skipTime;
        }
        

        VideoManager.time += skipTime;
        VideoManager.Play();
        ControlWrapper.SetActive(false);
    }

    private void OnVideoPrepareCompleted(VideoPlayer player)
    {
        Debug.Log("Video prepared!");

        VideoManager.prepareCompleted -= OnVideoPrepareCompleted;
        VideoManager.time = StartTimestamp;
        
        EnableVideoControls();
    }

    private void EnableReplayControl(bool activate) {
        ReplayControl.SetActive(activate);
        PlayControl.SetActive(!activate);        
    }

    //private void OnVideoStarted(VideoPlayer player)
    //{
    //    Debug.Log("Video started!");

    //    VideoManager.started -= OnVideoStarted;
    //    VideoManager.time = VideoTimestamp;
    //    VideoManager.Pause();
    //    PlayButton.gameObject.SetActive(true);
    //}

    //public void SkipToVideoFrame(double timestamp)
    //{
    //    VideoManager.Play();
    //    VideoTimestamp = timestamp;
    //    if (VideoManager.isPrepared)
    //    {
    //        Debug.Log("Video Prepared");
    //        StartCoroutine(UpdateVideoFrameAndPause(timestamp));
    //    }
    //}


    //private IEnumerator UpdateVideoFrameAndPause(double timestamp)
    //{
    //    // Wait for the next frame to be rendered
    //    yield return null;
    //    SkipTo(timestamp);
    //}

    //private void SkipTo(double timestamp)
    //{
    //    // Set the time and update the associated frame
    //    VideoManager.Pause();
    //    VideoManager.time = timestamp;
    //    VideoManager.frame = (long)(timestamp * VideoManager.frameRate);

    //    //VideoManager. += OnFrameReady;

    //    //// Pause the video player
    //    //VideoManager.Pause();

    //    //// Show the play button
    //    //PlayButton.gameObject.SetActive(true);

    //    Debug.Log("Timestamp: " + timestamp);
    //}

    //void OnVideoPrepared(VideoPlayer source)
    //{
    //    Debug.Log("Video ready!");
    //    SkipTo(VideoTimestamp);
    //    VideoManager.Pause();
    //}

    //void OnFrameReady(VideoPlayer source, long frameIdx)
    //{
    //    VideoManager.frameReady -= OnFrameReady;
    //    Debug.Log("Frame ready! timestamp:" + source.time + " frame: "+ frameIdx);
    //    //SkipTo(VideoTimestamp);
    //    // Pause the video player
    //    VideoManager.Pause();

    //    // Show the play button
    //    PlayButton.gameObject.SetActive(true);
    //}

    //void OnSeekCompleted(VideoPlayer source)
    //{
    //    Debug.Log("Seek completed! timestamp:" + source.time);
    //    //SkipTo(VideoTimestamp);
    //    // Pause the video player
    //    VideoManager.Play();
    //    VideoManager.Pause();

    //    // Show the play button
    //    PlayButton.gameObject.SetActive(true);
    //}

    public void LoadVideo(string url)
    {
        if (File.Exists(url)) {
            BlankState.SetActive(false);
            DataState.SetActive(true);

            if (VideoManager != null)
            {
                VideoManager.url = url;   
            }
        } else {
            DataState.SetActive(false);
            BlankState.SetActive(true);
        }

    }

    //public void PauseVideo()
    //{
    //    VideoTimestamp = VideoTimestamp > 0 ? VideoTimestamp : VideoManager.time;
    //    VideoManager.Pause();
    //}

    //public void ResumeVideo()
    //{
    //    SkipToVideoFrame(VideoTimestamp, VideoPreviewTexture);
    //}

    //public void PlayVideo()
    //{
    //    VideoManager.time = VideoTimestamp;
    //    VideoManager.Play();
    //    VideoTimestamp = -1;
    //    VideoPreviewTexture = null;
    //}




}
