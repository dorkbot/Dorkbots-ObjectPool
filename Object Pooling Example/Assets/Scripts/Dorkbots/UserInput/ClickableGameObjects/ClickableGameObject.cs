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