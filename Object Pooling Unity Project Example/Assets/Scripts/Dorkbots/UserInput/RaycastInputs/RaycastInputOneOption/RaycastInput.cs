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