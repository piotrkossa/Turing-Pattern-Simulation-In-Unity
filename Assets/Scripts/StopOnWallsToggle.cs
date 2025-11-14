using UnityEngine;
using UnityEngine.UI;

public class StopOnWallsToggle : MonoBehaviour
{
    [SerializeField] private SimulationScript _simulation;
    private Toggle _toggle;

    private void Start()
    {
        _toggle = this.GetComponent<Toggle>();

        _toggle.onValueChanged.AddListener(delegate { ValueChanged(); });
    }

    private void ValueChanged()
    {
        var value = _toggle.isOn;

        _simulation.StopOnWalls(value);
    }
}
