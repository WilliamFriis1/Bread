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

    private DialogueTree currentTree;
    private DialogueNode currentNode;

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
        Debug.Log($"Dialogue tree loaded from: {filename}");
    }

    public void StartDialogueAt(string nodeId)
    {
        if (currentTree == null)
        {
            Debug.LogError("No dialogue tree loaded!");
            return;
        }

        dialoguePanel.SetActive(true);
        ShowNode(currentTree.GetNode(nodeId));
    }

    private void ShowNode(DialogueNode node)
    {
        if (node == null)
        {
            Debug.LogError("Dialogue node not found!");
            return;
        }

        currentNode = node;
        speakerText.text = node.speaker;
        dialogueText.text = node.text;

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (DialogueChoice choice in node.choices)
        {
            GameObject button = Instantiate(choiceButtonPrefab, choiceContainer);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = choice.text;

            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (!string.IsNullOrEmpty(choice.action))
        {
            Debug.Log("Action triggered: " + choice.action);
            bool success = Random.value <= choice.successChance;

            string nextNode = success ? choice.gotoOnSuccess : choice.gotoOnFail;
            DialogueNode next = currentTree.GetNode(nextNode);

            if (next != null)
                ShowNode(next);
            else
                EndDialogue();
        }
        else if (!string.IsNullOrEmpty(choice.gotoNode))
        {
            DialogueNode next = currentTree.GetNode(choice.gotoNode);
            if (next != null)
                ShowNode(next);
            else
                EndDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentNode = null;
    }
}
