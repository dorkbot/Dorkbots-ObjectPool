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
using System.Collections.Generic;
using System;

namespace Dorkbots.UserInput.ClickableGameObjects
{
	[RequireComponent (typeof(Collider))]

	public class ClickableGameObject : MonoBehaviour 
	{
		public event EventHandler MouseEnter;
		public event EventHandler MouseDown;
		public event EventHandler MouseUp;
		public event EventHandler MouseExit;

		// mouse triggers
		void OnMouseEnter() 
		{
			MouseEnterEvent(EventArgs.Empty);
		}

		void OnMouseDown() 
		{
			MouseDownEvent(EventArgs.Empty);
		}

		void OnMouseUp() 
		{
			MouseUpEvent(EventArgs.Empty);
		}

		void OnMouseExit() 
		{
			MouseExitEvent(EventArgs.Empty);
		}

		// events
		private void MouseEnterEvent(EventArgs e)
		{
			EventHandler handler = MouseEnter;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void MouseDownEvent(EventArgs e)
		{
			EventHandler handler = MouseDown;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void MouseUpEvent(EventArgs e)
		{
			EventHandler handler = MouseUp;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void MouseExitEvent(EventArgs e)
		{
			EventHandler handler = MouseExit;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}