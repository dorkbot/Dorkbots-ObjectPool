using UnityEngine;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputOneOption
{
	public abstract class RaycastInput : IRaycastInput
	{
		private Camera camera;
		private IRaycastInputType inputType;


		public RaycastInput(Camera camera, IRaycastInputType inputType)
		{
			this.camera = camera;
			this.inputType = inputType;

			this.inputType.Init (this.camera);
		}


		public RaycastInputResults Check(string direction)
		{
			RaycastInputResults raycastInputResults = RaycastInputResults.HitNone;

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

					raycastInputResults = PerformCheck (hit, direction);
				}
			}

			//Debug.Log("+RaycastInput+ Check -> direction = " + direction);
			//Debug.Log("+RaycastInput+ Check -> raycastInputResults = " + raycastInputResults);

			return raycastInputResults;
		}


		public RaycastInputResults CheckStart()
		{
			if(inputType.ReadyForCheck())
			{	
				Ray ray = inputType.GetRay();
				RaycastHit[] hits = Physics.RaycastAll (ray);

				if (hits.Length > 0) 
				{
					return PerformCheckStart(hits);
				}
			}

			return RaycastInputResults.HitNone;
		}


		protected abstract RaycastInputResults PerformCheck(RaycastHit hit, string direction);
		protected abstract RaycastInputResults PerformCheckStart(RaycastHit[] hits);
	}
}