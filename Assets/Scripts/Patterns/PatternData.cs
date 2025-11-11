using UnityEngine;


[CreateAssetMenu(fileName = "ChangeMeGang", menuName = "Patterns/PatternBase", order = 1)]
public class PatternData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public float FeedRate { get; private set; }
    [field: SerializeField] public float KillRate { get; private set; }
    [field: SerializeField] public float DiffusionU { get; private set; } = 1;
    [field: SerializeField] public float DiffusionV { get; private set; } = 0.5f;
    [field: SerializeField] public float TimeStep { get; private set; } = 1;
}
