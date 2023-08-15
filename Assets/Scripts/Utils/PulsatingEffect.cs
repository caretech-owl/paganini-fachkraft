using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

public class PulsatingEffect : MonoBehaviour
{
    public float pulseSpeed = 1.0f;
    public float minAlpha = 0.0f;
    public float maxAlpha = 1.0f;

    private SVGImage image;
    private CanvasRenderer canvasRenderer;
    private bool increasing = true;

    private void Start()
    {
        image = GetComponent<SVGImage>();
        canvasRenderer = GetComponent<CanvasRenderer>();
    }

    private void Update()
    {
        if (image != null && canvasRenderer != null)
        {
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
            alpha = Mathf.Lerp(minAlpha, maxAlpha, alpha);

            Color color = image.color;
            color.a = alpha;

            image.color = color;
            canvasRenderer.SetAlpha(color.a);
        }
    }
}
