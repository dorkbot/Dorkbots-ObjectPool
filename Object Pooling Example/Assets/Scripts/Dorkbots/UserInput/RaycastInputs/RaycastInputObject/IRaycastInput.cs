using UnityEngine;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputObject
{
	public interface IRaycastInput
	{
		RaycastInputResults Check();
	}
}