using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankMotor))]
[RequireComponent(typeof(TankData))]
public class SampleController : MonoBehaviour
{
    private TankMotor m_motor;
    private TankData m_data;

    private void Start()
    {
        m_motor = gameObject.GetComponent<TankMotor>();
        m_data = gameObject.GetComponent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        m_motor.Move(m_data.moveSpeed);
        m_motor.Rotate(m_data.rotateSpeed);
    }
}
