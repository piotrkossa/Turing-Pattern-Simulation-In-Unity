using UnityEngine;

public class ComputeShaderSimulation : MonoBehaviour, ISimulationShader
{
    // our shader
    [SerializeField] private ComputeShader computeShader;

    // 2 buffers to ping-pong between
    private RenderTexture[] buffers;
    private int currentBuffer = 0;

    // our kernels from compute shader
    private int mainKernel;     // main simulation kernel
    private int initKernel;     // clear kernel
    private int seedKernel;     // kernel that spawns initial seed
    private int drawingKernel;  // kernel for drawing mode

    // resolution of simulation
    private int resolution = 512;

    // simulation settings
    public SimulationSettings Settings { get; set; }

    private void Awake()
    {
        // get kernels
        mainKernel = computeShader.FindKernel("CSMain");
        initKernel = computeShader.FindKernel("Initialize");
        seedKernel = computeShader.FindKernel("AddSeed");
        drawingKernel = computeShader.FindKernel("Draw");
    }

    public void Initialize(int resolution, SimulationSettings settings)
    {
        Settings = settings;
        this.resolution = resolution;

        // initialize buffers
        buffers = new RenderTexture[2];

        for (int i = 0; i < 2; i++) {
            buffers[i] = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
            buffers[i].enableRandomWrite = true;
            buffers[i].filterMode = FilterMode.Trilinear;
            buffers[i].Create();
        }

        // set resolution
        computeShader.SetInt("resolution", this.resolution);

        // clear simulation
        Clear();
    }

    public void Clear()
    {
        computeShader.SetTexture(initKernel, "Result", buffers[currentBuffer]);
        computeShader.Dispatch(initKernel, resolution / 8, resolution / 8, 1);
    }

    public void AddSeed(int size)
    {
        computeShader.SetInt("seedSize", size);
        
        computeShader.SetTexture(seedKernel, "Result", buffers[currentBuffer]);
        computeShader.Dispatch(seedKernel, resolution / 8, resolution / 8, 1);
    }

    public void SimulateFrame()
    {
        computeShader.SetFloat("feedRate", Settings.FeedRate);
        computeShader.SetFloat("killRate", Settings.KillRate);
        computeShader.SetFloat("diffusionU", Settings.DiffusionU);
        computeShader.SetFloat("diffusionV", Settings.DiffusionV);
        computeShader.SetFloat("deltaTime", Settings.TimeStep);
        computeShader.SetBool("stopOnWalls", Settings.StopOnWalls);

        for (int i = 0; i < Settings.IterationsPerFrame; i++)
        {
            computeShader.SetTexture(mainKernel, "prevState", buffers[currentBuffer]);
            computeShader.SetTexture(mainKernel, "Result", buffers[1 - currentBuffer]);

            computeShader.Dispatch(mainKernel, resolution / 8, resolution / 8, 1);

            currentBuffer = 1 - currentBuffer;
        }
    }

    public void Draw(Vector2 lastCursorPos, Vector2 cursorPos)
    {
        computeShader.SetVector("lastDrawPosition", new(lastCursorPos.x, lastCursorPos.y, 0, 0));
        computeShader.SetVector("newDrawPosition", new(cursorPos.x, cursorPos.y, 0, 0));

        computeShader.SetTexture(drawingKernel, "Result", buffers[currentBuffer]);
        computeShader.Dispatch(drawingKernel, resolution / 8, resolution / 8, 1);
    }

    public RenderTexture GetCurrentTexture()
    {
        return buffers[currentBuffer];
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