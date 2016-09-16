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

namespace Dorkbots.ObjectPooling
{
	public class ListOfObjectPools 
	{
		/// <summary>
        /// Creates a list of ObjectPools from an array of GameObject prefabs. This method can lead to the creation of Object Pools if any Object Pools do not already exist. 
        /// Use 'ObjectPools.GetPools' to grab an Object Pool and change its properties directly if needed.
		/// </summary>
		/// <param name="gameObjects">An array of GameObjects.</param>
        /// <param name="gameObjectForAddingComponent">For AutoShrink. The GameObject to attach the AutoShrink Component to. 
		/// Warning: If an AutoShrink is already attached to the Object Pool this will be ignored. Use 'AutoShrinksUpdate' to update all attached AutoShrinks.
		/// If needed use 'AutoShrinksUpdate' to remove attached AutoShrinks from an array of ObjectPools</param>
		/// <param name="autoShrinkSampleFrequencySeconds">For AutoShrink. The sample frequency in seconds, the delay between samples of the Object Pool's Active Objects.</param>
		/// <param name="autoShrinkFreguencySeconds">For AutoShrink. Frequency in seconds for finding an average and shrinking the Object Pool to that average.</param>
		/// <param name="autoShrinkBuffer">For AutoShrink. How inactive objects to leave.</param>
		/// <returns>Returns a list of ObjectPools, the list contains an ObjectPool for each unique GameObject Prefab.</returns>
		public static List<IObjectPool> GetList(GameObject[] gameObjects, GameObject gameObjectForAddingComponent = null, uint autoShrinkSampleFrequencySeconds = 5, 
			uint autoShrinkFreguencySeconds = 60, uint autoShrinkBuffer = 0)
		{
			IObjectPools objectPools = ObjectPoolingManager.Instance.objectPools;
			List<IObjectPool> objectPoolsList = new List<IObjectPool>();
			IObjectPool objectPool;
			GameObject gameObject;
            bool poolCreated;
			for (int i = 0; i < gameObjects.Length; i++) 
			{
				gameObject = gameObjects [i];
				objectPool = objectPools.CreateOrGetPoolConfirm ( gameObject, out poolCreated );

				// Auto Shrink
				if (gameObjectForAddingComponent != null && !objectPool.AutoShrinkCheck())
				{
					objectPool.AutoShrinkAttach(gameObjectForAddingComponent, autoShrinkSampleFrequencySeconds, autoShrinkFreguencySeconds, autoShrinkBuffer);
				}

                if (!poolCreated) 
				{
					//Object Pool was not created so an Object Pool for this prefab could have been created by another script, see if it's been added to this object pool list first.
					if(!objectPoolsList.Contains(objectPool))
					{
						// it has not been added to this object's pool list so add it.
						objectPoolsList.Add(objectPool);
					}
				} 
				else 
				{
					objectPoolsList.Add(objectPool);
				}
			}

			return objectPoolsList;
		}

		/// <summary>
        /// Removes all Object Pools inside of an array. Use this to dispose of all ObjectPools in the array. Stops attached AutoShrinks.
		/// </summary>
		/// <param name="objectPools">An array of ObjectPools to remove.</param>
		/// <param name="destroy">Destroy any objects. The Default is 'true'</param>
		/// <param name="strict">When set to 'true' an error will be logged if the Object Pool can not be found. Defaul is false.</param>
		public static void RemovePools(IObjectPool[] objectPools, bool destroy = true, bool strict = false)
		{
			IObjectPools allObjectPools = ObjectPoolingManager.Instance.objectPools;
			for (int i = 0; i < objectPools.Length; i++) 
			{
				allObjectPools.RemovePool (objectPools [i].objectName, destroy, strict);
			}
		}

		/// <summary>
		/// Returns a random GameObject from an array of Object Pools.
		/// </summary>
		/// <param name="objectPools">An array of IObjectPools.</param>
		/// <returns>Returns a GameObject.</returns>
		public static GameObject GetRandomObjectFromArray(IObjectPool[] objectPools)
		{
			int ranPos = Random.Range (0, objectPools.Length);
			IObjectPool ranObjectPool = objectPools[ranPos];
			return ranObjectPool.GetObject ();
		}

		/// <summary>
		/// Returns random Object Pool from an array of Object Pools
		/// </summary>
		/// <param name="objectPools">An array of IObjectPools.</param>
		/// <returns>Returns an IObjectPool.</returns>
		public static IObjectPool GetRandomPoolFromArray(IObjectPool[] objectPools)
		{
			int ranPos = Random.Range (0, objectPools.Length);
			return objectPools[ranPos];
		}

		/// <summary>
		/// Updates all AutoShrinks attached to ObjectPools in an array. Or attaches AutoShrinks to ObjectPools in an array.     
		/// </summary>
		/// <param name="objectPools">An array of IObjectPools.</param>
		/// <param name="gameObjectForAddingComponent">For AutoShrink. The GameObject to attach the AutoShrink Component to.</param>
		/// <param name="removeAutoShrink">If set to 'true', removes a previously attached AutoShrinks. If set to 'false' the AutoShrink's properties will be reset, 
		/// but it will not be started or stopped. Use 'AutoShrinksStart' or 'AutoShrinksStop' if needed.</param>
		/// <param name="autoShrinkSampleFrequencySeconds">The sample frequency in seconds, the delay between samples of the Object Pool's Active Objects.</param>
		/// <param name="autoShrinkFreguencySeconds">For AutoShrink. Frequency in seconds for finding an average and shrinking the Object Pool to that average.</param>
		/// <param name="autoShrinkBuffer">For AutoShrink. How inactive objects to leave.</param>
		public static void AutoShrinksUpdate(IObjectPool[] objectPools, GameObject gameObjectForAddingComponent, bool removeAutoShrink = true, uint autoShrinkSampleFrequencySeconds = 5, 
			uint autoShrinkFreguencySeconds = 60, uint autoShrinkBuffer = 0)
		{
			IObjectPool objectPool;
			AutoShrink autoShrink;
			for (int i = 0; i < objectPools.Length; i++) 
			{
				objectPool = objectPools[i];
				if (objectPool.AutoShrinkCheck())
				{
					if (removeAutoShrink)
					{
						objectPool.AutoShrinkRemove();
						objectPool.AutoShrinkAttach(gameObjectForAddingComponent, autoShrinkSampleFrequencySeconds, autoShrinkFreguencySeconds, autoShrinkBuffer);
					}
					else
					{
						autoShrink = objectPool.GetAutoShrink();
						autoShrink.sampleFrequencySeconds = autoShrinkSampleFrequencySeconds;
						autoShrink.autoShrinkFreguencySeconds = autoShrinkFreguencySeconds;
						autoShrink.buffer = autoShrinkBuffer;
					}
				}
				else
				{
					objectPool.AutoShrinkAttach(gameObjectForAddingComponent, autoShrinkSampleFrequencySeconds, autoShrinkFreguencySeconds, autoShrinkBuffer);
				}
			}
		}

		/// <summary>
		/// Starts all AutoShrinks attached to ObjectPools in an array.   
		/// </summary>
		/// <param name="objectPools">An array of IObjectPools.</param>
		public static void AutoShrinksStart(IObjectPool[] objectPools)
		{
			IObjectPool objectPool;
			AutoShrink autoShrink;
			for (int i = 0; i < objectPools.Length; i++) 
			{
				objectPool = objectPools[i];
				if (objectPool.AutoShrinkCheck())
				{
					autoShrink = objectPool.GetAutoShrink();
					autoShrink.StartAutoShrink();
				}
			}
		}

		/// <summary>
		/// Stops all AutoShrinks attached to ObjectPools in an array.   
		/// </summary>
		/// <param name="objectPools">An array of IObjectPools.</param>
		public static void AutoShrinksStop(IObjectPool[] objectPools)
		{
			IObjectPool objectPool;
			AutoShrink autoShrink;
			for (int i = 0; i < objectPools.Length; i++) 
			{
				objectPool = objectPools[i];
				if (objectPool.AutoShrinkCheck())
				{
					autoShrink = objectPool.GetAutoShrink();
					autoShrink.StopAutoShrink();
				}
			}
		}
	}
}