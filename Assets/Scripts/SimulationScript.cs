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

    private const int resolution = 512;

    // variables to update each frame
    private float feedRate;
    private float killRate;
    private float diffusionA = 1.0f;
    private float diffusionB = 0.5f;
    private int iterationsPerFrame = 0;


    [SerializeField] private Slider _feedRateSlider;
    [SerializeField] private Slider _killRateSlider;
    [SerializeField] private Slider _diffusionASlider;
    [SerializeField] private Slider _diffusionBSlider;
    [SerializeField] private Slider _speedSlider;


    public void Reset()
    {
        computeShader.SetTexture(initKernel, "Result", buffers[0]);
        computeShader.Dispatch(initKernel, resolution / 8, resolution / 8, 1);
        
        computeShader.SetTexture(seedKernel, "Result", buffers[0]);
        computeShader.Dispatch(seedKernel, resolution / 8, resolution / 8, 1);

        currentBuffer = 0;

        rawImage.texture = buffers[currentBuffer];
    }


    private float timeStep = 1.0f; // private for now

    private void UpdateSettings()
    {
        feedRate = _feedRateSlider.SliderValue;
        killRate = _killRateSlider.SliderValue;
        diffusionA = _diffusionASlider.SliderValue;
        diffusionB = _diffusionBSlider.SliderValue;
        iterationsPerFrame = (int)_speedSlider.SliderValue;
    }

    private void Start()
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

        Reset();
    }


    private void FixedUpdate()
    {
        UpdateSettings();

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
