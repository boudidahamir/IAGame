using System.Linq;
using UnityEngine;

public class MCTS
{
    private MCTSNode rootNode;
    private LayerMask obstacles = LayerMask.GetMask("Unwalkable");

    public MCTS(GameState initialState)
    {
        rootNode = new MCTSNode(null, initialState);
    }

    public Vector3 GetNextMove()
    {
        PerformSearch(30);
        return rootNode.GetBestChild()?.State.SeekerPosition ?? rootNode.State.SeekerPosition;
    }

    private void PerformSearch(int simulations)
    {
        int validSimulations = 0;

        for (int i = 0; i < simulations; i++)
        {
            MCTSNode node = SelectNode(rootNode);

            Vector3 newSeekerPosition = CalculateNewSeekerPosition(node.State);

            if (!IsPositionValid(newSeekerPosition, node.State))
            {
                continue;
            }

            GameState newState = node.State.SimulateMove(newSeekerPosition);
            node.AddChild(newState);

            Backpropagate(node.Children.Last(), newState.Evaluate());
            validSimulations++;

            if (validSimulations >= simulations) break;
        }

    }


    private bool IsPositionValid(Vector3 position, GameState state)
    {
        bool isWithinBounds = position.x >= state.MinBounds.x && position.x <= state.MaxBounds.x &&
                              position.y >= state.MinBounds.y && position.y <= state.MaxBounds.y &&
                              position.z >= state.MinBounds.z && position.z <= state.MaxBounds.z;

        bool isFreeFromObstacles = !Physics.CheckSphere(position, 0.5f, obstacles); // Adjust radius to match obstacle sizes

        return isWithinBounds && isFreeFromObstacles;
    }


    private MCTSNode SelectNode(MCTSNode node, int depth = 0)
    {
        int maxDepth = 20;
        while (node.Children.Count > 0 && depth < maxDepth)
        {
            node = node.GetBestUCTChild();
            depth++;
        }
        return node;
    }

    private void Backpropagate(MCTSNode node, float score)
    {
        while (node != null)
        {
            node.UpdateStats(score);
            node = node.Parent;
        }
    }

    private Vector3 CalculateNewSeekerPosition(GameState state)
    {
        Vector3 newPosition;

        // Prioritize moving towards the player
        if (state.SeekerState == State.Searching)
        {
            do
            {
                newPosition = new Vector3(
                    Random.Range(state.MinBounds.x, state.MaxBounds.x),
                    Random.Range(state.MinBounds.y, state.MaxBounds.y),
                    Random.Range(state.MinBounds.z, state.MaxBounds.z)
                );
            }
            while (!IsPositionValid(newPosition, state)); // Regenerate if invalid
        }
        else if (state.SeekerState == State.run)
        {
            Vector3 directionToTarget = state.targetPosition - state.SeekerPosition;

            Vector3 directionAwayFromTarget = -directionToTarget.normalized;

            float awayDistance = 5f;
            Vector3 pos = state.SeekerPosition + directionAwayFromTarget * awayDistance;
            newPosition = new Vector3 (pos.x , 0 , pos.z);
        }

        else
        {
            newPosition = new Vector3(state.targetPosition.x, 0, state.targetPosition.z);
        }

        return newPosition;
    }


    public void UpdateState(GameState newState)
    {
        var matchingChild = rootNode.Children.FirstOrDefault(child => child.State.Equals(newState));

        if (matchingChild != null)
        {
            rootNode = matchingChild;
            rootNode.Parent = null;
        }
        else
        {
            rootNode = new MCTSNode(null, newState);
        }
    }

}
