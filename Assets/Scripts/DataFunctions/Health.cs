using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [HideInInspector] public TankData data;

    // Use this for initialization
    void Start()
    {
        data = GetComponent<TankData>();
    }

    // reduce current health by the damage of the thing that hit it
    public void reduceCurrentHealth(float damage)
    {
        data.healthCurrent -= damage;
        checkDeath();
    }

    // check if player has died
    public void checkDeath()
    {
        if (data.healthCurrent <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Reset current health to max health
    public void resetHealth()
    {
        data.healthCurrent = data.healthMax;
    }

}
