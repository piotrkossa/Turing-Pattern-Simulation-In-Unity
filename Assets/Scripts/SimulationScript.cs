using UnityEngine;
using UnityEngine.UI;

public class SimulationScript : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public RawImage rawImage;


    private void Update()
    {
        if (renderTexture == null)
        {
            CreateRenderTexture();
        }

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        rawImage.texture = renderTexture;
    }


    private void CreateRenderTexture()
    {
        renderTexture = new RenderTexture(256, 256, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
    }
}
