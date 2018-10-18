using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player : MonoBehaviour
{
    [HideInInspector] private TankData data;
    [HideInInspector] private TankMotor motor;

    void Start()
    {
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();

        GameManager.instance.players.Add(this.data);

        // Set mesh color to purple
    }


	void Update ()
    {
        if (Input.GetButton("Fire1"))
        {
            // Shoot machine gun
            motor.ShootBullet();
        }
        if (Input.GetButton("Fire2"))
        {
            // Shoot cannon
            motor.ShootMissile();
        }
        if (Input.GetButton("MoveForward"))
        {
            // Move Forward
            Vector3 movementVector = (Vector3.forward * data.movementSpeed * Time.deltaTime);
            motor.move(movementVector);
        }
        if (Input.GetButton("MoveBackward"))
        {
            // Move Backward
            Vector3 movementVector = (Vector3.forward * data.movementSpeed * Time.deltaTime);
            motor.move(-movementVector);
        }
        if (Input.GetButton("RotateRight"))
        {
            // Move Right
            Vector3 vectorRotation = Vector3.up * data.rotationSpeed * Time.deltaTime;
            motor.rotate(vectorRotation);
        }
        if (Input.GetButton("RotateLeft"))
        {
            // Move Left
            Vector3 vectorRotation = Vector3.up * data.rotationSpeed * Time.deltaTime;
            motor.rotate(-vectorRotation);
        }
    }

    private void OnDestroy()
    {
        // Add to GameManager List players
        GameManager.instance.players.Remove(this.data);
    }
}