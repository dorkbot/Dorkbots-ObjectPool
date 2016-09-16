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

using System;
using UnityEngine;

namespace Dorkbots.ObjectPooling
{
	public interface IObjectPools
	{
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
        IObjectPool CreateOrGetPool(GameObject objToPool, uint initialPoolSize = 0, uint maxPoolSize = 0, bool destroyOnLoad = true);
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
        IObjectPool CreateOrGetPoolConfirm(GameObject objToPool, out bool poolCreated, uint initialPoolSize = 0, uint maxPoolSize = 0, bool destroyOnLoad = true);
        /// <summary>
        /// Returns an Object Pool. Use this to access more features in the Object Pool.
        /// </summary>
        /// <param name="objName">String name of the object and its pool you wish to have access to.</param>
        /// <returns>An ObjectPool.</returns>
		IObjectPool GetPool(string objName);
        /// <summary>
        /// Removes a pool from the managed list. Use this to dispose of an ObjectPool. Stops attached AutoShrink.
        /// </summary>
        /// <param name="objName">String name of the object that the pool is for.</param>
        /// <param name="destroy">Destroy any objects. The Default is 'true'</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Object Pool can not be found. Defaul is false.</param>
        void RemovePool(string objName, bool destroy = true, bool strict = false);
        /// <summary>
        /// Removes all pools from the list. Use this to dispose of ObjectPools. Stops attached AutoShrink.
        /// </summary>
        /// <param name="destroy">Destroy any objects. The Default is 'true'</param>
		void RemoveAllPools(bool destroy = true);
        /// <summary>
        /// Iterate through all pools and releases as many objects as possible until the pool sizes are back to the initial default size.
        /// </summary>
        /// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
        /// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
		void ShrinkAllPools(bool removeActive = true, bool destroy = true);
        /// <summary>
        /// Iterate through all the pools and Destroy all GameObjects.
        /// </summary>
        /// <param name="removeAutoShrink">Sets if the attached AutoShrink should be removed. The default is 'false'.</param>
        void DestroyAllObjectsInPools(bool removeAutoShrink = false);
        /// <summary>
        /// Iterate through all the pools and clear them of objects. Does not destroy objects. Use 'DestroyAllObjectsInPools' to destroy all objects.
        /// </summary>
		void ClearAllPools();

        /// <summary>
        /// Returns an active object from the object pool without resetting any of its values. 
        /// You will need to set its values and set it inactive again when you are done with it.
        /// </summary>
        /// <param name="objName">String name of the object you wish to have access to.</param>
        /// <returns>Game Object of requested type if it is available, otherwise null.</returns>
		GameObject GetObject(string objName);
        /// <summary>
        /// Use this to activate an object, this will make sure that the object is sorted properly.
        /// </summary>
        /// <param name="gameObject">Game Object to activate.</param>
		void ActivateObject(GameObject gameObject);
        /// <summary>
        /// Use this to deactivate an object, this will make sure that the object is sorted properly.
        /// </summary>
        /// <param name="gameObject">Game Object to deactivate.</param>
		void DeactivateObject(GameObject gameObject);

        /// <summary>
        /// Logs all pool stats.
        /// </summary>
		void LogStats();
	}
}