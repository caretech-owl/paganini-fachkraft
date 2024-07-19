using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GradientFill : MonoBehaviour
{
    public Gradient gradient;
    private Texture2D gradientTexture;

    void Start()
    {
        gradientTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
        for (int i = 0; i < 256; i++)
        {
            gradientTexture.SetPixel(i, 0, gradient.Evaluate(i / 255f));
        }
        gradientTexture.Apply();

        GetComponent<MeshRenderer>().material.mainTexture = gradientTexture;
    }

    void OnDestroy()
    {
        Destroy(gradientTexture);
    }
}
