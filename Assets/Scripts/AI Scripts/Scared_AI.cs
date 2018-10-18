using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scared_AI : MonoBehaviour
{

    Controller_AI controller;

    Vector3 lastKnownPosition;

    // Scared AI wil always try and flee from the target and shoot a bullet. 

    public enum states
    {
        Chase,
        Flee,
        Patrol
    };
    public states thisState;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<Controller_AI>();
       
        thisState = states.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        switch (thisState)
        {
            case states.Chase:
                stateChase();
                break;
            case states.Flee:
                stateFlee();
                break;
            case states.Patrol:
                statePatrol();
                break;
        }
    }

    void stateChase()
    {
        // go to last known target position if we don't see target
        if (!controller.canSeeTarget() && !controller.canHearTarget())
        {
            if (Vector3.Distance(transform.position, lastKnownPosition) >= controller.toClose)
            {
                controller.motor.rotateTowards(lastKnownPosition - transform.position);
                controller.obstacleAvoidanceMove();
            }
            else
            {
                thisState = states.Patrol;
            }
        }
        // if we see the target shoot at him and go into flee state
        else
        {
            controller.motor.rotateTowards(GameManager.instance.players[0].transform.position - transform.position);
            if (Vector3.Angle(transform.position, GameManager.instance.players[0].transform.position) < controller.skittishShootingAngle)
            {
                controller.motor.ShootMissile();
                lastKnownPosition = GameManager.instance.players[0].transform.position;
                thisState = states.Flee;
            }
        }
    }

    void stateFlee()
    {
        if (controller.timeInFlee <= controller.timeToFlee)
        {
            // flee away from player
            Vector3 directionToFlee = -(GameManager.instance.players[0].transform.position - transform.position);
            if (controller.canMove())
            {
                controller.obstacleAvoidanceMove();
                controller.motor.rotateTowards(directionToFlee);
            }
            controller.obstacleAvoidanceMove();
            // increase time for flee
            controller.timeInFlee += Time.deltaTime;
        }
        else
        {
            controller.timeInFlee = 0;
            // go back to patrol state
            thisState = states.Chase;
        }
    }

    void statePatrol()
    {
        // Will "patrol" for the player by rotating around in spot
        controller.motor.rotate(Vector3.up * controller.data.rotationSpeed * Time.deltaTime);
        // flee but remember where the noise and where it saw the target from
        if (controller.canSeeTarget() || controller.canHearTarget())
        {
            // location of the noise
            lastKnownPosition = GameManager.instance.players[0].transform.position;
            // go into flee state
            thisState = states.Flee;
        }
    }
}
