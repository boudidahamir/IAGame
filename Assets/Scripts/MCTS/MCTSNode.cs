using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MCTSNode
{
    public MCTSNode Parent;
    public List<MCTSNode> Children { get; } = new List<MCTSNode>();
    public GameState State { get; }
    public int Visits { get; private set; }
    public float Score { get; private set; }

    private static readonly float DefaultExplorationWeight = Mathf.Sqrt(2);

    public MCTSNode(MCTSNode parent, GameState state)
    {
        Parent = parent;
        State = state;
    }

    public void AddChild(GameState newState)
    {
        if (Children.Any(child => child.State.SeekerPosition == newState.SeekerPosition && child.State.SeekerState == newState.SeekerState))
        {
            return; // Avoid adding duplicate states.
        }
        Children.Add(new MCTSNode(this, newState));
    }

    public void UpdateStats(float score)
    {
        Visits++;
        Score += score;
    }

    public MCTSNode GetBestUCTChild(float explorationWeight = -1)
    {
        if (Children.Count == 0) return null;

        explorationWeight = explorationWeight < 0 ? DefaultExplorationWeight : explorationWeight;

        return Children.OrderByDescending(child =>
            (child.Score / (child.Visits + 1e-6f)) + // Exploitation term
            explorationWeight * Mathf.Sqrt(Mathf.Log(Visits + 1) / (child.Visits + 1e-6f)) // Exploration term
        ).FirstOrDefault();
    }

    public MCTSNode GetBestChild()
    {
        return Children.Count > 0 ? Children.OrderByDescending(c => c.Visits).FirstOrDefault() : null;
    }
}
