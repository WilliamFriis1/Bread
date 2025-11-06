using UnityEngine;

public static class DialogueActionResolver
{
    public static bool TryResolve(DialogueChoice choice, NpcDefinition npc, out bool success)
    {
        success = false;

        if (string.IsNullOrEmpty(choice.action))
        {
            return false;
        }
        //Bias from odds
        float bias = 0f;
        var gm = GameManager.Instance;
        if (gm != null && gm.Phase == GameManager.GamePhase.SpeakingToNPC && gm.OddsManager != null)
        {
            //Optional odds
            var oddsMgr = gm.OddsManager;
            var getBias = oddsMgr.GetType().GetMethod("GetDialogueBias");
            if (getBias != null)
            {
                bias = (float)getBias.Invoke(oddsMgr, null);
            }
        }
        float baseChance = choice.successChance;
        var player = gm != null ? gm.Player : null;

        switch (choice.action)
        {
            case "SEEK_INFO":
                {
                    float p = Mathf.Clamp01(baseChance + (npc?.infoBonus ?? 0f) + bias);
                    success = Random.value <= p;
                    return true;
                }
            case "BRIBE_COACH":
            case "BRIBE_REF":
                {
                    if (npc == null || !npc.acceptBribes)
                    {
                        success = false;
                        return true;
                    }
                    bool canPay = player != null && player.GetChips() > 0;
                    if (!canPay)
                    {
                        success = false;
                        return true;
                    }
                    float p = Mathf.Clamp01(baseChance + npc.bribeBonus + bias);
                    success = Random.value <= p;

                    return true;
                }
            case "BUY":
                {
                    if (npc != null && npc.isVendor && player != null)
                    {
                        if (player.GetChips() >= npc.commodityPrice)
                        {
                            player.RemoveChips(npc.commodityPrice);
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                        return true;
                    }
                    success = Random.value <= baseChance;
                    return true;
                }
            default:
                return false;

        }
    }
}
