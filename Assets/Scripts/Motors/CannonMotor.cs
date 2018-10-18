using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonMotor : MonoBehaviour
{

    [HideInInspector] public AmmoData data;
    [HideInInspector] public Rigidbody rb;

    // Use this for initialization
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        data = GetComponent<AmmoData>();

        // Destroy this projectile after designer set amount of time 
        Destroy(this.gameObject, data.projectileLifespan);
        pushForward();
    }

    // When a bullet collides with something destroy itself.
    private void OnTriggerEnter(Collider other)
    {
        // check to see if the target is either a player tank or an enemy tank
        if (GameManager.instance.players.Contains(other.gameObject.GetComponent<TankData>()) ||
            GameManager.instance.aiUnits.Contains(other.gameObject.GetComponent<TankData>()))
        {
            //reduce health if it hit player
            other.gameObject.GetComponent<Health>().reduceCurrentHealth(data.projectileDamage);
        }
        // destroy the bullet
        Destroy(this.gameObject);
    }

    // Pushes a bullet forward
    public void pushForward()
    {
        rb.AddForce(transform.forward * data.projectileSpeed);
    }
}
