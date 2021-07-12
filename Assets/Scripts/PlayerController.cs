using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankShooter))]
public class PlayerController : MonoBehaviour
{
    private TankData m_data;
    private TankMotor m_motor;
    private TankShooter m_shooter;
    public enum InputScheme { WASD, ArrowKeys };
    public InputScheme m_playerInputScheme = InputScheme.WASD;

    private void Start()
    {
        m_data = gameObject.GetComponent<TankData>();
        m_motor = gameObject.GetComponent<TankMotor>();
        m_shooter = gameObject.GetComponent<TankShooter>();
    }

    private void Update()
    {
        switch (m_playerInputScheme)
        {
            case InputScheme.ArrowKeys:
                m_motor.Move(0f);
                break;
            case InputScheme.WASD:
                // Handle Movement
                if (Input.GetKey(KeyCode.W))
                {
                    m_motor.Move(m_data.moveSpeed);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    m_motor.Move(-m_data.moveSpeed);
                }
                else // We need to pass in a movement speed of 0 to ensure gravity is being used.
                {
                    m_motor.Move(0f);
                }
                // Handle Rotation
                if (Input.GetKey(KeyCode.A))
                {
                    m_motor.Rotate(-m_data.rotateSpeed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    m_motor.Rotate(m_data.rotateSpeed);
                }
                // Handle Shooting
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_shooter.Shoot();
                }
                break;
            default:
                Debug.LogWarning("[PlayerController] Unimplemented Input Scheme used.");
                break;

        }
    }
}
