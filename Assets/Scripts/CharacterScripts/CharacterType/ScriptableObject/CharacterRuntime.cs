using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using NUnit.Framework.Internal;

public class CharacterRuntime : MonoBehaviour
{
    public CharacterDefinition definition;
    public IEnumerable<DialogueChoice> GetAvailableChoices(GameContext gameContext)
    {
        var list = new List<DialogueChoice>(4);
        list.Add(definition.overrideBaseChoices ? 
            definition.basicDialogueOverride : 
            definition.type.basicDialogue);
        //
        list.Add(definition.overrideBaseChoices ?
            definition.askInfoOverride :
            definition.type.askInfoAboutFighter);
        //
        list.Add(definition.overrideBaseChoices ?
            definition.endConversationOverride :
            definition.type.endConversation);
        //
        var action = definition.type.specialAction;
        if(action != null && action.IsAvailable(gameContext, this))
        {
            list.Add(new DialogueChoice
            {
                label = action.actionLabel,
                conditions = action.availability,
                effects = action.onPerform,
                nextNodeId = null
            });
        }
        return list;
    }
    List<DialogueChoice> FilterByConditions(DialogueChoice[] choices, GameContext gameContext)
    {
        var outList = new List<DialogueChoice>();
        foreach (var c in choices)
        {
            bool ok = true;
            if (c.conditions != null)
            {
                foreach(var cond in c.conditions)
                {
                    if(cond != null&& !cond.Evaluate(gameContext, this))
                    {
                        ok = false;
                        break;
                    }
                }
            }
        }
        return outList;
    }
    public void SelectChoice(GameContext gameContext, DialogueChoice choice)
    {
        if (choice.effects != null)
        {
            foreach(var e in choice.effects)
            {
                if (e != null)
                {
                    e.Apply(gameContext, this);
                }
            }
        }
        if(definition.type.specialAction!=null&&choice.label == definition.type.specialAction.actionLabel)
        {
            definition.type.specialAction.Perform(gameContext, this);
        }
    }
}
