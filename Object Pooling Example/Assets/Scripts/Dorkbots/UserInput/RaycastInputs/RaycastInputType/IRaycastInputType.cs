using UnityEngine;
using System.Collections;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputType
{
	public interface IRaycastInputType
	{
		void Init (Camera camera);
		bool ReadyForCheck();
		Ray GetRay(); 
	}
}