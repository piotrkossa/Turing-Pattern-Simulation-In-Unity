using UnityEngine;

public interface ISimulationShader
{
    void Initialize(int resolution, SimulationSettings settings);
    void Clear();
    void AddSeed();
    void SimulateFrame();

    void Draw(Vector2 lastCursorPos, Vector2 cursorPos);

    SimulationSettings Settings {get; set;}

    RenderTexture GetCurrentTexture();
}

public class SimulationSettings
{
    public SimulationSettings(
        float feedRate, 
        float killRate, 
        int iterationsPerFrame = 1, 
        float diffusionU = 1.0f, 
        float diffusionV = 0.5f, 
        bool stopOnWalls = false,
        float timeStep = 1.0f)
    {
        FeedRate = feedRate;
        KillRate = killRate;
        DiffusionU = diffusionU;
        DiffusionV = diffusionV;
        IterationsPerFrame = iterationsPerFrame;
        TimeStep = timeStep;
        StopOnWalls = stopOnWalls;
    }

    public float FeedRate { get; set; }
    public float KillRate { get; set; }
    public float DiffusionU { get; set; }
    public float DiffusionV { get; set; }
    public int IterationsPerFrame { get; set; }

    public bool StopOnWalls { get; set; }

    public float TimeStep { get; set; }
}