//Author: William Friis
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable enable

//TODO: Fixa så detectionen fungerar med felaktiga nodes
public class Node //Class as to be nullable, as well as default ref instead of copy
{
    public bool Visited;
    public string Dialogue;
    public string ID;
    public int Depth;
    public List<Node> Children;

    public Node(string text, string id, int depth = 0)
    {
        Visited = false;
        Dialogue = text;
        ID = id;
        Children = new List<Node>();
    }
}

public class DFS : MonoBehaviour
{
    public uint Iterations;
    public string Goal;

    public NpcDefinition[] Npcs;
    public DialogueManager DialogueManager;

    private List<DialogueTree> _dialogueGraphs;
    private List<Node> _dialogueRoots;
    private Dictionary<string, Node> _nodes;
    private Dictionary<string, List<string>> _potentialGoals;
    private HashSet<string> _recursionGuard;

    private int _failures;
    private int _successes;
    private int _totalSearches;
    private void Start()
    {
        _dialogueGraphs = new List<DialogueTree>();
        _dialogueRoots = new List<Node>();
        _nodes = new Dictionary<string, Node>();
        _potentialGoals = new Dictionary<string, List<string>>();
        _recursionGuard = new HashSet<string>();

        InitTrees();

        Run();
    }

    void Run()
    {
        foreach (var v in _dialogueGraphs)
        {
            BuildDictionary(v);
        }

        FindOrphanNodes();

        foreach(var v in _dialogueRoots)
        {
            foreach (var text in _potentialGoals[v.ID])
            {
               var result = Search(v, text);
                
                if(result != null)
                    _successes++;
                else
                    _failures++;

                _totalSearches++;
            }
        }

        Debug.Log($"Total searches: {_totalSearches}, Successes: {_successes}, Failures: {_failures}");
    }
    public void InitTrees()
    {
        for(int i = 0; i < Npcs.Length; i++)
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Dialogues", Npcs[i].dialogueFile);

            var json = File.ReadAllText(path);
            
            var tree = DialogueTree.FromJson(json);

            _dialogueGraphs.Add(tree);
        }
    }

    #region TREE_ACTIONS
    public Node Search(Node root, string goal)
    {
        if (root.Visited)
            return null;

        root.Visited = true;

        if(root.Dialogue == goal)
        {
            root.Visited = false;
            return root;
        }

        Node result = null;

        for(int i = 0; i < root.Children.Count; i++)
        {
           result = Search(root.Children[i], goal);

            if(result != null)
            {
                root.Visited = false;
                return result;
            }
        }

        root.Visited = false;
        return null;
    }

    public void FindOrphanNodes()
    {
        HashSet<string> nodesWithParents = new HashSet<string>();

        foreach (var node in _nodes.Values)
        {
            foreach (var child in node.Children)
            {
                nodesWithParents.Add(child.ID);
            }
        }

        var orphans = new List<Node>();

        foreach (var node in _nodes)
        {
            if (!_dialogueRoots.Exists(root => root.ID == node.Key) && !nodesWithParents.Contains(node.Key))
            {
                orphans.Add(node.Value);
            }
        }

        if (orphans.Count > 0)
        {
            Debug.Log("Orphans found: ");

            foreach (var node in orphans)
                Debug.Log(node.ID);
        }
        else
            Debug.Log("No orphans found.");
    }
    #endregion
    #region GRAPH_BUILDING
    public void BuildDictionary(DialogueTree tree)
    {
        var root = tree.GetNode(tree.start[0]);
        _potentialGoals.Add(root.id, new List<string>());

        foreach(var node in tree.nodes)
        {
            if(!_nodes.ContainsKey(node.id))
            {
                _nodes.Add(node.id, new Node(node.text, node.id));
                _potentialGoals[root.id].Add(node.text);
            }
        }

        _dialogueRoots.Add(_nodes[root.id]);

        AddNodeRecursively(root, tree);
    }

    public void AddNodeRecursively(DialogueNode root, DialogueTree tree)
    {
        if (_recursionGuard.Contains(root.id))
            return;

        _recursionGuard.Add(root.id);

        var targets = new List<string>();

        for(int i = 0; i < root.choices.Count; i++)
        {
            var choice = root.choices[i];

            targets.Add(choice.gotoNode);
            targets.Add(choice.gotoOnFail);
            targets.Add(choice.gotoOnSuccess);

            foreach(var v in targets)
            {
                if (!IsEmpty(v))
                {
                    var node = tree.GetNode(v);

                    AddNodeRecursively(node, tree);

                    _nodes[root.id].Children.Add(_nodes[node.id]);
                }
            }

            targets.Clear();
        }

        _recursionGuard.Remove(root.id);
    }
#endregion
    #region HELPER_METHODS
    public bool IsEmpty(string? s)
    {
        if(s == null || s == "" || s == string.Empty)
            return true;

        return false;
    }
    #endregion
}



