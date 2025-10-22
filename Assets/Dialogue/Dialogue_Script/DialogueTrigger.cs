using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private string startNodeId; // e.g. "npc_gingerbread_root"
    [SerializeField] private string jsonFileName = "bigDialogueTree.json";

    private bool playerInRange = false;

    private void Start()
    {
        dialogueManager.LoadTreeFromFile(jsonFileName);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            dialogueManager.StartDialogueAt(startNodeId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
