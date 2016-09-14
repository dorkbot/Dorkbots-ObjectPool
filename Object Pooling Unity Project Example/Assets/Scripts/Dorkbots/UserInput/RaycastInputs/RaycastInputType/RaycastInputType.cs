using UnityEngine;
using System.Collections;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputType
{
	public abstract class RaycastInputType : IRaycastInputType
	{
		protected Camera camera;

		public RaycastInputType()
		{
		}

		public void Init(Camera camera)
		{
			this.camera = camera;
		}

		public bool ReadyForCheck()
		{
			return PerformReadyForCheck ();
		}


		public Ray GetRay()
		{
			return PerformGetRay();
		}


		protected abstract bool PerformReadyForCheck();
		protected abstract Ray PerformGetRay();
	}
}