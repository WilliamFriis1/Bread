using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private DialogueTree currentTree;
    private DialogueNode currentNode;
    private System.Random rng = new System.Random();

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    // Load the entire big dialogue tree JSON from StreamingAssets and start from a node id
    public void LoadTreeFromFile(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        if (!File.Exists(path))
        {
            Debug.LogError("Dialogue file not found at: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        currentTree = DialogueTree.FromJson(json);
    }

    public void StartDialogueAt(string startNodeId)
    {
        if (currentTree == null)
        {
            Debug.LogError("Call LoadTreeFromFile(...) before starting dialogue.");
            return;
        }

        if (!currentTree.nodes.ContainsKey(startNodeId))
        {
            Debug.LogError($"Start node '{startNodeId}' not found in tree.");
            return;
        }

        ShowNode(startNodeId);
        dialoguePanel.SetActive(true);
    }

    private void ClearChoices()
    {
        foreach (Transform t in choicesContainer) Destroy(t.gameObject);
    }

    private void ShowNode(string nodeId)
    {
        ClearChoices();

        if (!currentTree.nodes.TryGetValue(nodeId, out currentNode))
        {
            Debug.LogError("Node not found: " + nodeId);
            EndDialogue();
            return;
        }

        speakerText.text = string.IsNullOrEmpty(currentNode.speaker) ? "" : currentNode.speaker;
        dialogueText.text = currentNode.text;

        if (currentNode.end)
        {
            var endBtn = Instantiate(choiceButtonPrefab, choicesContainer);
            endBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
            endBtn.onClick.AddListener(EndDialogue);
            return;
        }

        if (currentNode.choices == null || currentNode.choices.Count == 0)
        {
            var okBtn = Instantiate(choiceButtonPrefab, choicesContainer);
            okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            okBtn.onClick.AddListener(EndDialogue);
            return;
        }

        foreach (var ch in currentNode.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.text = ch.text;

            // Capture loop variable
            DialogueChoice capturedChoice = ch;

            btn.onClick.AddListener(() =>
            {
                ProcessChoice(capturedChoice);
            });
        }
    }

    private void ProcessChoice(DialogueChoice choice)
    {
        // Choice that has an action with success/fail routing:
        if (!string.IsNullOrEmpty(choice.action))
        {
            float chance = Mathf.Clamp01(choice.successChance);
            float roll = (float)rng.NextDouble();
            bool success = roll <= chance;

            if (success && !string.IsNullOrEmpty(choice.gotoOnSuccess))
            {
                ShowNode(choice.gotoOnSuccess);
                return;
            }
            else if (!success && !string.IsNullOrEmpty(choice.gotoOnFail))
            {
                ShowNode(choice.gotoOnFail);
                return;
            }

        }

        if (!string.IsNullOrEmpty(choice.gotoNode))
        {
            ShowNode(choice.gotoNode);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
