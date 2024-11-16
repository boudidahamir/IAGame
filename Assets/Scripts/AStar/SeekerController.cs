using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : MonoBehaviour
{
    public SeekerState currentState = SeekerState.Patrolling;
    public Transform player;
    public float sightRange = 10f;
    public float chaseDuration = 5f;
    public float movementSpeed = 5f;

    private PathFinding pathFinding;
    private Vector3 lastKnownPosition;
    private float timeSinceLastSeen = 0;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        pathFinding = GetComponent<PathFinding>();
        PatrolToRandomPosition();
    }

    void Update()
    {
        if (gameManager.gameType == GameType.PlayerHide)
        {
            if (!gameManager.startGame)
                return;
        }

        switch (currentState)
        {
            case SeekerState.Patrolling:
                Patrol();
                break;
            case SeekerState.Chasing:
                ChasePlayer();
                break;
            case SeekerState.Searching:
                SearchLastKnownPosition();
                break;
        }
    }

    void PatrolToRandomPosition()
    {
        Vector3 randomPosition = GetRandomGridPosition();
        pathFinding.findPath(transform.position, randomPosition);
    }

    void Patrol()
    {
        MoveAlongPath();

        if (CanSeePlayer())
        {
            currentState = SeekerState.Chasing;
            lastKnownPosition = player.position;
        }

        if (pathFinding.grid.path.Count == 0)
        {
            PatrolToRandomPosition();
        }
    }

    void ChasePlayer()
    {
        if (CanSeePlayer())
        {
            timeSinceLastSeen = 0;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= chaseDuration)
            {
                lastKnownPosition = player.position;
                currentState = SeekerState.Searching;
            }
        }
        pathFinding.findPath(transform.position, player.position);
        MoveAlongPath();
    }

    void SearchLastKnownPosition()
    {
        Node lastKnownNode = pathFinding.grid.NodeFromWorldPoint(lastKnownPosition);

        if (lastKnownNode != null && lastKnownNode.walkable)
        {
            pathFinding.findPath(transform.position, lastKnownPosition);
            MoveAlongPath();
        }
        else
        {
            Vector3 nearestWalkablePosition = FindNearestWalkablePosition(lastKnownPosition);

            if (nearestWalkablePosition != Vector3.zero)
            {
                pathFinding.findPath(transform.position, nearestWalkablePosition);
                MoveAlongPath();
            }
            else
            {
                currentState = SeekerState.Patrolling;
                PatrolToRandomPosition();
            }
        }

        if (pathFinding.grid.path == null || pathFinding.grid.path.Count == 0)
        {
            currentState = SeekerState.Patrolling;
            PatrolToRandomPosition();
        }
    }


    Vector3 FindNearestWalkablePosition(Vector3 targetPosition)
    {
        Node targetNode = pathFinding.grid.NodeFromWorldPoint(targetPosition);

        if (targetNode.walkable)
        {
            return targetNode.worldPosition;
        }

        List<Node> nearbyWalkableNodes = pathFinding.grid.GetNeighbours(targetNode);
        if (nearbyWalkableNodes != null && nearbyWalkableNodes.Count > 0)
        {
            return nearbyWalkableNodes[0].worldPosition;
        }

        return Vector3.zero;
    }


    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= sightRange)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }


    void MoveAlongPath()
    {
        if (pathFinding.grid.path != null && pathFinding.grid.path.Count > 0)
        {
            Vector3 nextStep = pathFinding.grid.path[0].worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, nextStep, Time.deltaTime * movementSpeed);

            if (Vector3.Distance(transform.position, nextStep) < 0.5f)
            {
                pathFinding.grid.path.RemoveAt(0);
            }

            if (pathFinding.grid.path.Count == 0)
            {
                if (currentState == SeekerState.Searching)
                {
                    currentState = SeekerState.Patrolling;
                    PatrolToRandomPosition();
                }
            }
        }
        else if (currentState == SeekerState.Searching)
        {
            currentState = SeekerState.Patrolling;
            PatrolToRandomPosition();
        }
    }


    Vector3 GetRandomGridPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Node randomNode = pathFinding.grid.GetRandomWalkableNode();

            if (randomNode != null && Vector3.Distance(randomNode.worldPosition, player.position) > 5f)
            {
                return randomNode.worldPosition;
            }
        }
        return transform.position;
    }



}

public enum SeekerState
{
    Patrolling,
    Chasing,
    Searching
}


