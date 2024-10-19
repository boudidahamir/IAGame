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

    void Start()
    {
        pathFinding = GetComponent<PathFinding>();
        PatrolToRandomPosition();
    }

    void Update()
    {
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
            pathFinding.findPath(transform.position, player.position);
            lastKnownPosition = player.position;
            timeSinceLastSeen = 0;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= chaseDuration)
            {
                currentState = SeekerState.Searching;
            }
        }
        MoveAlongPath();
    }

    void SearchLastKnownPosition()
    {
        pathFinding.findPath(transform.position, lastKnownPosition);
        MoveAlongPath();

        if (Vector3.Distance(transform.position, lastKnownPosition) < 1.0f)
        {
            currentState = SeekerState.Patrolling;
            PatrolToRandomPosition();
        }

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
        }
    }

    Vector3 GetRandomGridPosition()
    {
        for (int i = 0; i < 10; i++) // Limit attempts to prevent infinite loops
        {
            Node randomNode = pathFinding.grid.GetRandomWalkableNode();

            if (randomNode != null && Vector3.Distance(randomNode.worldPosition, player.position) > 5f)
            {
                return randomNode.worldPosition;
            }
        }
        return transform.position; // Fallback position
    }



}

public enum SeekerState
{
    Patrolling,
    Chasing,
    Searching
}


