using UnityEngine;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    [SerializeField] private SimulationScript simulationManager;

    [SerializeField] private GameObject shaderHolder;

    [SerializeField] private Button webVersionButton;
    [SerializeField] private Button appVersionButton;

    private void Start()
    {
        webVersionButton.onClick.AddListener(OpenWebVersion);
        appVersionButton.onClick.AddListener(OpenAppVersion);
    }

    private void OpenWebVersion()
    {
        shaderHolder.GetComponent<ComputeShaderSimulation>().enabled = false;
        shaderHolder.GetComponent<FragmentShaderSimulation>().enabled = true;

        simulationManager.Start();

        this.gameObject.SetActive(false);
    }
    private void OpenAppVersion()
    {
        shaderHolder.GetComponent<ComputeShaderSimulation>().enabled = true;
        shaderHolder.GetComponent<FragmentShaderSimulation>().enabled = false;

        simulationManager.Start();

        this.gameObject.SetActive(false);
    }
}
