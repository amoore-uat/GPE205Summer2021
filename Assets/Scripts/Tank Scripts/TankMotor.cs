using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMotor : MonoBehaviour
{
    private CharacterController m_characterController;

    private void Start()
    {
        m_characterController = gameObject.GetComponent<CharacterController>();
    }

    public void Move(float moveSpeed)
    {
        Vector3 movementVector = transform.forward * moveSpeed;
        m_characterController.SimpleMove(movementVector);
    }

    public void Rotate(float rotateSpeed)
    {
        Vector3 rotationVector = transform.up * rotateSpeed * Time.deltaTime;
        transform.Rotate(rotationVector);
    }
}
