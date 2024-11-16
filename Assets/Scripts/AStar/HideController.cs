using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float runAwayRadius = 10f;
    public float moveSpeed = 5f;

    private Grid grid;
    private PathFinding pathFinding;
    private Vector3 currentDestination;

    public enum State { Hiding, Running, Searching }
    public State currentState = State.Searching;

    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        pathFinding = GetComponent<PathFinding>();
        FindHidingSpot();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == State.Hiding)
        {
            if (distanceToPlayer <= detectionRadius)
            {
                currentState = State.Running;
                SetRunAwayTarget();
            }
        }
        else if (currentState == State.Running)
        {
            if (distanceToPlayer > runAwayRadius)
            {
                currentState = State.Searching;
                FindHidingSpot();
            }
            else
            {
                RunAway();
            }
        }
        else if (currentState == State.Searching)
        {
            if (grid.path != null && grid.path.Count > 0)
            {
                MoveToDestination();
            }
            else
            {
                currentState = State.Hiding;
            }
        }
    }

    void SetRunAwayTarget()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized; 
        Vector3 targetPosition = transform.position + directionAwayFromPlayer * runAwayRadius;

        Node targetNode = grid.NodeFromWorldPoint(targetPosition); 
        if (targetNode != null && targetNode.walkable)
        {
            currentDestination = targetNode.worldPosition;
            pathFinding.findPath(transform.position, currentDestination);
        }
        else
        {
            Node nearestWalkable = FindNearestWalkableNode(targetPosition);
            if (nearestWalkable != null)
            {
                currentDestination = nearestWalkable.worldPosition;
                pathFinding.findPath(transform.position, currentDestination);
            }
        }
    }

    void RunAway()
    {
        if (grid.path != null && grid.path.Count > 0)
        {
            MoveToDestination();
        }
        else
        {
            currentState = State.Searching;
        }
    }

    void MoveToDestination()
    {
        Node nextNode = grid.path[0];
        transform.position = Vector3.MoveTowards(transform.position, nextNode.worldPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextNode.worldPosition) < 0.1f)
        {
            grid.path.RemoveAt(0);
        }
    }

    void FindHidingSpot()
    {
        Node nearestUnwalkable = GetNearestUnwalkableNode();

        if (nearestUnwalkable != null)
        {
            currentDestination = nearestUnwalkable.worldPosition;
            pathFinding.findPath(transform.position, currentDestination);
        }
    }

    Node FindNearestWalkableNode(Vector3 position)
    {
        Node nearestWalkable = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Node node in grid.GetAllNodes())
        {
            if (node.walkable)
            {
                float distance = Vector3.Distance(position, node.worldPosition);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestWalkable = node;
                }
            }
        }
        return nearestWalkable;
    }

    Node GetNearestUnwalkableNode()
    {
        Node nearestUnwalkable = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Node node in grid.GetAllNodes())
        {
            if (!node.walkable)
            {
                float distance = Vector3.Distance(transform.position, node.worldPosition);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestUnwalkable = node;
                }
            }
        }

        if (nearestUnwalkable != null)
        {
            List<Node> neighbours = grid.GetNeighbours(nearestUnwalkable);
            foreach (Node neighbour in neighbours)
            {
                if (neighbour.walkable)
                {
                    return neighbour;
                }
            }
        }

        return null;
    }
}
