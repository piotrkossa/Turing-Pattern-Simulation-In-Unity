using Unity.VisualScripting;
using UnityEngine;

public class FragmentShaderSimulation : MonoBehaviour, ISimulationShader
{
    // shaders for different operations
    [SerializeField] private Shader computeShader;
    [SerializeField] private Shader seedShader;
    [SerializeField] private Shader initShader;
    [SerializeField] private Shader drawingShader;

    // our tools to utilise shaders
    private Material computeMaterial;
    private Material seedMaterial;
    private Material initMaterial;
    private Material drawingMaterial;



    // ping pong buffers
    private RenderTexture[] buffers;
    private int currentBuffer = 0;

    private int resolution = 512;

    public SimulationSettings Settings { get; set; }

    private void Awake()
    {
        computeMaterial = new Material(computeShader);
        seedMaterial = new Material(seedShader);
        initMaterial = new Material(initShader);
        drawingMaterial = new Material(drawingShader);
    }

    public void Initialize(int resolution, SimulationSettings settings)
    {
        Settings = settings;
        this.resolution = resolution;

        // initialize buffers
        buffers = new RenderTexture[2];

        for (int i = 0; i < 2; i++) {
            buffers[i] = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
            buffers[i].wrapMode = TextureWrapMode.Repeat;
            buffers[i].filterMode = FilterMode.Trilinear;
            buffers[i].Create();
        }

        // set resolution
        computeMaterial.SetFloat("_Resolution", this.resolution);

        // clear simulation
        Clear();
    }

    public void Clear()
    {
        Graphics.Blit(null, buffers[currentBuffer], initMaterial); 
    }

    public void AddSeed(int size)
    {
        seedMaterial.SetInt("_SeedSize", size);
        seedMaterial.SetInt("_Resolution", resolution);

        Graphics.Blit(buffers[currentBuffer], buffers[1 - currentBuffer], seedMaterial);
        currentBuffer = 1 - currentBuffer;
    }

    public void SimulateFrame()
    {
        computeMaterial.SetFloat("_FeedRate", Settings.FeedRate);
        computeMaterial.SetFloat("_KillRate", Settings.KillRate);
        computeMaterial.SetFloat("_DiffusionU", Settings.DiffusionU);
        computeMaterial.SetFloat("_DiffusionV", Settings.DiffusionV);
        computeMaterial.SetFloat("_DeltaTime", Settings.TimeStep);
        computeMaterial.SetInt("_StopOnWalls", Settings.StopOnWalls ? 1 : 0);

        for (int i = 0; i < Settings.IterationsPerFrame; i++)
        {
            Graphics.Blit(buffers[currentBuffer], buffers[1 - currentBuffer], computeMaterial);
            currentBuffer = 1 - currentBuffer;
        }
    }

    public void Draw(Vector2 lastCursorPos, Vector2 cursorPos)
    {
        drawingMaterial.SetFloat("_Resolution", resolution);
        drawingMaterial.SetVector("_LastDrawPosition", new Vector4(lastCursorPos.x, lastCursorPos.y, 0, 0));
        drawingMaterial.SetVector("_NewDrawPosition", new Vector4(cursorPos.x, cursorPos.y, 0, 0));

        Graphics.Blit(buffers[currentBuffer], buffers[1 - currentBuffer], drawingMaterial);
        currentBuffer = 1 - currentBuffer;
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
