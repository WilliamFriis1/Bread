using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;
    [Header("Context")]
    [SerializeField] private NpcDefinition npc;
    private TestNPC owningNpc;

    private DialogueTree currentTree;
    private DialogueNode currentNode;

    private Dictionary<string, DialogueNode> byId;

    public void LoadTreeFromFile(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", filename);

        if (!File.Exists(path))
        {
            Debug.LogError("Dialogue file not found at: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        currentTree = DialogueTree.FromJson(json);
        BuildIndex(currentTree);
        Debug.Log($"Dialogue tree loaded from: {filename}");
    }

    public void StartDialogueAt(string nodeId)
    {
        if (currentTree == null)
        {
            Debug.LogError("No dialogue tree loaded!");
            return;
        }
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.Phase = GameManager.GamePhase.SpeakingToNPC;
        }
        dialoguePanel.SetActive(true);
        ShowNode(currentTree.GetNode(nodeId));
    }

    private void ShowNode(DialogueNode node)
    {
        if (node == null)
        {
            Debug.LogError("Dialogue node not found!");
            EndDialogue();
            return;
        }

        currentNode = node;
        speakerText.text = node.speaker;
        dialogueText.text = node.text;

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
        if (node.choices != null && node.choices.Count > 0)
        {
            foreach (DialogueChoice choice in node.choices)
            {
                GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = choice.text;

                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        //Pref resolver for actioned choices (SEEK_INFO / BRIBE_ / BUY)
        if (!string.IsNullOrEmpty(choice.action))
        {
            bool handled = DialogueActionResolver.TryResolve(choice, npc, out bool success);
            if (handled)
            {
                string nextId = success ? choice.gotoOnSuccess : choice.gotoOnFail;
                var next = GetNode(nextId);
                if (next != null) { ShowNode(next); return; }
                EndDialogue(); return;
            }

            // Unknown action? fall back to your old simple roll:
            Debug.Log($"[Dialogue] Unhandled action '{choice.action}' — using fallback roll.");
            bool fallback = Random.value <= choice.successChance;
            string nextFallback = fallback ? choice.gotoOnSuccess : choice.gotoOnFail;
            var nextNodeFallback = GetNode(nextFallback);
            if (nextNodeFallback != null) { ShowNode(nextNodeFallback); return; }
            EndDialogue(); return;
        }

        //Plain goto
        if (!string.IsNullOrEmpty(choice.gotoNode))
        {
            var next = GetNode(choice.gotoNode);
            if (next != null) { ShowNode(next); return; }
            EndDialogue(); return;
        }

        // nav => end
        EndDialogue();


        // //mostly for testing, but works with the game as well
        // if (!string.IsNullOrEmpty(choice.action))
        // {
        //     Debug.Log("Action triggered: " + choice.action);
        //     bool success = Random.value <= choice.successChance;

        //     string nextNode = success ? choice.gotoOnSuccess : choice.gotoOnFail;
        //     DialogueNode next = currentTree.GetNode(nextNode);

        //     if (next != null)
        //         ShowNode(next);
        //     else
        //         EndDialogue();
        // }
        // else if (!string.IsNullOrEmpty(choice.gotoNode))
        // {
        //     DialogueNode next = currentTree.GetNode(choice.gotoNode);
        //     if (next != null)
        //         ShowNode(next);
        //     else
        //         EndDialogue();
        // }
        // else
        // {
        //     EndDialogue();
        // }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentNode = null;

        //back to loop
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.MoveToNextPhase();
        }
        owningNpc?.OnEnd?.Invoke();
    }

    //NEW added by el Gustaf
    private void BuildIndex(DialogueTree t)
    {
        byId = new Dictionary<string, DialogueNode>();
        if (t?.nodes == null)
        {
            return;
        }
        foreach (var n in t.nodes)
        {
            if (!string.IsNullOrEmpty(n.id))
            {
                byId[n.id] = n;
            }
        }
    }
    public void SetNpc(NpcDefinition def) => npc = def;
    public void SetOwningNpc(TestNPC testNpc) => owningNpc = testNpc;
    private DialogueNode GetNode(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (byId != null && byId.TryGetValue(id, out var n)) return n;
        // Fallback to the tree's own lookup if you have one
        return currentTree.GetNode(id);
    }
    public void StartFromTreeStart()
    {
        if (currentTree?.start != null && currentTree.start.Count > 0)
        {
            StartDialogueAt(currentTree.start[0]);
        }
        else
        {
            Debug.LogError("[DialogueManager] No start node defined in dialogue tree.");
        }
    }
}
