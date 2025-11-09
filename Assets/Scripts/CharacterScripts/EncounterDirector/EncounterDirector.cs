using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    [SerializeField] private CharacterSlot slot;
    [SerializeField] private NpcRandomizer randomizer;
    public void StartNpcEncounter()
    {
        if (slot == null || randomizer == null)
        {
            Debug.Log("Shit's not working, type shi (aka,missing refs)");
            return;
        }
        randomizer.AdvanceRound();

        var def = randomizer.PickDefinition();
        if (def == null)
        {
            Debug.Log("NO NPC DEFINITION FOUND, FUUUUCK");
            return;
        }
        slot.Present(def); // idk man, you just get that shit working. 
    }
}
