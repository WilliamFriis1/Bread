using UnityEngine;
public abstract class ConditionSO : ScriptableObject
{
    public abstract bool Evaluate(GameContext gameContext, CharacterRuntime character);
}
public abstract class EffectSO : ScriptableObject
{
    public abstract void Apply(GameContext gameContext, CharacterRuntime character);
}

[System.Serializable]
public struct DialogueChoice
{
    [Tooltip("Shown on the button")]
    public string label;

    [Tooltip("Optional conditions to show/enable this choice")]
    public ConditionSO[] conditions;

    [Tooltip("Effects to apply after selecting this choice (state updates, flags, etc.)")]
    public EffectSO[] effects;

    [Tooltip("Next dialogue node id or special routing key")] //Might not be used depending on how we do the dialogue system.
    public string nextNodeId;
}
