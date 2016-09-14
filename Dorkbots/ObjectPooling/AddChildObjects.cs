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