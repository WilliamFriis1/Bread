using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string dialogueFile = "bigDialogueTree.json";
    [SerializeField] private string startNodeId = "npc_gingerbread_root";
    [SerializeField] private DialogueManager dialogueManager;

    private bool playerInRange = false;

    private void Start()
    {
        if (dialogueManager == null)
            return;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Starting dialogue: {startNodeId}");
            dialogueManager.LoadTreeFromFile(dialogueFile);
            dialogueManager.StartDialogueAt(startNodeId);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
