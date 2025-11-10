using UnityEngine;

public static class DialogueActionResolver
{
    public static bool TryResolve(DialogueChoice choice, NpcDefinition npc, out bool success)
    {
        success = false;
        var gm = GameManager.Instance;
        if (gm == null)
            return false;

        var odds = gm.OddsManager;
        if (odds == null)
            return false;

        switch (choice.action)
        {
            case "SEEK_INFO":
                success = Random.value <= choice.successChance;
                if (success)
                {
                    odds.RaiseFighterAOdds(0.2f);
                    Debug.Log("[DialogueAction] SEEK_INFO success, fighter A odds increased.");
                }
                else
                {
                    odds.RaiseFighterBOdds(0.2f);
                    Debug.Log("[DialogueAction] SEEK_INFO fail, fighter B odds increased.");
                }
                return true;

            case "BRIBE":
                success = Random.value <= choice.successChance;
                if (success)
                {
                    odds.RaiseFighterAOdds(0.35f);
                    Debug.Log("[DialogueAction] BRIBE success,  fighter A odds increased.");
                }
                else
                {
                    odds.RaiseFighterBOdds(0.35f);
                    Debug.Log("[DialogueAction] BRIBE fail, fighter B odds increased.");
                }
                return true;

            case "PICKUP_CHIPS":
                if (gm?.Player != null)
                {
                    int coinAmount = Random.Range(10, 30);
                    gm.Player.AddChips(coinAmount);
                    Debug.Log($"[DialogueAction] Player picked up {coinAmount} coins.");
                }
                success = true;
                return true;

            default:
                return false;
        }
    }
}
