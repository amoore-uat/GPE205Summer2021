using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIPersonality { Aggressive };


[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]
public class AIController : MonoBehaviour
{
    public enum PatrolType { PingPong, Stop, Loop };
    public enum AvoidanceState { NotAvoiding, RotatingToAvoid, MovingToAvoid };
    public enum AIState { Idle, Patrol, ChaseAndFire };
    public AIState currentAIState = AIState.Idle;
    public AIPersonality personality;
    public AvoidanceState avoidance = AvoidanceState.NotAvoiding;
    public List<GameObject> m_waypoints = new List<GameObject>();
    public float closeEnough = 1f; // A distance from a location that is close enough to consider ourselves there.
    public float avoidanceTime = 1f;
    private float avoidanceTimer;
    private float stateEnterTime;
    private GameObject seenPlayer;
    public float fieldOfView = 60f;
    public int currentWayPoint;
    private bool moveForward = true;
    private GameObject m_randomWaypoint;
    public int m_waypointsToGenerate = 3;
    public Material aggressiveMaterial;

    private TankData m_data;
    private TankMotor m_motor;
    private TankShooter m_shooter;

    // Start is called before the first frame update
    void Start()
    {
        m_data = gameObject.GetComponent<TankData>();
        m_motor = gameObject.GetComponent<TankMotor>();
        m_shooter = gameObject.GetComponent<TankShooter>();
    }

    // Update is called once per frame
    void Update()
    {

        RunFiniteStateMachine();
    }

    private void RunFiniteStateMachine()
    {
        switch (personality)
        {
            case AIPersonality.Aggressive:
                AggressiveFSM();
                break;
            default:
                Debug.LogWarning("[AIController] Unimplemented AI Personality");
                break;
        }
    }

    private void AggressiveFSM()
    {
        switch (currentAIState)
        {
            case AIState.Idle:
                ChangeState(AIState.Patrol);
                break;
            case AIState.ChaseAndFire:
                // Do Behavior
                ChaseAndFire(seenPlayer);
                // Check for transitions
                if (!CanSee(seenPlayer))
                {
                    ChangeState(AIState.Patrol);
                }
                break;
            case AIState.Patrol:
                // Do Behavior
                Patrol(PatrolType.PingPong);
                // Check for transitions
                foreach (GameObject player in GameManager.Instance.Players)
                {
                    if (CanSee(player))
                    {
                        ChangeState(AIState.ChaseAndFire);
                        seenPlayer = player;
                        break;
                    }
                }
                break;
            default:
                ChangeState(AIState.Idle);
                break;
        }
    }

    private void ChaseAndFire(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[AIController - ChaseAndFire] Target object is null");
            return;
        }
        // Move toward our target
        MoveTowards(targetObject.transform.position);
        // Shoot
        m_shooter.Shoot();
    }

    private void ChangeState(AIState newState)
    {
        currentAIState = newState;
        stateEnterTime = Time.time;
    }

    private void ChangeState(AvoidanceState newState)
    {
        avoidance = newState;
        avoidanceTimer = avoidanceTime;
    }

    // Move towards a target position.
    public void MoveTowards(Vector3 targetPosition)
    {
        switch (avoidance)
        {
            case AvoidanceState.NotAvoiding:
                // Rotate towards the target position if we are not facing the target position.
                if (!IsFacing(targetPosition))
                {
                    RotateTowards(targetPosition);
                }
                // Move forward if facing the target position.
                else
                {
                    // Don't move if we've already arrived at our destination.
                    if (!IsCloseEnough(targetPosition))
                    {
                        if (CanMove(m_data.moveSpeed))
                        {
                            m_motor.Move(m_data.moveSpeed);
                        }
                        else
                        {
                            ChangeState(AvoidanceState.RotatingToAvoid);
                        }
                    }
                }
                break;
            case AvoidanceState.RotatingToAvoid:
                Debug.Log("Rotating to avoid");
                // Rotate until we can move forward
                m_motor.Rotate(m_data.rotateSpeed);
                // if we can move forward, then switch to moving to avoid.
                if (CanMove(m_data.moveSpeed))
                {
                    ChangeState(AvoidanceState.MovingToAvoid);
                    //avoidanceTimer = avoidanceTime;
                }
                break;
            case AvoidanceState.MovingToAvoid:

                // if timer runs down, switch to not avoiding
                if (avoidanceTimer <= 0)
                {
                    ChangeState(AvoidanceState.NotAvoiding);
                }
                // if we're about to run into something, switch to rotating
                else if (!CanMove(m_data.moveSpeed))
                {
                    ChangeState(AvoidanceState.RotatingToAvoid);
                }
                else
                {
                    // move forward for a number of seconds.
                    m_motor.Move(m_data.moveSpeed);
                    avoidanceTimer -= Time.deltaTime;
                }
                break;
        }
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        Quaternion targetRotation = GetRotationToTarget(GetVectorToTarget(targetPosition));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_data.rotateSpeed * Time.deltaTime);
    }

    private Quaternion GetRotationToTarget(Vector3 vectorToTarget)
    {
        return Quaternion.LookRotation(vectorToTarget);
    }

    private Vector3 GetVectorToTarget(Vector3 targetPosition)
    {
        return targetPosition - transform.position;
    }

    public bool IsFacing(Vector3 targetPosition)
    {
        Quaternion targetRotation = Quaternion.LookRotation(GetVectorToTarget(targetPosition));
        return (targetRotation == transform.rotation);
    }

    public bool IsCloseEnough(Vector3 targetPosition)
    {
        return (Vector3.SqrMagnitude(targetPosition - transform.position) <= (closeEnough * closeEnough));
    }

    public void Patrol(PatrolType patrolType)
    {
        if (m_waypoints.Count < 1)
        {
            for (int i = 0; i < m_waypointsToGenerate; i++)
            {
                m_waypoints.Add(GameManager.Instance.GetRandomWaypoint());
            }
            Debug.LogWarning(gameObject.name + " [AIController - Patrol] Attempting to loop through empty array of waypoints. Getting new waypoints.");
            return;
        }
        MoveTowards(m_waypoints[currentWayPoint].transform.position);
        if (IsCloseEnough(m_waypoints[currentWayPoint].transform.position))
        {
            switch (patrolType)
            {
                case PatrolType.PingPong:
                    // go in reverse through waypoints after final waypoint.
                    if (moveForward)
                    {
                        if (currentWayPoint < (m_waypoints.Count - 1))
                        {
                            currentWayPoint++;
                        }
                        else
                        {
                            moveForward = !moveForward;
                        }
                    }
                    else
                    {
                        if (currentWayPoint > 0)
                        {
                            currentWayPoint--;
                        }
                        else
                        {
                            moveForward = !moveForward;
                        }
                    }
                    break;
                case PatrolType.Loop:
                    // go to first waypoint after final waypoint.
                    // Modulo (%) lets us get the remainder from integer division.
                    // 1/4 is 0 remainder 1. 4/4 is 1 remainder 0.
                    // This lets us loop through the waypoints without using an if statement.
                    currentWayPoint = ((currentWayPoint + 1) % 4);
                    break;
                case PatrolType.Stop:
                    // stop at final waypoint.
                    if (currentWayPoint < (m_waypoints.Count - 1))
                    {
                        currentWayPoint++;
                    }
                    break;
                default:
                    Debug.LogWarning(gameObject.name + " [AIController - Patrol] Unimplemented Patrol Type.");
                    break;
            }
        }
    }

    public void Wander()
    {

        if (m_randomWaypoint == null)
        {
            // Get a random waypoint if we don't already have one.
            m_randomWaypoint = GameManager.Instance.GetRandomWaypoint();
        }
        else
        {
            // Move to it.
            MoveTowards(m_randomWaypoint.transform.position);
            if (IsCloseEnough(m_randomWaypoint.transform.position))
            {
                m_randomWaypoint = null;
            }
        }

    }

    public void ChangeTankColors()
    {
        // Get all of the mesh renderers for the tank.
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        // Set color based on current personality
        switch (personality)
        {
            case AIPersonality.Aggressive:
                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.material = aggressiveMaterial;
                }
                break;
            default:
                break;
        }
    }

    public bool CanMove(float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            return false;
        }
        return true;
    }

    public void MoveAwayFrom(Vector3 targetPosition)
    {
        Vector3 vectorAwayFromTarget = -1 * GetVectorToTarget(targetPosition);
        vectorAwayFromTarget.Normalize();
        Vector3 fleePosition = (vectorAwayFromTarget * m_data.moveSpeed) + transform.position;
        Debug.Log(fleePosition);
        MoveTowards(fleePosition);
    }

    public void Flee(GameObject targetObject)
    {
        MoveAwayFrom(targetObject.transform.position);
    }

    public bool CanSee(GameObject targetObject)
    {
        if (targetObject == null) { return false; }
        // We use the location of our target in a number of calculations - store it in a variable for easy access.
        Vector3 targetPosition = targetObject.transform.position;

        // Find the vector from the agent to the target
        // We do this by subtracting "destination minus origin", so that "origin plus vector equals destination."
        Vector3 agentToTargetVector = GetVectorToTarget(targetObject.transform.position);

        // Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
        float angleToTarget = Vector3.Angle(agentToTargetVector, transform.forward);

        // if that angle is less than our field of view
        if (angleToTarget < fieldOfView)
        {
            // Create a variable to hold a ray from our position to the target
            Ray rayToTarget = new Ray();

            // Set the origin of the ray to our position, and the direction to the vector to the target
            rayToTarget.origin = transform.position;
            rayToTarget.direction = agentToTargetVector;

            // Create a variable to hold information about anything the ray collides with
            RaycastHit hitInfo;

            // Cast our ray for infinity in the direciton of our ray.
            //    -- If we hit something...
            if (Physics.Raycast(rayToTarget, out hitInfo, Mathf.Infinity))
            {
                // ... and that something is our target 
                if (hitInfo.collider.gameObject == targetObject)
                {
                    // return true 
                    //    -- note that this will exit out of the function, so anything after this functions like an else
                    return true;
                }
            }
        }
        // return false
        //   -- note that because we returned true when we determined we could see the target, 
        //      this will only run if we hit nothing or if we hit something that is not our target.
        return false;
    }
}