using UnityEngine;
using System.Collections;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputType
{
	public class TouchRaycastInput : RaycastInputType, IRaycastInputType
	{
		public TouchRaycastInput()
		{
			
		}


		protected override bool PerformReadyForCheck ()
		{
			for (var i = 0; i < Input.touchCount; ++i) 
			{
				if (Input.GetTouch (i).phase == TouchPhase.Began) 
				{
					return true;
				}
			}
			
			return false;
		}


		protected override Ray PerformGetRay ()
		{
			return camera.ScreenPointToRay(Input.GetTouch(0).position);
		}
	}
}