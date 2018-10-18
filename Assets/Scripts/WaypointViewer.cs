using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointViewer : MonoBehaviour
{
    public bool showSprites;
    public List<SpriteRenderer> waypoints;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        enableWaypointSprites();
    }

    void enableWaypointSprites()
    {
        if (showSprites == true)
        {
            foreach (SpriteRenderer waypoint in waypoints)
            {
                waypoint.enabled = true;
            }
        }
        else
        {
            foreach (SpriteRenderer waypoint in waypoints)
            {
                waypoint.enabled = false;
            }
        }
    }
}
