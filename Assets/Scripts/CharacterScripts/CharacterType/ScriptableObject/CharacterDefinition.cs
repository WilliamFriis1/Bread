using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public string characterId;
    public string displayName;
    public Sprite characterSprite; //Instead of a texture2D for easier use when integrating it into the UI later down the line.
    public CharacterArchetype type;

    [Tooltip("Optional overrides of the three base choices")]
    public bool overrideBaseChoices;
    public DialogueChoice basicDialogueOverride;
    public DialogueChoice askInfoOverride;
    public DialogueChoice endConversationOverride;

    [Tooltip("Character-specific extra choices")] //THIS IS STRICTLY OPTIONAL, we can remove it later on if it doesn't fit with the interaction (dialogue system)
    public DialogueChoice[] characterExtraChoices;
}
