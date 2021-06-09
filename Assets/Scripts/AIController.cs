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
    public AvoidanceState avoidance = AvoidanceState.NotAvoiding;
    public GameObject[] m_waypoints;
    public float closeEnough = 1f; // A distance from a location that is close enough to consider ourselves there.
    public float avoidanceTime = 1f;
    private float avoidanceTimer;

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
        if (AimedAtAPlayer())
        {
            m_shooter.Shoot();
        }
        MoveAwayFrom(new Vector3(0, 0, 0));
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
                            avoidance = AvoidanceState.RotatingToAvoid;
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
                    avoidance = AvoidanceState.MovingToAvoid;
                    avoidanceTimer = avoidanceTime;
                }
                break;
            case AvoidanceState.MovingToAvoid:
                
                // if timer runs down, switch to not avoiding
                if (avoidanceTimer <= 0)
                {
                    avoidance = AvoidanceState.NotAvoiding;
                }
                // if we're about to run into something, switch to rotating
                else if (!CanMove(m_data.moveSpeed))
                {
                    avoidance = AvoidanceState.RotatingToAvoid;
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
        // TODO: Implement a method that detects if the ai can see a object
        return false;
    }
}
