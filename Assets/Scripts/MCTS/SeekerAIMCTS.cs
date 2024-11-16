using UnityEngine;

public class SeekerAIMCTS : MonoBehaviour
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
    private Vector3 lastKnownPosition;
    private bool playerInSight;
    private float lostPlayerTimer = 5f;

    public State currentState;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

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
        if (gameManager.gameType == GameType.PlayerHide)
        {
            if (!gameManager.startGame)
                return;
        }

        CheckPlayerVisibility();

        Vector3 targetPosition = mcts.GetNextMove();


        if (IsWithinBounds(targetPosition))
        {
            MoveTowards(targetPosition);
            LookTowards(targetPosition);
        }

        HandleStateTransitions();
    }

    void HandleStateTransitions()
    {
        switch (currentState)
        {
            case State.Searching:
                if (playerInSight)
                {
                    Debug.Log("Player spotted. Transitioning to Chasing.");
                    TransitionToState(State.Chasing, player.position);
                }
                break;

            case State.Chasing:
                if (!playerInSight)
                {
                    Debug.Log("Lost sight of player. Transitioning to LostPlayer.");
                    TransitionToState(State.LostPlayer, lastKnownPosition);
                }
                else
                {
                    TransitionToState(State.Chasing, player.position);
                    lastKnownPosition = player.position;
                }
                break;

            case State.LostPlayer:
                lostPlayerTimer -= Time.deltaTime;
                if (lostPlayerTimer <= 0)
                {
                    Debug.Log("Lost player timeout. Transitioning to Searching.");
                    TransitionToState(State.Searching, lastKnownPosition);
                    lostPlayerTimer = 5;
                }
                if (playerInSight)
                {
                    Debug.Log("Player spotted. Transitioning to Chasing.");
                    TransitionToState(State.Chasing, player.position);
                }
                break;
        }
    }

    void CheckPlayerVisibility()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        playerInSight = directionToPlayer.magnitude <= detectionRadius &&
                        !Physics.Linecast(transform.position, player.position, obstaclesLayer);
    }

    void MoveTowards(Vector3 targetPosition)
    {
        // Calculate direction and ignore the Y-axis
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Ignore Y-axis
        direction = direction.normalized;

        if (direction.magnitude > 0.01f) // Only move if the distance is significant
        {
            // Avoid obstacles and move on X-Z plane
            direction = AvoidObstacles(direction);
            Vector3 targetXZ = transform.position + direction;
            targetXZ.y = transform.position.y; // Maintain current Y position
            transform.position = Vector3.MoveTowards(transform.position, targetXZ, speed * Time.deltaTime);

            AdjustToGround(); // Ensure the AI remains on the ground
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
