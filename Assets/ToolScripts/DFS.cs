//Author: William Friis
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#nullable enable

public class Node //Class as to be nullable, as well as default ref instead of copy
{
    public bool Visited;
    public string Dialogue;
    public string ID;
    public int DTreeIndex;
    public int TreeIndex;
    public List<Node> Children;

    public Node(string text, string id, int dTreeIndex)
    {
        Visited = false;
        Dialogue = text;
        ID = id;
        Children = new List<Node>();
        DTreeIndex = dTreeIndex;
    }
}

public class DFS : MonoBehaviour
{
    public uint Iterations;
    public string Goal;

    public NpcDefinition[] Npcs;
    public DialogueManager DialogueManager;

    private List<DialogueTree> DialogueTrees;
    private List<Node> DialogueTreeRoots;
    private void Start()
    {
        DialogueTrees = new List<DialogueTree>();
        DialogueTreeRoots = new List<Node>();

        InitTrees();
        BuildTree(DialogueTreeRoots[0], 0);

        var goal = "Interesting. I see what you want me to do, kid. If you've got the chips, I'll do it.";
        var result = Search(DialogueTreeRoots[0], goal);

        if (result == null)
            Debug.Log("Unable to find goal");
        else
            Debug.Log($"Goal found at level {result.TreeIndex}");
    }

    public void InitTrees()
    {
        for(int i = 0; i < Npcs.Length; i++)
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Dialogues", Npcs[i].dialogueFile);

            var json = File.ReadAllText(path);
            
            var tree = DialogueTree.FromJson(json);

            DialogueTrees.Add(tree);
            DialogueTreeRoots.Add(new Node(tree.GetNode(tree.start[0]).text, tree.start[0], i));
        }
    }
    public Node Search(Node root, string goal)
    {
        Node result;

        root.Visited = true;

        if (root.Dialogue == goal)
            return root;

        for (int i = 0; i < root.Children.Count; i++)
        {
            if (root.Children[i] == null) return null;

            if (root.Children[i].Dialogue == goal)
                result = root.Children[i];
            else
                result = Search(root.Children[i], goal);

            if (result != null) return result;
        }

        return null;
    }

    public void BuildTree(Node root, int currIndex)
    {
        root.TreeIndex = currIndex;

        foreach (var choice in DialogueTrees[root.DTreeIndex].GetNode(root.ID).choices)
        {
            if(!IsEmpty(choice.gotoNode))
            {
                var newNode = new Node(DialogueTrees[root.DTreeIndex].GetNode(choice.gotoNode).text, choice.gotoNode, root.DTreeIndex);

                root.Children.Add(newNode);
                BuildTree(newNode, root.TreeIndex + 1);
            }

            if (!IsEmpty(choice.gotoOnSuccess))
            {
                var newNode = new Node(DialogueTrees[root.DTreeIndex].GetNode(choice.gotoOnSuccess).text, choice.gotoOnSuccess, root.DTreeIndex);

                root.Children.Add(newNode);
                BuildTree(newNode, root.TreeIndex + 1);
            }

            if (!IsEmpty(choice.gotoOnFail))
            {
                var newNode = new Node(DialogueTrees[root.DTreeIndex].GetNode(choice.gotoOnFail).text, choice.gotoOnFail, root.DTreeIndex);

                root.Children.Add(newNode);
                BuildTree(newNode, root.TreeIndex + 1);
            }
        }
    }

    public bool IsEmpty(string? s)
    {
        if(s == null || s == "" || s == string.Empty)
            return true;

        return false;
    }
}   



