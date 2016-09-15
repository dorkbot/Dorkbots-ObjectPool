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

namespace Dorkbots.ObjectPooling
{
	public class AddChildObjects
	{
		/// <summary>
		/// From a parent GameObject, add child objects created outside of the pools. Use with caution! This can cause issues, ex: If the objects are added and then destroyed, 
		/// or destroyed first then added, then the pool will be out of sync. This method can lead to the creation of Object Pools if any Object Pools do not already exist. 
        /// Use 'ObjectPools.GetPools' to grab a Object Pool and change its properties directly if needed.
		/// </summary>
		/// <param name="parent">GameObject the contains the child objects</param>
		/// <param name="priority">If set to true any other inactive or active game objects will be removed to make room.</param>
		/// <param name="destroy">If set to true any other inactive or active game objects will be destroyed to make room.</param>
		/// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
		public static void Add(GameObject parent, bool priority = true, bool destroy = true, bool removeActive = true)
		{
			IObjectPools objectPools = ObjectPoolingManager.Instance.objectPools;

			IObjectPool objectPool;
			GameObject gameObject;
			int childCount = parent.transform.childCount;
			for (int i = 0; i < childCount; i++) 
			{
				gameObject = parent.transform.GetChild (i).gameObject;
                objectPool = objectPools.CreateOrGetPool (gameObject);
				objectPool.AddObject (gameObject, priority, destroy, removeActive);
			}
		}
	}
}