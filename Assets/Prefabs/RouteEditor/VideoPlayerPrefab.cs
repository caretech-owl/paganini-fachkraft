using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerPrefab : MonoBehaviour
{
    public VideoPlayer VideoManager;

    [Header("Video Controllers")]
    public Button PlayButton;
    public RawImage Preview;

    private string VideoUrl;
    private double VideoTimestamp;
    private Texture2D VideoPreviewTexture;
    //todo: add status of playback, add PlayVideo function as well

    // Start is called before the first frame update
    void Start()
    {
      //  VideoManager.prepareCompleted += OnVideoPrepared;
      //  VideoManager.seekCompleted += OnSeekCompleted;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SkipToVideoFrame(double timestamp, Texture2D preview = null)
    {
        Debug.Log("SkipToVideoFrame from: " + VideoManager.time + "o Timestamp: " + timestamp);
        VideoTimestamp = timestamp;
        VideoPreviewTexture = preview;

        VideoManager.time = timestamp;

        if (!VideoManager.isPrepared)
        {
            VideoManager.prepareCompleted += OnVideoPrepareCompleted;
            VideoManager.Play();
        }


        if (preview != null)
        {
            Preview.gameObject.SetActive(true);
            Preview.texture = preview;
        }
        else
        {
            Preview.gameObject.SetActive(false);
        }
        

    }

    private IEnumerator RetrySkip(double timestamp)
    {
        // Wait for the next frame to be rendered

        Debug.Log(string.Format("Retry! VideoManager.time : {0}  Timestamp:{1}  Paused? : {2}",
            VideoManager.time, timestamp, VideoManager.isPaused));

        while (VideoManager.isPaused &&
            VideoTimestamp == timestamp &&
            Mathf.Abs((float)(VideoManager.time - timestamp)) > 1)
        {                        
            VideoManager.time = timestamp;
            Debug.Log("Retry! :" + timestamp);
            yield return new WaitForSecondsRealtime(0.5f);
        }

        Debug.Log("Done Trying! :");

    }

    private void OnVideoPrepareCompleted(VideoPlayer player)
    {
        Debug.Log("Video prepared!");

        VideoManager.prepareCompleted -= OnVideoPrepareCompleted;
        VideoManager.time = VideoTimestamp;
        VideoManager.Pause();
        PlayButton.gameObject.SetActive(true);
    }

    private void OnVideoStarted(VideoPlayer player)
    {
        Debug.Log("Video started!");

        VideoManager.started -= OnVideoStarted;
        VideoManager.time = VideoTimestamp;
        VideoManager.Pause();
        PlayButton.gameObject.SetActive(true);
    }

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


    private IEnumerator UpdateVideoFrameAndPause(double timestamp)
    {
        // Wait for the next frame to be rendered
        yield return null;
        SkipTo(timestamp);
    }

    private void SkipTo(double timestamp)
    {
        // Set the time and update the associated frame
        VideoManager.Pause();
        VideoManager.time = timestamp;
        VideoManager.frame = (long)(timestamp * VideoManager.frameRate);

        //VideoManager. += OnFrameReady;

        //// Pause the video player
        //VideoManager.Pause();

        //// Show the play button
        //PlayButton.gameObject.SetActive(true);

        Debug.Log("Timestamp: " + timestamp);
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("Video ready!");
        SkipTo(VideoTimestamp);
        VideoManager.Pause();
    }

    void OnFrameReady(VideoPlayer source, long frameIdx)
    {
        VideoManager.frameReady -= OnFrameReady;
        Debug.Log("Frame ready! timestamp:" + source.time + " frame: "+ frameIdx);
        //SkipTo(VideoTimestamp);
        // Pause the video player
        VideoManager.Pause();

        // Show the play button
        PlayButton.gameObject.SetActive(true);
    }

    void OnSeekCompleted(VideoPlayer source)
    {
        Debug.Log("Seek completed! timestamp:" + source.time);
        //SkipTo(VideoTimestamp);
        // Pause the video player
        VideoManager.Play();
        VideoManager.Pause();

        // Show the play button
        PlayButton.gameObject.SetActive(true);
    }

    public void LoadVideo(string url)
    {
        if (VideoManager != null)
        {
            VideoManager.url = url;
            if (isActiveAndEnabled)
            {
                SkipToVideoFrame(0);
            }            
        }
        VideoUrl = url;
    }

    public void PauseVideo()
    {
        VideoTimestamp = VideoTimestamp > 0 ? VideoTimestamp : VideoManager.time;
        VideoManager.Pause();
    }

    public void ResumeVideo()
    {
        SkipToVideoFrame(VideoTimestamp, VideoPreviewTexture);
    }

    public void PlayVideo()
    {
        VideoManager.time = VideoTimestamp;
        VideoManager.Play();
        VideoTimestamp = -1;
        VideoPreviewTexture = null;
    }




}
