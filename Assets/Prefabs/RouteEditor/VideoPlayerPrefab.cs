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

    private string VideoUrl;
    private double VideoTimestamp;

    // Start is called before the first frame update
    void Start()
    {
        //LoadVideo(VideoUrl);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SkipToVideoFrame(double timestamp)
    {
        VideoManager.Play();
        VideoManager.Pause();
        VideoManager.time = timestamp;
        PlayButton.gameObject.SetActive(true);

        Debug.Log("Timestamp: " + timestamp);
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
        VideoTimestamp = VideoManager.time;
        VideoManager.Pause();
    }

    public void ResumeVideo()
    {
        SkipToVideoFrame(VideoTimestamp);
    }


}
