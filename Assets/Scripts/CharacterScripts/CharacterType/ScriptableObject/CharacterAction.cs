using Mono.Cecil.Cil;
using UnityEditor.EditorTools;
using UnityEngine;

public class CharacterAction : ScriptableObject
{
    public string actionLabel = "Special action";
    public ConditionSO[] availability;
    public EffectSO[] onPerform;

    public virtual bool IsAvailable(GameContext gameContext, CharacterRuntime character)
    {
        if (availability == null) return true;
        foreach(var c in availability)
            if(c != null && !c.Evaluate(gameContext, character)) return false;
        return true;
    }
    public virtual void Perform(GameContext gameContext, CharacterRuntime character)
    {
        if (onPerform == null) return;
        foreach (var e in onPerform)
        {
            if (e != null)
            {
                e.Apply(gameContext, character);
            }
        }
    }
}
