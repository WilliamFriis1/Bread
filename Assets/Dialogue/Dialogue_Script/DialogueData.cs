using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string id;
    public string text;
    public string gotoNode;         
    public string action;           
    public float successChance = 1f; 
    public string gotoOnSuccess;   
    public string gotoOnFail;       
}

[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public bool end = false;
    public List<DialogueChoice> choices;
}

[Serializable]
public class DialogueTreeJsonWrapper
{
    public string start;
    public List<DialogueNode> nodes;
}

public class DialogueTree
{
    public string start;
    public Dictionary<string, DialogueNode> nodes = new Dictionary<string, DialogueNode>();

    public static DialogueTree FromJson(string json)
    {
        var wrapper = JsonUtility.FromJson<DialogueTreeJsonWrapper>(json);
        DialogueTree tree = new DialogueTree();
        tree.start = wrapper.start;
        if (wrapper.nodes != null)
        {
            foreach (var n in wrapper.nodes)
                tree.nodes[n.id] = n;
        }
        return tree;
    }
}
