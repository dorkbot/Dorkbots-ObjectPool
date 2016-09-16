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
	// This class manages object pools
	public class ObjectPools : IObjectPools
	{
		//look up list of various object pools.
		private Dictionary<String, IObjectPool> objectPools;

		public ObjectPools()
		{
			this.objectPools = new Dictionary<String, IObjectPool>();
		}

		/// <summary>
        /// Creates a new Object Pool for the prefab object you wish to pool, or returns an existing Object Pool that has already been created for the prefab object. 
        /// If an Object Pool has already been created, the Object Pool property arguments are ignored, however you can directly change the returned Object Pool's properties if needed. 
        /// To confirm if a new Object Pool was created or not, use the method 'CreateOrGetPoolConfirm' that includes an 'out' boolean response.
		/// </summary>
		/// <param name="objToPool">The object you wish to pool. The name property of the object MUST be unique.</param>
        /// <param name="initialPoolSize">Initial and default size of the pool. The default is '0'.</param>
		/// <param name="maxPoolSize">Maximum number of objects this pool can contain. '0' is the default and sets the Poll to grow when needed.</param>
		/// <param name="destroyOnLoad">Set GameObjects to not be destroyed when a new scene is loaded. Default is set to 'true'</param>
		/// <returns>Returns the ObjectPool.</returns>
        public IObjectPool CreateOrGetPool(GameObject objToPool, uint initialPoolSize = 0, uint maxPoolSize = 0, bool destroyOnLoad = true)
		{
            IObjectPool objectPool;
            //Check to see if the pool already exists.
            if ( !objectPools.TryGetValue( objToPool.name.Replace( "(Clone)", "" ), out objectPool ) )
            {
                //No pool found. Create a new pool using the properties
                objectPool = new ObjectPool(objToPool, initialPoolSize, maxPoolSize, destroyOnLoad);
                //Add the pool to the dictionary of pools to manage 
                //using the object name as the key and the pool as the value.
                objectPools.Add(objToPool.name, objectPool);
			}

            return objectPool;
		}

        /// <summary>
        /// Creates a new Object Pool for the prefab object you wish to pool, or returns an existing Object Pool that has already been created for the prefab object. 
        /// If an Object Pool has already been created, the Object Pool property arguments are ignored, however you can directly change the returned Object Pool's properties if needed. 
        /// </summary>
        /// <param name="objToPool">The object you wish to pool.  The name property of the object MUST be unique for each prefab.</param>
        /// <param name="poolCreated">This is an out paramter. 'False' = An Object Pool was not created because an Object Pool already exist for this prefab. 
        /// 'True' = An Object Pool was created for this prefab.</param>
        /// <param name="initialPoolSize">Initial and default size of the pool. The default is '0'.</param>
        /// <param name="maxPoolSize">Maximum number of objects this pool can contain. '0' is the default and sets the Poll to grow when needed.</param>
        /// <param name="destroyOnLoad">Set GameObjects to not be destroyed when a new scene is loaded. Default is set to 'true'</param>
        /// <returns>Returns the ObjectPool.</returns>
        public IObjectPool CreateOrGetPoolConfirm(GameObject objToPool, out bool poolCreated, uint initialPoolSize = 0, uint maxPoolSize = 0, bool destroyOnLoad = true)
        {
            IObjectPool objectPool;
            //Check to see if the pool already exists.
            if ( objectPools.TryGetValue( objToPool.name.Replace( "(Clone)", "" ), out objectPool ) )
            {
                //let the caller know it already exists, just use the pool out there.
                poolCreated = false;
            }
            else
            {
                //create a new pool using the properties
                objectPool = new ObjectPool(objToPool, initialPoolSize, maxPoolSize, destroyOnLoad);
                //Add the pool to the dictionary of pools to manage 
                //using the object name as the key and the pool as the value.
                objectPools.Add(objToPool.name, objectPool);
                //We created a new pool!
                poolCreated = true;
            }

            return objectPool;
        }

        /// <summary>
        /// Returns an Object Pool. Use this to access more features in the Object Pool.
        /// </summary>
        /// <param name="objName">String name of the object and its pool you wish to have access to.</param>
        /// <returns>An ObjectPool.</returns>
        public IObjectPool GetPool(string objName)
        {
            IObjectPool objectPool;
            if (objectPools.TryGetValue(objName.Replace("(Clone)", ""), out objectPool))
            {
                return objectPool;
            }
            else
            {
                Debug.LogError("Use 'CreateOrGetPool' or 'CreateOrGetPoolConfirm' before using GetPool. A pool needs to be created first before the method GetPool can be used for this prefab name!");
            }

            return null;
        }

        /// <summary>
        /// Removes a pool from the managed list. Use this to dispose of an ObjectPool. Stops attached AutoShrink.
		/// </summary>
		/// <param name="objName">String name of the object that the pool is for.</param>
		/// <param name="destroy">Destroy any objects. The Default is 'true'</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Object Pool can not be found. Defaul is false.</param>
        public void RemovePool(string objName, bool destroy = true, bool strict = false)
		{
            objName = objName.Replace("(Clone)", "");
            IObjectPool objPool;
            if (objectPools.TryGetValue(objName, out objPool))
            {
                objPool.DestroyPool (destroy);

                objectPools.Remove(objName);
            }
            else
            {
                if (strict) Debug.LogError("Object Pool for Object Name does not exist!");
            }
		}

		/// <summary>
        /// Removes all pools from the list. Use this to dispose of ObjectPools. Stops attached AutoShrink.
		/// </summary>
		/// <param name="destroy">Destroy any objects. The Default is 'true'</param>
		public void RemoveAllPools(bool destroy = true)
		{
            IObjectPool objPool;
			foreach (KeyValuePair<String, IObjectPool> pair in objectPools)
			{
                objPool = pair.Value;
				objPool.DestroyPool(destroy);
			}

			objectPools.Clear ();
		}
			
		/// <summary>
		/// Iterate through all pools and releases as many objects as possible until the pool sizes are back to the initial default size.
		/// </summary>
		/// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
		/// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
		public void ShrinkAllPools(bool removeActive = true, bool destroy = true)
		{
			foreach (KeyValuePair<String, IObjectPool> pair in objectPools)
			{
				pair.Value.Shrink (removeActive, destroy);
			}
		}

		/// <summary>
		/// Iterate through all the pools and Destroy all GameObjects.
		/// </summary>
        /// <param name="removeAutoShrink">Sets if the attached AutoShrink should be removed. The default is 'false'.</param>
        public void DestroyAllObjectsInPools(bool removeAutoShrink = false)
		{
			foreach (KeyValuePair<String, IObjectPool> pair in objectPools)
			{
                pair.Value.DestroyPool (removeAutoShrink);
			}
		}

		/// <summary>
		/// Iterate through all the pools and clear them of objects. Does not destroy objects. Use 'DestroyAllObjectsInPools' to destroy all objects.
		/// </summary>
		public void ClearAllPools()
		{
			foreach (KeyValuePair<String, IObjectPool> pair in objectPools)
			{
                pair.Value.RemoveObjects ();
			}
		}

		/// <summary>
		/// Returns an active object from the object pool without resetting any of its values. 
		/// You will need to set its values and set it inactive again when you are done with it.
		/// </summary>
		/// <param name="objName">String name of the object you wish to have access to.</param>
		/// <returns>Game Object of requested type if it is available, otherwise null.</returns>
		public GameObject GetObject(string objName)
		{
            IObjectPool objectPool;
            if (objectPools.TryGetValue(objName.Replace("(Clone)", ""), out objectPool))
            {
                return objectPool.GetObject ();
            }
            else
            {
                Debug.LogError("Use CreatePool before using GetObject. A pool needs to be created first before the method GetObject can be used for this prefab name!");
            }

			return null;
		}

		/// <summary>
		/// Use this to activate an object, this will make sure that the object is sorted properly.
		/// </summary>
		/// <param name="gameObject">Game Object to activate.</param>
		public void ActivateObject(GameObject gameObject)
		{
            IObjectPool objectPool;
            if (objectPools.TryGetValue(gameObject.name.Replace("(Clone)", ""), out objectPool))
            {
                objectPool.ActivateObject(gameObject);
            }
            else
            {
                Debug.LogError("Use CreatePool before using ActivateObject. A pool needs to be created first before the method ActivateObject can be used for this prefab name!");
            }
		}

		/// <summary>
		/// Use this to deactivate an object, this will make sure that the object is sorted properly.
		/// </summary>
		/// <param name="gameObject">Game Object to deactivate.</param>
		public void DeactivateObject(GameObject gameObject)
		{
            IObjectPool objectPool;
            if (objectPools.TryGetValue(gameObject.name.Replace("(Clone)", ""), out objectPool))
            {
                objectPool.DeactivateObject(gameObject);
            }
            else
            {
                Debug.LogError("Use CreatePool before using DeactivateObject. A pool needs to be created first before the method DeactivateObject can be used for this prefab name!");
            }
		}

		/// <summary>
		/// Logs all pool stats.
		/// </summary>
		public void LogStats()
		{
            Debug.Log("++++BEGIN OBJECT POOLS STATS++++");
            uint allInactive = 0;
            uint allActive = 0;
            uint allTotal = 0;
			foreach (KeyValuePair<String, IObjectPool> pair in objectPools)
			{
                allInactive += pair.Value.inactiveCount;
                allActive += pair.Value.activeCount;
                allTotal += pair.Value.count;
				Debug.Log ("NAME = " + pair.Key + " | INACTIVE = " + pair.Value.inactiveCount + " | ACTIVE = " + pair.Value.activeCount + " | TOTAL = " + pair.Value.count);
			}

            Debug.Log ("ALL INACTIVE = " + allInactive + " | ALL ACTIVE = " + allActive + " | ALL TOTAL = " + allTotal);
		}
	}
}