using System.Linq;
using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    [SerializeField] private CharacterSlot slot;
    [SerializeField] private Randomizer randomizer;
    [SerializeField] private EventRunner eventRunner;
    [ContextMenu("Print the NPC queue (preview)")]
    public void PrintQueuePreview()
    {
        if (randomizer == null)
        {
            Debug.LogWarning("EncounterDirector: Randomizer ref is missing.");
            return;
        }

        var arr = randomizer.NpcOrder.ToArray(); // safe: does NOT dequeue
        if (arr.Length == 0)
        {
            Debug.Log("[Randomizer] Queue is EMPTY. Did you call SetNpcs() yet?");
            return;
        }

        string Label(NpcDefinition n) =>
            n == null ? "<null>" :
            !string.IsNullOrWhiteSpace(n.displayName) ? n.displayName :
            !string.IsNullOrWhiteSpace(n.npcId) ? n.npcId : n.name;

        var names = arr.Select(Label);
        Debug.Log("[Randomizer] Today's NPC queue:\n - " + string.Join("\n - ", names));
    }
    public void StartNpcEncounter()
    {
        if (slot == null || randomizer == null)
        {
            Debug.Log("Shit's not working, type shi (aka,missing refs)");
            return;
        }
        var def = randomizer.GetNextNpc();

        if (def == null)
        {
            Debug.Log("All npc's spawned for the day");
            return;
        }
        slot.Present(def); // idk man, you just get that shit working. 
    }
    public void FinishNpcAndEvent()
    {
        var ev = randomizer.GetRandomEvent();
        if (ev is EventNpcAdapter && eventRunner != null)
        {
            eventRunner.Execute(ev);
        }
        else if (ev is EventNpcAdapter == false && ev != null)
        {
            Debug.Log("Random event returned a non event asset, so it skipped");
        }
        StartNpcEncounter();
    }

}
