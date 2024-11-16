using UnityEngine;

public class GameState
{
    public Vector3 SeekerPosition { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public Vector3 MinBounds { get; private set; }
    public Vector3 MaxBounds { get; private set; }
    public State SeekerState { get; private set; }

    public GameState(Vector3 seekerPos, Vector3 pos, State state, Vector3 minBounds, Vector3 maxBounds)
    {
        SeekerPosition = seekerPos;
        targetPosition = pos;
        SeekerState = state;
        MinBounds = minBounds;
        MaxBounds = maxBounds;
    }

    public GameState SimulateMove(Vector3 newSeekerPos)
    {
        newSeekerPos.x = Mathf.Clamp(newSeekerPos.x, MinBounds.x, MaxBounds.x);
        newSeekerPos.y = Mathf.Clamp(newSeekerPos.y, MinBounds.y, MaxBounds.y);
        newSeekerPos.z = Mathf.Clamp(newSeekerPos.z, MinBounds.z, MaxBounds.z);

        return new GameState(newSeekerPos, targetPosition, SeekerState, MinBounds, MaxBounds);
    }


    public float Evaluate()
    {
        float score = 0f;

        switch (SeekerState)
        {
            case State.Searching:
            case State.LostPlayer:
                score -= Vector3.Distance(SeekerPosition, targetPosition) * 0.1f;
                break;

            case State.Chasing:
                score -= Vector3.Distance(SeekerPosition, targetPosition) * 10f;
                break;
        }

        return score;
    }


    public override bool Equals(object obj)
    {
        if (obj is not GameState other) return false;

        return SeekerPosition == other.SeekerPosition &&
               targetPosition == other.targetPosition &&
               SeekerState == other.SeekerState;
    }

    public override int GetHashCode()
    {
        return SeekerPosition.GetHashCode() ^ targetPosition.GetHashCode() ^ SeekerState.GetHashCode();
    }
}
