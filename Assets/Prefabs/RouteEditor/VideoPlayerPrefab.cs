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
        VideoManager.time = timestamp;
        VideoManager.Pause();
        PlayButton.gameObject.SetActive(true);
    }

    public void LoadVideo(string url)
    {
        if (VideoManager != null)
        {
            VideoManager.url = url;
            VideoManager.Play();
            SkipToVideoFrame(0);
        }
        VideoUrl = url;
    }


}
