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
                    odds.RaiseFighterAOdds(0.05f);
                    Debug.Log("[DialogueAction] SEEK_INFO success, fighter A odds increased.");
                }
                else
                {
                    odds.RaiseFighterBOdds(0.05f);
                    Debug.Log("[DialogueAction] SEEK_INFO fail, fighter B odds increased.");
                }
                return true;

            case "BRIBE_COACH":
                success = Random.value <= choice.successChance;
                if (success)
                {
                    odds.RaiseFighterAOdds(0.08f);
                    Debug.Log("[DialogueAction] BRIBE success,  fighter A odds increased.");
                }
                else
                {
                    odds.RaiseFighterBOdds(0.08f);
                    Debug.Log("[DialogueAction] BRIBE fail, fighter B odds increased.");
                }
                return true;

            default:
                return false;
        }
    }
}
