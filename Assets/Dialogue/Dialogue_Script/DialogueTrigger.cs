using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue File Info")]
    [SerializeField] private string dialogueFile = "npc_gingerbread1.json";
    [SerializeField] private string startNodeId = "npc_gingerbread1_root";

    [Header("Manager Reference")]
    [SerializeField] private DialogueManager dialogueManager;

    private bool playerInRange;

    private void Start()
    {
        if (dialogueManager == null)
            return;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueManager == null)
            {
                Debug.LogError("DialogueManager not found in scene!");
                return;
            }

            Debug.Log($"Starting dialogue from file: {dialogueFile} at node: {startNodeId}");
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
