using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class HiderAIMCTS : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float obstacleDetectionRadius = 2f;
    public float avoidanceForce = 10f;
    public LayerMask obstaclesLayer;
    public LayerMask groundLayer;
    public float detectionRadius = 15f;

    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;

    private MCTS mcts;
    public bool playerInSight;

    public State currentState;
    
    private Vector3 hidingSpot;
    public float searchRadius;
    public bool hiding;
    bool setHidingSpot = false;

    void Start()
    {
        mcts = new MCTS(new GameState(
            transform.position,
            player.position,
            State.Searching,
            mapMinBounds,
            mapMaxBounds
        ));
    }

    void Update()
    {
        CheckPlayerVisibility();

        Vector3 targetPosition = mcts.GetNextMove();


        if (IsWithinBounds(targetPosition))
        {
            if (!hiding)
            {
                MoveTowards(targetPosition);
                LookTowards(targetPosition);
            }
        }

        HandleStateTransitions();
    }

    void HandleStateTransitions()
    {
        switch (currentState)
        {
            case State.hide:
                if (playerInSight)
                {
                    setHidingSpot = false ;
                    TransitionToState(State.run, player.position);
                }
                else if(!hiding)
                {
                    SetRandomHidePosition();
                    TransitionToState(State.hide, hidingSpot);
                }
                else
                    LookTowards(player.position);
                break;

            case State.run:
                if (!playerInSight)
                {
                    SetRandomHidePosition();
                    TransitionToState(State.hide, hidingSpot);
                }
                else
                {
                    setHidingSpot = false;
                    TransitionToState(State.run, player.position);
                }
                break;
        }
    }

    void CheckPlayerVisibility()
    {
        hiding = false;
        Vector3 directionToPlayer = player.position - transform.position;
        playerInSight = directionToPlayer.magnitude <= detectionRadius &&
                        !Physics.Linecast(transform.position, player.position, obstaclesLayer);
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        direction = direction.normalized;

        if (direction.magnitude > 0.1f)
        {
            direction = AvoidObstacles(direction);
            Vector3 targetXZ = transform.position + direction;
            targetXZ.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, targetXZ, speed * Time.deltaTime);

            AdjustToGround();
        }
        else
        {
            if(currentState == State.hide)
            {
                hiding = true;
            }
        }
    }

    void LookTowards(Vector3 targetPosition)
    {
        Vector3 directionToLook = targetPosition - transform.position;
        directionToLook.y = 0;

        if (directionToLook != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    Vector3 AvoidObstacles(Vector3 direction)
    {
        if (Physics.SphereCast(transform.position, obstacleDetectionRadius, direction, out RaycastHit hit, obstacleDetectionRadius, obstaclesLayer))
        {
            Vector3 avoidDirection = Vector3.Reflect(direction, hit.normal);
            return (direction + avoidDirection * avoidanceForce).normalized;
        }

        return direction;
    }

    void AdjustToGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    bool IsWithinBounds(Vector3 position)
    {
        bool isWithin = position.x >= mapMinBounds.x && position.x <= mapMaxBounds.x &&
                        position.y >= mapMinBounds.y && position.y <= mapMaxBounds.y &&
                        position.z >= mapMinBounds.z && position.z <= mapMaxBounds.z;


        return isWithin;
    }

    void TransitionToState(State newState, Vector3 targetPosition)
    {
        currentState = newState;
        mcts.UpdateState(new GameState(transform.position, targetPosition, newState, mapMinBounds, mapMaxBounds));
    }

    void SetRandomHidePosition()
    {
        if (!setHidingSpot)
        {
            Collider[] obstacles = Physics.OverlapSphere(transform.position, searchRadius, obstaclesLayer);

            if (obstacles.Length > 0)
            {
                Collider chosenObstacle = obstacles[Random.Range(0, obstacles.Length)];

                Vector3 directionToPlayer = (player.position - chosenObstacle.transform.position).normalized;
                Vector3 hideOffset = -directionToPlayer * 2f;

                Vector3 potentialHidePosition = chosenObstacle.transform.position + hideOffset;

                hidingSpot = GetTerrainPosition(potentialHidePosition);
            }
            else
            {
                hidingSpot = transform.position;
            }
            setHidingSpot = true;
        }
    }

    Vector3 GetTerrainPosition(Vector3 originalPosition)
    {
        Vector3 adjustedPosition = originalPosition;

        if (Physics.Raycast(new Vector3(originalPosition.x, originalPosition.y + 10f, originalPosition.z),
                            Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            adjustedPosition.y = hit.point.y;
        }

        return adjustedPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 2));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((mapMinBounds + mapMaxBounds) / 2, mapMaxBounds - mapMinBounds);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mcts.GetNextMove(), 1f);
        }

    }
}
