using UnityEngine;
using System;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputObject;
using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastGameObject
{
	[RequireComponent (typeof(Collider))]

	public class RaycastGameObject : MonoBehaviour 
	{
		[SerializeField]
		private Camera targetCamera;

		public event EventHandler Hit;

		private IRaycastInput raycastInput;

		void Start () 
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				raycastInput = new RaycastInput (targetCamera, new TouchRaycastInput (), this.gameObject);
			}
			else
			{
				raycastInput = new RaycastInput (targetCamera, new MouseRaycastInput (), this.gameObject);
			}
		}

		void Update ()
		{
			if (raycastInput.Check() == RaycastInputResults.HitCorrect) HitEvent(EventArgs.Empty);
		}

		// events
		void HitEvent(EventArgs e)
		{
			EventHandler handler = Hit;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}