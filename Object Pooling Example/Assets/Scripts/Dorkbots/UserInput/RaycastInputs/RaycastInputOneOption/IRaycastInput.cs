using UnityEngine;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastInputOneOption
{
	public interface IRaycastInput
	{
		RaycastInputResults Check(string direction);
		RaycastInputResults CheckStart();
	}
}