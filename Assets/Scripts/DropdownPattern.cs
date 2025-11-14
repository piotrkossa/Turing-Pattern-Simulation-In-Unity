using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPattern : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider _feedRateSlider;
    [SerializeField] private UnityEngine.UI.Slider _killRateSlider;
    [SerializeField] private UnityEngine.UI.Slider _diffusionASlider;
    [SerializeField] private UnityEngine.UI.Slider _diffusionBSlider;

    // mitosis, solitions, mazes, flower
    [SerializeField] private PatternData[] patterns;


    private TMP_Dropdown _dropdown;

    private void Start()
    {
        _dropdown = this.GetComponent<TMP_Dropdown>();

        var options = new List<string>();

        foreach (var pattern in patterns)
        {
            options.Add(pattern.Name);
        }

        _dropdown.ClearOptions();

        _dropdown.AddOptions(options);

        _dropdown.onValueChanged.AddListener(delegate { ValueChanged(); });

        _feedRateSlider.onValueChanged.AddListener(delegate { SetPlaceHolder(); });
        _killRateSlider.onValueChanged.AddListener(delegate { SetPlaceHolder(); });
        _diffusionASlider.onValueChanged.AddListener(delegate { SetPlaceHolder(); });
        _diffusionBSlider.onValueChanged.AddListener(delegate { SetPlaceHolder(); });
    }

    private void SetPlaceHolder()
    {
        _dropdown.SetValueWithoutNotify(-1);
    }

    private void ValueChanged()
    {
        int value = _dropdown.value;
        PatternData pattern = patterns[value];

        _feedRateSlider.value = pattern.FeedRate;
        _killRateSlider.value = pattern.KillRate;
        _diffusionASlider.value = pattern.DiffusionU;
        _diffusionBSlider.value = pattern.DiffusionV;
    }
}
