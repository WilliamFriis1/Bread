using System.Collections.Generic;
using UnityEngine;

public class GameContext
{
    public int Money;
    public string CurrentFighterId;
    public HashSet<string> Flags = new();
    public List<string> Inventory = new(); //for when you buy "flour" or other items from the shady guy
}
