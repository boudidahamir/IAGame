using UnityEngine;

public class AdhocHider : MonoBehaviour
{
    public Transform player;
    public float searchRadius = 10f;
    public float runRadius = 30f;
    public float runSpeed = 12f;
    public float searchSpeed = 5f;
    public float fieldOfViewAngle = 360f;
    public float rotationSpeed = 5f;
    public float obstacleAvoidanceDistance = 2f;

    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    public Vector3 lastKnownPlayerPosition;
    public StateHider currentState;

    private Vector3 hidePosition;

    public enum StateHider
    {
        Hiding,
        Running,
        Searching
    }

    void Start()
    {
        SetRandomHidePosition();
        currentState = StateHider.Searching;
    }

    void Update()
    {
        switch (currentState)
        {
            case StateHider.Hiding:
                HandleHidingState();
                break;
            case StateHider.Running:
                RunAwayFromPlayer();
                break;
            case StateHider.Searching:
                SearchForHideSpot();
                break;
        }
    }

    void HandleHidingState()
    {
        MoveTowards(hidePosition);
        RotateTowards(hidePosition);

        if (IsPlayerInProximity())
        {
            currentState = StateHider.Running;
        }
    }

    void RunAwayFromPlayer()
    {
        Vector3 runDirection = (transform.position - player.position).normalized;
        Vector3 runTarget = transform.position + runDirection * runRadius;

        Vector3 terrainAwareTarget = GetTerrainPosition(runTarget);
        MoveTowards(terrainAwareTarget, runSpeed);
        RotateTowards(terrainAwareTarget);

        if (!IsPlayerInProximity())
        {
            currentState = StateHider.Searching;
            SetRandomHidePosition();
        }
    }

    void SearchForHideSpot()
    {
        Vector3 terrainAwareTarget = GetTerrainPosition(hidePosition);
        MoveTowards(terrainAwareTarget);
        RotateTowards(terrainAwareTarget);

        if (Vector3.Distance(transform.position, hidePosition) < 0.5f)
        {
            currentState = StateHider.Hiding;
        }

        if (IsPlayerInProximity())
        {
            currentState = StateHider.Running;
        }
    }

    bool IsPlayerInProximity()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= searchRadius && IsPlayerInView();
    }

    void MoveTowards(Vector3 targetPosition, float speed = 0f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float moveSpeed = speed > 0 ? speed : searchSpeed;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            Vector3 hitNormal = hit.normal;
            Vector3 avoidanceDirection = Vector3.Cross(hitNormal, Vector3.up).normalized;
            Vector3 targetAvoidancePosition = hit.point + avoidanceDirection;

            targetAvoidancePosition = AdjustToGround(targetAvoidancePosition);
            transform.position = Vector3.MoveTowards(transform.position, targetAvoidancePosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 terrainTarget = GetTerrainPosition(targetPosition);

            terrainTarget = AdjustToGround(terrainTarget);
            transform.position = Vector3.MoveTowards(transform.position, terrainTarget, moveSpeed * Time.deltaTime);
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

        if (angleToPlayer < fieldOfViewAngle / 2f && distanceToPlayer < runRadius)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
            {
                return true;
            }
        }
        return false;
    }

    void SetRandomHidePosition()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, searchRadius, obstacleLayer);

        if (obstacles.Length > 0)
        {
            Collider chosenObstacle = obstacles[Random.Range(0, obstacles.Length)];

            Vector3 directionToPlayer = (player.position - chosenObstacle.transform.position).normalized;
            Vector3 hideOffset = -directionToPlayer * 2f;

            Vector3 potentialHidePosition = chosenObstacle.transform.position + hideOffset;

            hidePosition = GetTerrainPosition(potentialHidePosition);
        }
        else
        {
            hidePosition = transform.position;
        }
    }


    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
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
}
