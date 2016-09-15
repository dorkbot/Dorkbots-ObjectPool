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
using System;
using System.Collections;

using Dorkbots.UserInput.RaycastInputs.RaycastInputObject;
using Dorkbots.UserInput.RaycastInputs.RaycastInputType;

namespace Dorkbots.UserInput.RaycastInputs.RaycastGameObject
{
	[RequireComponent (typeof(Collider))]

	public class RaycastGameObject : MonoBehaviour 
	{
		[SerializeField]
		private Camera targetCamera;

		public event EventHandler Hit;

		private IRaycastInput raycastInput;

		void Start () 
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				raycastInput = new RaycastInput (targetCamera, new TouchRaycastInput (), this.gameObject);
			}
			else
			{
				raycastInput = new RaycastInput (targetCamera, new MouseRaycastInput (), this.gameObject);
			}
		}

		void Update ()
		{
			if (raycastInput.Check() == RaycastInputResults.HitCorrect) HitEvent(EventArgs.Empty);
		}

		// events
		void HitEvent(EventArgs e)
		{
			EventHandler handler = Hit;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}