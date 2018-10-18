using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_AI : MonoBehaviour
{
    [HideInInspector] public TankData data;
    [HideInInspector] public TankMotor motor;
    public List<Transform> waypoints;
    public int currentWaypoint = 0;

    public float timeInFlee;
    public float timeToFlee;

    public float toClose;
    public float distanceToMaintain;
    public float distanceToMaintainHG;
    public float distanceToMaintainAggro;
    public float distanceToMaintainCreeper;
    public float distanceToMaintainScared;

    public float skittishShootingAngle;

    [HideInInspector]public enum personalities
    {
        creeper,
        scared,
        holdground,
        aggro
    };
    //public personalities personality;

	// Use this for initialization
	void Start ()
    {
        data = GetComponent<TankData>();
        motor = GetComponent<TankMotor>();
        GameManager.instance.aiUnits.Add(this.data);

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Debug line
        Debug.DrawRay(transform.position + (transform.forward * data.wallDetectDistance), -transform.up, Color.red);
        // Debug line 
        Debug.DrawRay(transform.position, transform.forward * data.wallDetectDistance, Color.blue);
    }

    private void OnDestroy()
    {
        // Remove from GameManager 
        GameManager.instance.aiUnits.Remove(this.data);
    }

    public bool canMove()
    {
        // If raycast hits nothing return true
        if (Physics.Raycast(transform.position, transform.forward, data.wallDetectDistance))
        {
            return false;
        }
        // else return false
        return true;
    }

    public bool floorExists()
    {
        // If raycast hits nothing return false
        if (Physics.Raycast(transform.position + (transform.forward * data.wallDetectDistance), -transform.up, data.wallDetectDistance))
        {
            return true;
        }
        // else return true
        return false;
    }

    public bool canSeeTarget()
    {
        // vector to target by getting the target position and subtracting current position
        Vector3 vectorToTarget = (GameManager.instance.players[0].transform.position - transform.position);
        
        float Angle = Vector3.Angle(vectorToTarget, transform.forward);

        // if target is not within field of view, return false
        if (Angle > data.fieldOfView)
        {
            return false;
        }

        // if raycast target hitInfo is null, return false
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, vectorToTarget, out hitInfo, data.viewDistance);
        if (hitInfo.collider == null)
        {
            return false;
        }

        
        Collider targetCollider = GameManager.instance.players[0].GetComponent<Collider>();
        if (targetCollider != hitInfo.collider)
        {
            return false;
        }

        // We can see the player!, return true
        return true;
    }

    public bool canHearTarget()
    {
        float distance = Vector3.Distance(transform.position, GameManager.instance.players[0].transform.position);
        if (distance >= (GameManager.instance.players[0].noiseLevel + data.hearingDistance))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void obstacleAvoidanceMove()
    {
        // If there are no obstacles obstructing our movement path, move forward
        if (canMove())
        {
            // Move forward if there is a floor.
            if (floorExists())
            {
                motor.move(Vector3.forward * data.movementSpeed * Time.deltaTime);
            }
            else
            {
                motor.rotate(Vector3.up * data.rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            motor.rotate(Vector3.up * data.rotationSpeed * Time.deltaTime);
        }
    }
    public void getNextWaypoint()
    {
        int maxWaypoints = GameManager.instance.waypoints.Count - 1;
        if (currentWaypoint < maxWaypoints)
        {
            currentWaypoint++;
        }
        else
        {
            currentWaypoint = 0;
        }
    }
}