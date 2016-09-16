/*
MIT License

Copyright (c) 2016 Dayvid Jones

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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