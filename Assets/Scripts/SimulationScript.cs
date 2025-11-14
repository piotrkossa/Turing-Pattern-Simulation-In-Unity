using UnityEngine;
using UnityEngine.UI;

public class SimulationScript : MonoBehaviour
{
    private RectTransform thisTransform;

    [Header("Setup")]
    public ComputeShader computeShader;

    public RawImage rawImage;

    public RenderTexture[] buffers;
    private int currentBuffer = 0;

    private int mainKernel;
    private int initKernel;
    private int seedKernel;
    private int drawingKernel;

    private const int resolution = 512;

    // variables to update each frame
    private float feedRate;
    private float killRate;
    private float diffusionA = 1.0f;
    private float diffusionB = 0.5f;
    private int iterationsPerFrame = 0;
    private bool stopOnWalls = false;
    private bool drawingMode = false;


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

    public void StopOnWalls(bool value)
    {
        stopOnWalls = value;
    }

    public void SetDrawingMode(bool value)
    {
        drawingMode = value;
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
        thisTransform = this.GetComponent<RectTransform>();

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
        drawingKernel = computeShader.FindKernel("Draw");
        
        computeShader.SetInt("resolution", resolution);

        Reset();
    }


    private void FixedUpdate()
    {
        UpdateSettings();

        computeShader.SetFloat("feedRate", feedRate);
        computeShader.SetFloat("killRate", killRate);
        computeShader.SetFloat("diffusionU", diffusionA);
        computeShader.SetFloat("diffusionV", diffusionB);
        computeShader.SetFloat("deltaTime", timeStep);
        computeShader.SetBool("stopOnWalls", stopOnWalls);


        if (drawingMode)
            Draw();

        for (int i = 0; i < iterationsPerFrame; i++)
        {
            computeShader.SetTexture(mainKernel, "prevState", buffers[currentBuffer]);
            computeShader.SetTexture(mainKernel, "Result", buffers[1 - currentBuffer]);

            computeShader.Dispatch(mainKernel, resolution / 8, resolution / 8, 1);

            currentBuffer = 1 - currentBuffer;
        }

        
        rawImage.texture = buffers[currentBuffer];
    }

    private void Draw()
    {
        if (!Input.GetMouseButton(0)) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(thisTransform, Input.mousePosition, null, out var localPoint))
        {
            if (!thisTransform.rect.Contains(localPoint)) return;

            Vector2 uv = new Vector2(
                Mathf.InverseLerp(thisTransform.rect.xMin, thisTransform.rect.xMax, localPoint.x),
                Mathf.InverseLerp(thisTransform.rect.yMin, thisTransform.rect.yMax, localPoint.y)
            );

            computeShader.SetInt("pointToDrawX", Mathf.FloorToInt(uv.x * resolution));
            computeShader.SetInt("pointToDrawY", Mathf.FloorToInt(uv.y * resolution));

            computeShader.SetTexture(drawingKernel, "Result", buffers[currentBuffer]);
            computeShader.Dispatch(drawingKernel, resolution / 8, resolution / 8, 1);
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
