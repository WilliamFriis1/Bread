using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSlot : MonoBehaviour
{
    [Header("Curtains")]
    [SerializeField] CurtainsController curtains;
    [Header("Dialogue")]
    [SerializeField] DialogueManager dialogueManager;
    [Header("Stage")]
    [SerializeField] Transform characterRoot;
    [SerializeField] SpriteRenderer characterRenderer;
    [Header("Refs")]
    [SerializeField] private EncounterDirector encounterDirector;

    NpcDefinition activeDef;
    public void Present(NpcDefinition def, string startNodeId = "")
    {
        StartCoroutine(DoPresent(def, startNodeId));
    }
    IEnumerator DoPresent(NpcDefinition def, string nodeId)
    {
        if (curtains) yield return curtains.Close();

        activeDef = def;
        if (characterRenderer)
        {
            characterRenderer.sprite = def.characterSprite;
            characterRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        if (curtains)
        {
            yield return curtains.Open();
        }
        if (dialogueManager != null)
        {
            dialogueManager.DialogueEnded -= OnDialogueEnded; // avoid double-subscribe
            dialogueManager.DialogueEnded += OnDialogueEnded;
            //Kick that shit off (dialogue)
            dialogueManager.LoadTreeFromFile(def.dialogueFile);
            dialogueManager.SetNpc(def);
            dialogueManager.SetOwningNpc(null);

            if (!string.IsNullOrWhiteSpace(nodeId))
            {
                dialogueManager.StartDialogueAt(nodeId);
            }
            else
            {
                dialogueManager.StartFromTreeStart();
            }
        }
    }
    private void OnDialogueEnded()
    {
        if (dialogueManager != null)
        {
            dialogueManager.DialogueEnded -= OnDialogueEnded;
        }
        CloseCurtainsNow();
        encounterDirector?.FinishNpcAndEvent();
    }
    public void CloseCurtainsNow()
    {
        if (gameObject.activeInHierarchy && curtains)
        {
            StartCoroutine(curtains.Close());
        }
    }
}
