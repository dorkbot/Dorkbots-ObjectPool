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

namespace Dorkbots.ObjectPooling
{
	// this manages other pools (pool managers).
	public class ObjectPoolingManager
	{
		//the variable is declared to be volatile to ensure that 
		//assignment to the instance variable completes before the 
		//instance variable can be accessed.
		private static ObjectPoolingManager instance;

		//the real pool manager
		private IObjectPools _objectPools;
		public IObjectPools objectPools 
		{
			get{return _objectPools;}
		}

		//object for locking
		private static object syncRoot = new System.Object();

		// this singleton approach allows for threading.
		public static ObjectPoolingManager Instance
		{
			get
			{
				if(instance == null)
				{
					lock (syncRoot)
					{
						if(instance == null)
							instance = new ObjectPoolingManager();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Constructor for the class.
		/// </summary>
		private ObjectPoolingManager()
		{
			//Ensure object pools exists.
			this._objectPools = new ObjectPools();
		}
	}
}