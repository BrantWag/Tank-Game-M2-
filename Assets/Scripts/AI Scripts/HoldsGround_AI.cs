using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldsGround_AI : MonoBehaviour
{

    Controller_AI controller;

    // Hold ground does exactly how its named, it holds it ground and shoot at the target.

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
        // hold ground and shoot at the target
        Vector3 targetLocation = GameManager.instance.players[0].transform.position - transform.position;
        // rotate to target position
        controller.motor.rotateTowards(targetLocation);
        // keep firing
        controller.motor.ShootMissile();

        
        // go into flee state if health is below 50%
        if (controller.data.healthCurrent <= (controller.data.healthMax / 2)) // TODO: Probably change to a percentage threshhold, but we will leave at 1/2 for now
        {
            thisState = states.Flee;
        }
        //resume patrolling if can not see target or hear target. 
        if (!controller.canHearTarget() && !controller.canSeeTarget())
        {
            thisState = states.Patrol;
        }
    }

    void stateFlee()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, GameManager.instance.players[0].transform.position);
        if (controller.distanceToMaintain >= distanceFromTarget)
        {
            // run away from target
            Vector3 directionToFlee = -(GameManager.instance.players[0].transform.position - transform.position);
            if (controller.canMove())
            {
                controller.obstacleAvoidanceMove();
                controller.motor.rotateTowards(directionToFlee);
            }
            controller.obstacleAvoidanceMove();
        }
        else
        {
            // back to chase state
            thisState = states.Chase;
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
        if (controller.canHearTarget() || controller.canSeeTarget())
        {
            // go into chase state
            thisState = states.Chase;
        }
    }
}
