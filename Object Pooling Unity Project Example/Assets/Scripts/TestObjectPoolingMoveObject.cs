using UnityEngine;
using System.Collections;
using System;

public class TestObjectPoolingMoveObject : MonoBehaviour 
{
	private Vector3 speed = new Vector3 (1, 1, 1);
	private Vector3 direction = new Vector3(0, 0, -1);
	private Vector3 movement;

    private float zVelocity;
	// Use this for initialization
	void Start () 
	{
		
	}

	void OnBecameInvisible () 
	{
       OnBecameInvisibleEvent (EventArgs.Empty);
	}

	void OnEnable()
	{
		Init();
	}

    void OnDestroy()
    {
        BecameInvisibleEvent = null;
    }

    void Update()
    {
        movement = GetComponent<Rigidbody> ().velocity;
        float newZForce = 0;
        if (movement.z > zVelocity)
        {
            newZForce -= movement.z - zVelocity;
        }
        else
        {
            newZForce = zVelocity - movement.z;
        }
            
        movement = new Vector3(movement.x, movement.y, movement.z + newZForce);
        GetComponent<Rigidbody> ().velocity = movement;

    }

    private void Init()
    {
        zVelocity = speed.z * direction.z;
        movement = new Vector3 (speed.x * direction.x, speed.y * direction.y, speed.z * direction.z);
        GetComponent<Rigidbody> ().velocity = movement;
        GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
    }

	//EVENTS
	public event EventHandler BecameInvisibleEvent;
	private void OnBecameInvisibleEvent(EventArgs e)
	{
		EventHandler handler = BecameInvisibleEvent;
		if (handler != null)
		{
			handler(this, e);
		}
	}
}
