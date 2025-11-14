using UnityEngine;
using UnityEngine.UI;

public class DrawingMode : MonoBehaviour
{
    [SerializeField] private SimulationScript _simulation;
    private Toggle _toggle;

    private void Start()
    {
        _toggle = this.GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(delegate { OnToggleChanged(); });
    }

    private void OnToggleChanged()
    {
        if (_toggle.isOn)
        {
            _simulation.SetDrawingMode(true);
        }
        else
        {
            _simulation.SetDrawingMode(false);
        }
    }
}
