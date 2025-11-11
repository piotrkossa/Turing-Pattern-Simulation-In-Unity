using UnityEngine;
using UnityEngine.UI;

public class SimulationScript : MonoBehaviour
{
    [Header("Setup")]
    public ComputeShader computeShader;

    public RawImage rawImage;

    public RenderTexture[] buffers;
    private int currentBuffer = 0;

    private int mainKernel;
    private int initKernel;
    private int seedKernel;

    [Header("Simulation Parameters")]
    public int resolution = 512;
    [Range(0f, 0.1f)] public float feedRate = 0.0545f;
    [Range(0f, 0.1f)] public float killRate = 0.062f;
    [Range(0f, 2f)] public float diffusionA = 1.0f;
    [Range(0f, 2f)] public float diffusionB = 0.5f;
    private float timeStep = 1.0f; // private for now
    [Range(0, 300)] public int iterationsPerFrame = 0;


    void Start()
    {
        buffers = new RenderTexture[2];
        for (int i = 0; i < 2; i++)
        {
            buffers[i] = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RGFloat);
            buffers[i].enableRandomWrite = true;
            buffers[i].Create();
        }

        mainKernel = computeShader.FindKernel("CSMain");
        initKernel = computeShader.FindKernel("Initialize");
        seedKernel = computeShader.FindKernel("AddSeed");
        
        computeShader.SetInt("resolution", resolution);
        
        computeShader.SetTexture(initKernel, "Result", buffers[0]);
        computeShader.Dispatch(initKernel, resolution / 8, resolution / 8, 1);
        
        computeShader.SetTexture(seedKernel, "Result", buffers[0]);
        computeShader.Dispatch(seedKernel, resolution / 8, resolution / 8, 1);

        currentBuffer = 0;

        rawImage.texture = buffers[currentBuffer];
    }


    private void FixedUpdate()
    {
        for (int i = 0; i < iterationsPerFrame; i++)
        {
            computeShader.SetFloat("feedRate", feedRate);
            computeShader.SetFloat("killRate", killRate);
            computeShader.SetFloat("diffusionU", diffusionA);
            computeShader.SetFloat("diffusionV", diffusionB);
            computeShader.SetFloat("deltaTime", timeStep);

            computeShader.SetTexture(mainKernel, "prevState", buffers[currentBuffer]);
            computeShader.SetTexture(mainKernel, "Result", buffers[1 - currentBuffer]);

            computeShader.Dispatch(mainKernel, resolution / 8, resolution / 8, 1);

            currentBuffer = 1 - currentBuffer;

            rawImage.texture = buffers[currentBuffer];
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 2; i++)
        {
            if (buffers != null && buffers[i] != null)
            {
                buffers[i].Release();
            }
        }
    }
}
