using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]
public class AIController : MonoBehaviour
{
    public enum PatrolType { PingPong, Stop, Loop};
    public enum AvoidanceState { NotAvoiding, RotatingToAvoid, MovingToAvoid };
    public enum AIPersonality { Aggressive };
    public enum AIState { Idle, Patrol, ChaseAndFire };
    public AIState currentAIState = AIState.Idle;
    public AIPersonality personality;
    public AvoidanceState avoidance = AvoidanceState.NotAvoiding;
    public GameObject[] m_waypoints;
    public float closeEnough = 1f; // A distance from a location that is close enough to consider ourselves there.
    public float avoidanceTime = 1f;
    private float avoidanceTimer;
    private float stateEnterTime;
    private GameObject seenPlayer;
    public float FOV = 60f;

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
        if (m_waypoints.Length < 1)
        {
            Debug.LogWarning(gameObject.name + " [AIController - Patrol] Attempting to loop through empty array of waypoints.");
            return;
        }
        switch (patrolType)
        {
            case PatrolType.PingPong:
                break;
            case PatrolType.Loop:
                break;
            case PatrolType.Stop:
                break;
            default:
                Debug.LogWarning(gameObject.name + " [AIController - Patrol] Unimplemented Patrol Type.");
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

    public bool AimedAtAPlayer()
    {
        // TODO: Implement a method that detects when the ai is aimed at something it should shoot.
        return false;
    }

    public bool CanSee(GameObject targetObject)
    {
        // Check to see if the target is in the field of view.
        float angleToTarget = Vector3.Angle(GetVectorToTarget(targetObject.transform.position), transform.forward);
        if (angleToTarget <= (FOV/2f))
        {
            // Check to see if there are things between us and the target
            RaycastHit hit;
            if (Physics.Raycast(transform.position, targetObject.transform.position, out hit))
            {
                return (hit.collider.gameObject == targetObject);
            }
        }
        return false;
    }
}
