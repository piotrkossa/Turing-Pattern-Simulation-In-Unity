using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScript : MonoBehaviour
{
    private RectTransform thisTransform;

    [Header("Shader")]
    [SerializeField] private MonoBehaviour simulationShaderObject;
    
    private ISimulationShader SimulationShader => (ISimulationShader)simulationShaderObject;
    

    private RawImage rawImage;

    private int Resolution => int.Parse(resolutionDropdown.options[resolutionDropdown.value].text);

    [Header("Setup")]
    [SerializeField] private Slider _feedRateSlider;
    [SerializeField] private Slider _killRateSlider;
    [SerializeField] private Slider _diffusionASlider;
    [SerializeField] private Slider _diffusionBSlider;
    [SerializeField] private Slider _speedSlider;

    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private float FeedRate => _feedRateSlider.SliderValue;
    private float KillRate => _killRateSlider.SliderValue;
    private float DiffusionA => _diffusionASlider.SliderValue;
    private float DiffusionB => _diffusionBSlider.SliderValue;
    private int IterationsPerFrame => (int)_speedSlider.SliderValue;


    private bool stopOnWalls = false;
    private bool drawingMode = false;

    private int GenerateSeedSize(int res)
    {
        return Mathf.RoundToInt(2f * Mathf.Pow(res / 64f, 0.81f)) + 5;
    }

    public void Reset()
    {
        SimulationShader.Clear();
        SimulationShader.AddSeed(GenerateSeedSize(Resolution));

        rawImage.texture = SimulationShader.GetCurrentTexture();
    }

    public void StopOnWalls(bool value)
    {
        stopOnWalls = value;
        SimulationShader.Settings.StopOnWalls = value;
    }

    public void SetDrawingMode(bool value)
    {
        drawingMode = value;
    }

    private void UpdateSettings()
    {
        SimulationShader.Settings.FeedRate = FeedRate;
        SimulationShader.Settings.KillRate = KillRate;
        SimulationShader.Settings.DiffusionU = DiffusionA;
        SimulationShader.Settings.DiffusionV = DiffusionB;
        SimulationShader.Settings.IterationsPerFrame = IterationsPerFrame;
    }

    private void InitShader()
    {
        SimulationShader.Initialize(Resolution, new SimulationSettings(
            feedRate: FeedRate,
            killRate: KillRate,
            iterationsPerFrame: IterationsPerFrame,
            diffusionU: DiffusionA,
            diffusionV: DiffusionB,
            stopOnWalls: stopOnWalls
        ));

        Reset();
    }

    public void Start()
    {
        thisTransform = this.GetComponent<RectTransform>();
        rawImage = this.GetComponent<RawImage>();

        resolutionDropdown.onValueChanged.AddListener(delegate { InitShader(); });

        InitShader();
    }


    private void FixedUpdate()
    {
        UpdateSettings();

        if (drawingMode)
            Draw();

        SimulationShader.SimulateFrame();

        
        rawImage.texture = SimulationShader.GetCurrentTexture();
    }

    private Vector2 lastPosition = Vector2.negativeInfinity;

    private void Draw()
    {
        if (!Input.GetMouseButton(0)) {
            lastPosition = Vector2.negativeInfinity;
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(thisTransform, Input.mousePosition, null, out var localPoint))
        {
            if (!thisTransform.rect.Contains(localPoint)) return;

            Vector2 uv = new Vector2(
                Mathf.InverseLerp(thisTransform.rect.xMin, thisTransform.rect.xMax, localPoint.x),
                Mathf.InverseLerp(thisTransform.rect.yMin, thisTransform.rect.yMax, localPoint.y)
            );

            Vector2 position = new(Mathf.FloorToInt(uv.x * Resolution), Mathf.FloorToInt(uv.y * Resolution));
            
            if (lastPosition != Vector2.negativeInfinity)
                SimulationShader.Draw(lastPosition, position);
            else
                SimulationShader.Draw(position, position);

            lastPosition = position;
        }
    }
}
