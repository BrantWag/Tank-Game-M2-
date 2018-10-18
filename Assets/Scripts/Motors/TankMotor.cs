using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMotor : MonoBehaviour
{
    public Transform firingPoint;
    public GameObject bulletPrefab;
    public GameObject missilePrefab;

    [HideInInspector] public TankData data;

    private void Start()
    {
        data = GetComponent<TankData>();
    }

    // Reduce current cooldowns, so the skill can eventually be used again
    private void Update()
    {
        if (data.missileCooldownCurrent >= 0)
        {
            data.missileCooldownCurrent -= Time.deltaTime;
        }
        if (data.bulletCooldownCurrent >= 0)
        {
            data.bulletCooldownCurrent -= Time.deltaTime;
        }
        if (data.noiseLevel >= 0)
        {
            data.noiseLevel -= data.noiseLevelReducPerSec;
        }
    }

    // move the character 
    public void move(Vector3 movement)
    {
        transform.Translate(movement);
        data.noiseLevel = data.moveNoiseLevel;
    }

    // rotate the character 
    public void rotate(Vector3 rotation)
    {
        transform.Rotate(rotation);
        data.noiseLevel = data.rotateNoiseLevel;
    }

    // check cooldown 
    bool checkCooldown(float cooldown)
    {
        if (cooldown >= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // shoots bullets
    public void ShootBullet()
    {
        // checks if the cooldown is ready
        if (checkCooldown(data.bulletCooldownCurrent))
        {
            // set cooldown to the designer input
            data.bulletCooldownCurrent = data.bulletCooldownMax;
            // create a bullet to be fired 
            var bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
            
            bullet.GetComponent<AmmoData>().shooterName = this.data.myName;

            // Set noiseLevel
            data.noiseLevel = data.bulletNoiseLevel;
        }
    }

    // shoots missiles
    
    public void ShootMissile()
    {
        if (checkCooldown(data.missileCooldownCurrent))
        {
            data.missileCooldownCurrent = data.missileCooldownMax;
            var missile = Instantiate(missilePrefab, firingPoint.position, firingPoint.rotation);
            missile.GetComponent<AmmoData>().shooterName = this.data.myName;
        }
        data.noiseLevel = data.missileNoiseLevel;
    }

    public void rotateTowards(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, data.rotationSpeed * Time.deltaTime);
        data.noiseLevel = data.rotateNoiseLevel;
    }
}
