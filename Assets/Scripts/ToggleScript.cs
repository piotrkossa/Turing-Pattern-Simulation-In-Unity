using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] private RawImage _simulationImage;

    [SerializeField] private Material _blackWhiteMaterial;
    [SerializeField] private Material _lsdMaterial;

    private void Start()
    {
        _toggle = this.GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(delegate { OnToggleChanged(); });
        _toggle.isOn = false;
        _simulationImage.material = _blackWhiteMaterial;
    }

    private void OnToggleChanged()
    {
        if (_toggle.isOn)
        {
            _simulationImage.material = _lsdMaterial;
        }
        else
        {
            _simulationImage.material = _blackWhiteMaterial;
        }
    }
}
