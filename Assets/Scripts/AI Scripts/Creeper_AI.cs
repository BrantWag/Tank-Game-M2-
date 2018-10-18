using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creeper_AI : MonoBehaviour
{

    Controller_AI controller;

    public float lastHealthTotal;
    // Creeper AI gets behind the player and shoots at the target.

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
        lastHealthTotal = controller.data.healthCurrent;
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
        //go behind the player and then shoot at target.
        Vector3 targetPosition = (-(controller.distanceToMaintain * GameManager.instance.players[0].transform.forward));
        // set a distance to maintain 
        if (Vector3.Distance(transform.position, GameManager.instance.players[0].transform.position) <= controller.distanceToMaintain)
        {
            controller.obstacleAvoidanceMove();
            controller.motor.rotateTowards(targetPosition);
        }
        // when far enough away rotate towards player and shoot 
        else
        {
            controller.motor.rotateTowards(GameManager.instance.players[0].transform.position - transform.position);
            controller.motor.ShootMissile();
        }
        // 
        // go into flee state if we get hit
        if (controller.data.healthCurrent < lastHealthTotal)
        {
            lastHealthTotal = controller.data.healthCurrent;
            thisState = states.Flee;
        }
        // Resume patrolling if can not see target or hear target. 
        if (!controller.canHearTarget() && !controller.canSeeTarget())
        {
            thisState = states.Patrol;
        }

    }

    void stateFlee()
    {
        if (controller.timeInFlee <= controller.timeToFlee)
        {
            // run away from player
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
            //go back to patrol state
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
