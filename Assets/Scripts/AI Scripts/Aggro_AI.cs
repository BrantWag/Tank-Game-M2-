using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro_AI : MonoBehaviour
{

    Controller_AI controller;

    // aggro AI tracks down 
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
        // FSM
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
        // if target is within range, stop moving, but keep shooting at the player
        Vector3 targetLocation = GameManager.instance.players[0].transform.position - transform.position;

        if (Vector3.Distance(GameManager.instance.players[0].transform.position, transform.position) >= controller.distanceToMaintain)
        {
            if (controller.canMove())
            {
                controller.obstacleAvoidanceMove();
                controller.motor.rotateTowards(targetLocation);
            }
            controller.obstacleAvoidanceMove();
            controller.motor.ShootMissile();
        }
        else
        {
            controller.motor.rotateTowards(targetLocation);
            controller.motor.ShootMissile();
        }
        if (controller.data.healthCurrent <= (controller.data.healthMax / 2)) 
        {
            thisState = states.Flee;
        }
        if (!controller.canHearTarget() && !controller.canSeeTarget())
        {
            thisState = states.Patrol;
        }
    }

    void stateFlee()
    {
        if (controller.timeInFlee <= controller.timeToFlee)
        {
            // run away from target
            Vector3 directionToFlee = -(GameManager.instance.players[0].transform.position - transform.position);
            if (controller.canMove())
            {
                controller.obstacleAvoidanceMove();
                controller.motor.rotateTowards(directionToFlee);
            }
            controller.obstacleAvoidanceMove();
            // increase the time for fleeing
            controller.timeInFlee += Time.deltaTime;
        }
        else
        {
            controller.timeInFlee = 0;
            // go back to patrol state
            thisState = states.Patrol;
        }
    }

    void statePatrol()
    {
        // Patrol between waypoints
        Vector3 targetPosition = new Vector3(GameManager.instance.waypoints[controller.currentWaypoint].position.x, transform.position.y, GameManager.instance.waypoints[controller.currentWaypoint].position.z);
        Vector3 dirToWaypoint = targetPosition - transform.position;
        if (controller.canMove())
        {
            controller.obstacleAvoidanceMove();
            controller.motor.rotateTowards(dirToWaypoint);
        }
        controller.obstacleAvoidanceMove();

        if (Vector3.Distance(transform.position, targetPosition) <= controller.toClose)
        {
            controller.getNextWaypoint();
        }
        if (controller.canSeeTarget() || controller.canHearTarget())
        {
            // go into chase state
            thisState = states.Chase;
        }
    }
}
