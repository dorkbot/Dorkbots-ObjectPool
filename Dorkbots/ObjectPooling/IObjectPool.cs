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
	public interface IObjectPool
	{
		uint maxPoolSize{get; set;}
		uint initialPoolSize{get; set;}
		uint count{get;}
		uint activeCount{get;}
		uint inactiveCount{get;}
		string objectName{ get; }

        /// <summary>
        /// Returns an active object from the object pool without resetting any of its values. 
        /// Use 'ActivateObject', 'DeactivateObject' and 'DestroyObject' to insure Object Pool accuracy and effectiveness.
		/// 
		/// Also, this method will injected the GameObject's ObjectPool into any attached MonoBehavior class that implements IObjectForPool.
        /// </summary>
        /// <param name="activate">Set whether the game object is set to active.</param>
        /// <returns>Game Object of requested type if it is available, otherwise null.</returns>
        GameObject GetObject(bool activate = true);
        /// <summary>
        /// Use this to activate an object, this will make sure that the object is sorted properly.
        /// </summary>
        /// <param name="gameObject">Game Object to activate.</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Game Object can not be found. Default is 'true'.</param>
        void ActivateObject(GameObject gameObject, bool strict = true);
        /// <summary>
        /// Use this to deactivate an object, this will make sure that the object is sorted properly.
        /// </summary>
        /// <param name="gameObject">Game Object to deactivate.</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Game Object can not be found. Default is 'true'.</param>
        void DeactivateObject(GameObject gameObject, bool strict = true);
        /// <summary>
        /// Checks if the pool contains the object.
        /// </summary>
        /// <param name="gameObject">Game Object to check.</param>
        /// <returns>Returns true if the pool contains the object. Returns false if the pool doesn't contain the object.</returns>
		bool ContainsObjectCheck(GameObject gameObject);
        /// <summary>
        /// Use this to destroy an object, this will keep the pool accurate.
        /// </summary>
        /// <param name="gameObject">Game Object to destroy.</param>
		void DestroyObject(GameObject gameObject);
		/// <summary>
		/// Add an object created outside of this pool. Use with caution! This can cause issues, ex: If the  object is added and then destroyed, 
		/// or destroyed first then added, then the pool will be out of sync.
		/// 
		/// Also, this method will injected the GameObject's ObjectPool into any attached MonoBehavior class that implements IObjectForPool.
		/// </summary>
		/// <param name="gameObject">Game Object to add.</param>
		/// <param name="priority">If set to true any other inactive or active game objects will be removed to make room.</param>
		/// <param name="destroy">If set to true any other inactive or active game objects will be destroyed to make room.</param>
		/// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
		void AddObject(GameObject gameObject, bool priority = true, bool destroy = true, bool removeActive = true);
        /// <summary>
        /// Remove an object.
        /// </summary>
        /// <param name="gameObject">Object to remove.</param>
		void RemoveObject(GameObject gameObject);
        /// <summary>
        /// Iterate through the pool and releases as many objects as possible until the pool size is back to the initial default size.
        /// </summary>
        /// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
        /// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
		void Shrink(bool removeActive = true, bool destroy = true);
        /// <summary>
        /// This is similar to 'Shrink', it shrinks the pool to the passed size argument.
        /// </summary>
        /// <param name="size">The size to shrink the pool to.</param>
        /// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
        /// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
        void ShrinkToSize(uint size, bool removeActive = true, bool destroy = true);
        /// <summary>
        /// Iterates through the pool and Destroys all GameObjects. Use 'DestroyPool' if you need to dispose of the object pool.
        /// </summary>
        void DestroyObjects();
        /// <summary>
        /// Removes all objects, or destroys al objects. Also removes AutoShrink.
        /// </summary>
        /// <param name="destroyObjects">'True' = destroy objects. Default is 'true'.</param>
        void DestroyPool(bool destroyObjects = true);
        /// <summary>
        /// Remove all objects from the pool without destroying them.
        /// </summary>
		void RemoveObjects();
        /// <summary>
        /// Initialize pool. This is useful when a pool has been destroyed and you want to use the pool again.
		/// 
		/// Also, this method will injected the GameObject's ObjectPool into any attached MonoBehavior class that implements IObjectForPool.
        /// </summary>
        /// <param name="destroy">Destroy any objects. The Default is 'true'</param>
		void InitializePool(bool destroy = true);
        /// <summary>
        /// Use this method if Active and Inactive List seem to be out of sync.
        /// </summary>
		void SortPool();

        /// <summary>
        /// Check if this ObjectPool has an AutoShrink attached.
        /// </summary>
        /// <returns>Returns true if this Object Pool has an AutoShrink object attached, false if it does not.</returns>
        bool AutoShrinkCheck();
		/// <summary>
		/// Returns AutoShrink. Use careful, another object may be making changes and managing it. 
		/// You can use 'AutoShrinkCheck' if you only need to verify that an AutoShrink is attached.
		/// </summary>
		/// <returns>Returns the AutoShrink object, if there is one attached.</returns>
		AutoShrink GetAutoShrink();
        /// /// <summary>
        /// Attaches an AutoShrink component to this Object Pool. An Object Pool can have only one AutoShrink, and vice versa. 
        /// This method throws an error if an AutoShrink is already attached. Use 'AutoShrinkCheck' to check if the Object Pool already has an AutoShrink Attached.
        /// Use 'AutoShrinkRemove' to remove an AutoShrink.
        /// This also starts the AutoShrink. Use 'GetAutoShrink' to have more control of the AutoShrink's properties.
        /// </summary>
        /// <param name="gameObjectForAddingComponent">The GameObject to attach the AutoShrink Component to.</param>
        /// <param name="autoShrinkSampleFrequencySeconds">The sample frequency in seconds, the delay between samples of the Object Pool's Active Objects.</param>
        /// <param name="autoShrinkFreguencySeconds">Frequency in seconds for finding an average and shrinking the Object Pool to that average.</param>
        /// <param name="autoShrinkBuffer">How inactive objects to leave.</param>
        void AutoShrinkAttach(GameObject gameObjectForAddingComponent, uint autoShrinkSampleFrequencySeconds = 5, 
            uint autoShrinkFreguencySeconds = 60, uint autoShrinkBuffer = 0);
        /// <summary>
        /// Removes and destroys the AutoShrink, if there is one attached.
        /// </summary>
        void AutoShrinkRemove();
        /// <summary>
        /// This should only be called from the attached AutoShrink via its OnDestroy event.
        /// </summary>
        /// /// <param name="autoShrink">The attached AutoShrink that was destroyed.</param>
        void AutoShrinkOnDestroy(AutoShrink autoShrink);
	}
}