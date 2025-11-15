using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    public float SliderValue { get; private set; }

    private UnityEngine.UI.Slider _slider;

    [SerializeField] private TMP_InputField _inputField;

    [Header("Settings")]
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;
    [SerializeField] private bool wholeNumbers;

    private void Start()
    {
        _slider = this.GetComponent<UnityEngine.UI.Slider>();
        _slider.onValueChanged.AddListener(delegate { SliderChanged(); });
        _inputField.onEndEdit.AddListener(delegate { InputFieldChanged(); });

        _slider.wholeNumbers = wholeNumbers;
        _slider.minValue = minValue;
        _slider.maxValue = maxValue;
        UpdatePublicValue();
    }

    private void SliderChanged()
    {
        _inputField.text = _slider.value.ToString();
        UpdatePublicValue();
    }

    private void InputFieldChanged()
    {
        var text = _inputField.text.Replace(',', '.');

        if (float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsed))
        {
            _slider.value = parsed;
        }
        else
        {
            SliderChanged();
        }
        UpdatePublicValue();
    }
    private void UpdatePublicValue()
    {
        SliderValue = _slider.value;
        Debug.Log(SliderValue);
    }
}
