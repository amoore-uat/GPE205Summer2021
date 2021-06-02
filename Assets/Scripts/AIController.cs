using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]
public class AIController : MonoBehaviour
{
    public enum LoopType { PingPong, Stop, Loop};
    public GameObject[] m_waypoints;
    public float closeEnough = 1f; // A distance from a location that is close enough to consider ourselves there.
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
        LoopThroughWaypoints(LoopType.PingPong);
    }

    // Move towards a target position.
    public void MoveTowards(Vector3 targetPosition)
    {
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
                m_motor.Move(m_data.moveSpeed);
            }
        }
        
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        // Get the vector to the target.
        Vector3 vectorToTarget = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_data.rotateSpeed * Time.deltaTime);
    }

    public bool IsFacing(Vector3 targetPosition)
    {
        // Get the vector to the target.
        Vector3 vectorToTarget = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        return (targetRotation == transform.rotation);
    }

    public bool IsCloseEnough(Vector3 targetPosition)
    {
        return (Vector3.SqrMagnitude(targetPosition - transform.position) <= (closeEnough * closeEnough));
    }

    public void LoopThroughWaypoints(LoopType loopType)
    {
        if (m_waypoints.Length < 1)
        {
            Debug.LogWarning(gameObject.name + " [AIController - LoopThroughWaypoints] Attempting to loop through empty array of waypoints.");
            return;
        }
        switch (loopType)
        {
            case LoopType.PingPong:
                break;
            case LoopType.Loop:
                break;
            case LoopType.Stop:
                break;
            default:
                Debug.LogWarning(gameObject.name + " [AIController] Unimplemented Loop Type.");
                break;
        }
    }
}
