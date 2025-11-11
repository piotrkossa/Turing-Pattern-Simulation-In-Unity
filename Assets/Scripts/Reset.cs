using UnityEngine;
using UnityEngine.UI;

public class Reset : MonoBehaviour
{
    [SerializeField] private SimulationScript _simulation;

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
    }
    
    private void OnClick()
    {
        _simulation.Reset();
    }
}
