using UnityEngine;

public class AdhocSeeker : MonoBehaviour
{
    public Transform player;
    public float searchRadius = 10f;
    public float chaseRadius = 30f;
    public float chaseSpeed = 10f;
    public float searchSpeed = 5f;
    public float confusedDuration = 3f;
    public float fieldOfViewAngle = 45f;
    public float rotationSpeed = 5f;
    public float obstacleAvoidanceDistance = 2f;

    public LayerMask obstacleLayer;

    private Vector3 searchPosition;
    public Vector3 lastKnownPlayerPosition;
    private float confusedTime;
    public StateAdhoc currentState;
    GameManager gameManager;
    public LayerMask groundLayer;

    public enum StateAdhoc
    {
        Searching,
        Chasing,
        Confused
    }

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        SetRandomSearchPosition();
        currentState = StateAdhoc.Searching;
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
            case StateAdhoc.Searching:
                SearchForPlayer();
                break;
            case StateAdhoc.Chasing:
                ChasePlayer();
                break;
            case StateAdhoc.Confused:
                HandleConfusedState();
                break;
        }
    }

    void SearchForPlayer()
    {
        MoveTowards(searchPosition);

        RotateTowards(searchPosition);

        if (Vector3.Distance(transform.position, searchPosition) < 0.5f)
        {
            SetRandomSearchPosition();
        }

        if (IsPlayerInView())
        {
            lastKnownPlayerPosition = player.position;
            currentState = StateAdhoc.Chasing;
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position);

        RotateTowards(player.position);
        lastKnownPlayerPosition = player.position;
        
        if (!IsPlayerInView())
        {
            currentState = StateAdhoc.Confused;
            confusedTime = Time.time; 
        }
    }

    void HandleConfusedState()
    {
        MoveTowards(lastKnownPlayerPosition);

        RotateTowards(lastKnownPlayerPosition);

        if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 0.5f)
        {
            float checkRadius = 5f; 
            bool playerFound = false;

            for (float rotationAngle = 0; rotationAngle < 360; rotationAngle += 45f)
            {
                transform.Rotate(Vector3.up, rotationAngle);
                playerFound = CheckForPlayerInProximity(checkRadius);
                if (playerFound)
                {
                    currentState = StateAdhoc.Chasing;
                    lastKnownPlayerPosition = player.position;
                    return;
                }
                transform.Rotate(Vector3.up, -rotationAngle);
            }

            currentState = StateAdhoc.Searching;
            SetRandomSearchPosition();
        }
    }

    bool CheckForPlayerInProximity(float checkRadius)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= checkRadius && IsPlayerInView();
    }


    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            Vector3 hitNormal = hit.normal;

            Vector3 avoidanceDirection = Vector3.Cross(hitNormal, Vector3.up).normalized; 
            Vector3 targetAvoidancePosition = hit.point + avoidanceDirection * 1f; 

            targetAvoidancePosition = AdjustToGround(targetAvoidancePosition);

            transform.position = Vector3.MoveTowards(transform.position, targetAvoidancePosition, searchSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 adjustedTargetPosition = AdjustToGround(targetPosition);

            transform.position = Vector3.MoveTowards(transform.position, adjustedTargetPosition, searchSpeed * Time.deltaTime);
        }
    }

    Vector3 AdjustToGround(Vector3 position)
    {
        Ray groundRay = new Ray(new Vector3(position.x, position.y + 10f, position.z), Vector3.down);

        if (Physics.Raycast(groundRay, out RaycastHit groundHit, 20f, groundLayer))
        {
            position.y = groundHit.point.y;
        }

        return position;
    }


    bool IsPlayerInView()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (angleToPlayer < fieldOfViewAngle / 2f && distanceToPlayer < chaseRadius)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayer))
            {
                float obstacleDistance = hit.distance;
                float distanceThreshold = 5f;

                if (obstacleDistance < distanceToPlayer && (distanceToPlayer - obstacleDistance) > distanceThreshold)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    void SetRandomSearchPosition()
    {
        float randomX = Random.Range(-searchRadius, searchRadius);
        float randomZ = Random.Range(-searchRadius, searchRadius);
        searchPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
