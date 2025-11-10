using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    [SerializeField] private CharacterSlot slot;
    [SerializeField] private Randomizer randomizer;
    [SerializeField] private EventRunner eventRunner;
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
        if (ev != null && ev.archetype == NpcArchetype.Event)
        {
            if (eventRunner != null)
            {
                eventRunner.Execute(ev);
            }
            else
            {
                Debug.Log("No eventrunner assigned. Event skipped");
            }
        }
        StartNpcEncounter();
    }
}
public class EventRunner : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    public void Execute(NpcDefinition ev)
    {
        if (ev == null || ev.archetype != NpcArchetype.Event) return;
        if (gm == null) return;

        var player = gm.Player;
        var odds = gm.OddsManager;

        switch (ev.eventKind)
        {
            case EventKind.MoneyDelta:
                if (player != null) player.AddChips(ev.moneyDelta); // negative lowers money
                break;

            case EventKind.GiveItem:
                // Single-slot “flour”
                if (player != null && (string.IsNullOrEmpty(ev.itemId) || ev.itemId == "flour"))
                    player.GiveFlourFree();
                break;

            case EventKind.TakeItem:
                if (player != null && (string.IsNullOrEmpty(ev.itemId) || ev.itemId == "flour"))
                    player.TakeFlourIfAny();
                break;

            case EventKind.OddsDelta:
                if (odds != null) ApplyOddsDelta(odds, ev.oddsDelta);
                break;

            case EventKind.InfoPopup:
                if (!string.IsNullOrEmpty(ev.infoMessage))
                    Debug.Log($"[Event Info] {ev.infoMessage}");
                break;
        }
    }
    private void ApplyOddsDelta(OddsManager odds, float delta)
    {
        if (delta > 0f) odds.RaiseFighterAOdds(delta);
        else if (delta < 0f) odds.RaiseFighterBOdds(-delta);
        // delta == 0 -> no change
    }
}
