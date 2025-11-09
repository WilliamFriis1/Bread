using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    [SerializeField] private CharacterSlot slot;
    [SerializeField] private NpcRandomizerTest randomizer;

    public void OnNextPhasePressed()
    {
        GameManager.Instance.MoveToNextPhase();
        if (GameManager.Instance.Phase == GameManager.GamePhase.SpeakingToNPC)
        {
            var def = randomizer.PickDefinition();
            slot.Present(def);
        }
    }
}
