using UnityEngine;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputObject
{
	public class RaycastInput : IRaycastInput
	{
		/// <summary>
		/// Use this class for code centric set up. Use RaycastGameObject to attach directly to a GameObject.
		/// </summary>
		/// 
		private Camera camera;
		private IRaycastInputType inputType;
		private GameObject gameObject;

		public RaycastInput(Camera camera, IRaycastInputType inputType, GameObject gameObject)
		{
			this.camera = camera;
			this.inputType = inputType;
			if (gameObject.GetComponent<Collider> () == null) 
			{
				Debug.LogError("The supplied Game Object does not have a collider. A Collider component must be attached!");
			}
			this.gameObject = gameObject;

			this.inputType.Init (this.camera);
		}

		public RaycastInputResults Check()
		{
			if(inputType.ReadyForCheck())
			{		
				// TODO: not sure why a layerMask isn't working
				//int layerMask = 1 << 13;
				//LayerMask layerMask = LayerMask.NameToLayer ("GamePlayControlUI");

				RaycastHit hit;
				Ray ray = inputType.GetRay();

				if (Physics.Raycast (ray, out hit)) 
					//if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) 
				{
					//Debug.Log("+RaycastInput+ Check -> You just hit " + hit.collider.gameObject.name);

					return PerformCheck (hit);
				}
			}

			return RaycastInputResults.HitNone;
		}

		private RaycastInputResults PerformCheck(RaycastHit hit)
		{
			if (hit.collider.gameObject == gameObject)
			{
				return RaycastInputResults.HitCorrect;
			}
			else
			{
				return RaycastInputResults.HitIncorrect;
			}
		}
	}
}