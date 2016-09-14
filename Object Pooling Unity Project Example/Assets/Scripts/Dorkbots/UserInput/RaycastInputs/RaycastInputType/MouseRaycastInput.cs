using UnityEngine;
using System.Collections;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputType
{
	public class MouseRaycastInput : RaycastInputType, IRaycastInputType
	{
		public MouseRaycastInput()
		{
			
		}


		protected override bool PerformReadyForCheck ()
		{
			if(Input.GetMouseButtonDown(0))
			{
				return true;
			}

			return false;
		}


		protected override Ray PerformGetRay ()
		{
			return camera.ScreenPointToRay(Input.mousePosition);
		}
	}
}