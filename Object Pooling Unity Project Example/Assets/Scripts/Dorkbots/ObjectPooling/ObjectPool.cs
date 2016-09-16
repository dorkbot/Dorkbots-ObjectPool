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
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

namespace Dorkbots.ObjectPooling
{
	/// <summary>
	/// Creates a reausable list of objects. Use this pool to activate and to deactivate objects that are in the list.
	/// </summary>
	public class ObjectPool : IObjectPool
	{
		//the list of active objects.
		private List<GameObject> pooledObjectsAll;

		//the list of inactive objects.
		private List<GameObject> pooledObjectsInactive;

		//the list of active objects.
		private List<GameObject> pooledObjectsActive;

		//Prefab to pool
		private readonly GameObject pooledObj;

		//maximum number of objects to have in the list.
		private uint _maxPoolSize;

		//initial and default number of objects to have in the list.
		private uint _initialPoolSize;

		// prevent objects from being destroyed when a new scene loads.
		private readonly bool destroyOnLoad;

        private AutoShrink autoShrink = null;

		/// <summary>
		/// Constructor for creating a new Object Pool.
		/// </summary>
		/// <param name="obj">Game Object for this pool</param>
		/// <param name="initialPoolSize">Initial and default size of the pool.</param>
		/// <param name="maxPoolSize">Maximum number of objects this pool can contain. '0' is the default and sets the Poll to grow when needed.</param>
		/// <param name="destroyOnLoad">Set GameObjects to not be destroyed when a new scene is loaded. Default is set to 'true'</param>
		public ObjectPool(GameObject obj, uint initialPoolSize, uint maxPoolSize = 0, bool destroyOnLoad = true)
		{
			pooledObjectsAll = new List<GameObject>();
			pooledObjectsInactive = new List<GameObject>();
			pooledObjectsActive = new List<GameObject>();

			this._maxPoolSize = maxPoolSize;
			this.pooledObj = obj;

			if (initialPoolSize > maxPoolSize && maxPoolSize != 0) 
			{
				this._initialPoolSize = maxPoolSize;
			}
			else
			{
				this._initialPoolSize = initialPoolSize;
			}

			this.destroyOnLoad = destroyOnLoad;

			if (this._maxPoolSize != 0 && this._maxPoolSize < this._initialPoolSize)
			{
				this._maxPoolSize = this._initialPoolSize;
			}

			InitializePool();
		}

		// GETTERS/SETTERS
		public String objectName
		{
			get{return pooledObj.name;}
		}

		public uint maxPoolSize
		{
			get{return _maxPoolSize;}
			set
			{
				_maxPoolSize = value;
				if (pooledObjectsAll.Count > _maxPoolSize && _maxPoolSize != 0) PShrink(_maxPoolSize);
				if (_initialPoolSize > _maxPoolSize && _maxPoolSize != 0) _initialPoolSize = _maxPoolSize;
			}
		}

		//initial and default number of objects to have in the list.
		public uint initialPoolSize
		{
			get{return _initialPoolSize;}
			set
			{
				_initialPoolSize = value;
				if (_initialPoolSize > _maxPoolSize && _maxPoolSize != 0) _maxPoolSize = _initialPoolSize;
			}
		}

		public uint count
		{
			get{return (uint)pooledObjectsAll.Count;}
		}

		public uint activeCount
		{
			get{return (uint)pooledObjectsActive.Count;}
		}

		public uint inactiveCount
		{
			get{return (uint)pooledObjectsInactive.Count;}
		}

		/// <summary>
        /// Returns an active object from the object pool without resetting any of its values. 
        /// Use 'ActivateObject', 'DeactivateObject' and 'DestroyObject' to insure Object Pool accuracy and effectiveness.
		/// 
		/// Also, this method will injected the GameObject's ObjectPool into any attached MonoBehavior class that implements IObjectForPool.
		/// </summary>
		/// <param name="activate">Set whether the game object is set to active.</param>
		/// <returns>Game Object of requested type if it is available, otherwise null.</returns>
		public GameObject GetObject(bool activate = true)
		{
			GameObject gameObject = null;
			// first check inactive list
			if (pooledObjectsInactive.Count > 0)
			{
                gameObject = pooledObjectsInactive[0];
			}
				
			//if we make it this far, we obviously didn't find an inactive object so we need to see if we can grow beyond our current count.
			if (gameObject == null && (this._maxPoolSize == 0 || this._maxPoolSize > pooledObjectsAll.Count))
			{
                //Instantiate a new object.
				gameObject = GameObject.Instantiate(pooledObj, Vector3.zero, Quaternion.identity) as GameObject;
				InjectObjectPool(gameObject);
                pooledObjectsAll.Add(gameObject);
			}

			if (gameObject != null) 
			{
               if (activate) 
				{
					ActivateObject (gameObject);
				} 
				else 
				{
					DeactivateObject (gameObject);
				}
			}

			return gameObject;
		}

		/// <summary>
		/// Use this to activate an object, this will make sure that the object is sorted properly.
		/// </summary>
		/// <param name="gameObject">Game Object to activate.</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Game Object can not be found. Default is 'true'.</param>
        public void ActivateObject(GameObject gameObject, bool strict = true)
		{
            if (pooledObjectsAll.IndexOf (gameObject) < 0) 
			{
                if (strict) Debug.LogError ("Object is currently NOT in the pool. Use 'AddObject' to add the object to the pool.");
			} 
			else 
			{
				gameObject.SetActive(true);
				pooledObjectsActive.Add(gameObject);
				pooledObjectsInactive.Remove(gameObject);
			}
		}

		/// <summary>
		/// Use this to deactivate an object, this will make sure that the object is sorted properly.
		/// </summary>
		/// <param name="gameObject">Game Object to deactivate.</param>
        /// <param name="strict">When set to 'true' an error will be logged if the Game Object can not be found. Default is 'true'.</param>
        public void DeactivateObject(GameObject gameObject, bool strict = true)
		{
			if (pooledObjectsAll.IndexOf (gameObject) < 0) 
			{
                if (strict) Debug.LogError("Object is currently NOT in the pool. Use 'AddObject' to add the object to the pool before calling 'DeactiveObject'.");
			}
			else 
			{
				gameObject.SetActive(false);
				pooledObjectsActive.Remove(gameObject);
				pooledObjectsInactive.Add(gameObject);
			}
		}

		/// <summary>
		/// Checks if the pool contains the object.
		/// </summary>
		/// <param name="gameObject">Game Object to check.</param>
		/// <returns>Returns true if the pool contains the object. Returns false if the pool doesn't contain the object.</returns>
		public bool ContainsObjectCheck(GameObject gameObject)
		{
			if (pooledObjectsAll.IndexOf (gameObject) < 0) 
			{
				return false;
			}
			else 
			{
				return true;
			}
		}

		/// <summary>
		/// Use this to destroy an object, this will keep the pool accurate.
		/// </summary>
		/// <param name="gameObject">Game Object to destroy.</param>
		public void DestroyObject(GameObject gameObject)
		{
			if (pooledObjectsAll.IndexOf (gameObject) < 0) 
			{
				Debug.LogError("Object is currently NOT in the pool. Use 'AddObject' to add the object to the pool before calling 'DestroyObject'.");
			}
			else 
			{
				pooledObjectsAll.Remove(gameObject);
				pooledObjectsActive.Remove(gameObject);
				pooledObjectsInactive.Remove(gameObject);

				GameObject.Destroy (gameObject);
			}
		}
        
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
		public void AddObject(GameObject gameObject, bool priority = true, bool destroy = true, bool removeActive = true)
		{
			// check if object is already in the pool
			if (pooledObjectsAll.IndexOf(gameObject) < 0)
			{
				int pooledObjectsAllCount = pooledObjectsAll.Count;
				if (this._maxPoolSize > 0 && pooledObjectsAllCount == this._maxPoolSize && priority)
				{
					PShrink((uint)pooledObjectsAllCount - 1, removeActive, destroy);
				}

				if(this._maxPoolSize == 0 || pooledObjectsAll.Count < this._maxPoolSize)
				{
					pooledObjectsAll.Add(gameObject);
					InjectObjectPool(gameObject);
					if (gameObject.activeSelf)
					{
						pooledObjectsActive.Add(gameObject);
					}
					else
					{
						pooledObjectsInactive.Add(gameObject);
					}
				}
			}
		}

		/// <summary>
		/// Remove an object.
		/// </summary>
		/// <param name="gameObject">Object to remove.</param>
		public void RemoveObject(GameObject gameObject)
		{
			pooledObjectsAll.Remove(gameObject);
			pooledObjectsInactive.Remove(gameObject);
			pooledObjectsActive.Remove(gameObject);
		}

		/// <summary>
		/// Iterate through the pool and releases as many objects as possible until the pool size is back to the initial default size.
		/// </summary>
		/// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
		/// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
		public void Shrink(bool removeActive = true, bool destroy = true)
		{
			PShrink(_initialPoolSize, removeActive, destroy);
		}

        /// <summary>
        /// This is similar to 'Shrink', it shrinks the pool to the passed size argument.
        /// </summary>
        /// <param name="size">The size to shrink the pool to.</param>
        /// <param name="removeActive">After inactive objects have been removed, then remove active objects. Default is set to 'true'</param>
        /// <param name="destroy">Destroy GameObjects that are removed from the pool. The Default is 'true'</param>
        public void ShrinkToSize(uint size, bool removeActive = true, bool destroy = true)
        {
            PShrink(size, removeActive, destroy);
        }

		private void PShrink(uint size, bool removeActive = true, bool destroy = true)
		{
            //how many objects are we trying to remove here?
			int objectsToRemoveCount = pooledObjectsAll.Count - (int)size;
			//check if any objects need to be removed
			if (objectsToRemoveCount > 0)
			{
				//iterate through our lists and remove objects
				GameObject gameObject;
				int i = 0;

				if (pooledObjectsInactive.Count > 0)
				{
					// first remove inactive objects
					for (i = 0; i < objectsToRemoveCount; i++)
					{
                        gameObject = pooledObjectsInactive[0];
						//kill it from the list.
						pooledObjectsAll.Remove(gameObject);
						pooledObjectsInactive.Remove(gameObject);
                        pooledObjectsActive.Remove(gameObject);
						// destroy it
                        if (destroy) GameObject.Destroy(gameObject);
						// check if list is empty
                        if (pooledObjectsInactive.Count == 0) break;
					}
				}
               
				// deduct any inactive objects removed
				objectsToRemoveCount -= i;
				// only continue if 'removeActive is true
				if (objectsToRemoveCount > 0 && removeActive)
				{
					if (pooledObjectsActive.Count > 0)
					{
						// then remove active objects
						for (i = 0; i < objectsToRemoveCount; i++)
						{
                            gameObject = pooledObjectsActive[0];
							//kill it from the list.
                            pooledObjectsAll.Remove(gameObject);
                            pooledObjectsInactive.Remove(gameObject);
                            pooledObjectsActive.Remove(gameObject);
							// destroy it
							if (destroy) GameObject.Destroy(gameObject);
							// check if list is empty
							if (pooledObjectsActive.Count == 0) break;
						}
					}

					// deduct any active objects removed
					objectsToRemoveCount -= i;
					if (objectsToRemoveCount > 0)
					{
						// if at this point objectsToRemoveCount > 0 then something is wrong, the lists are not synced.
						if (pooledObjectsAll.Count > 0)
						{
							for (i = 0; i < objectsToRemoveCount; i++)
							{
                                gameObject = pooledObjectsAll[0];
								//kill it from the list.
								pooledObjectsAll.Remove(gameObject);
                                pooledObjectsInactive.Remove(gameObject);
                                pooledObjectsActive.Remove(gameObject);
								// destroy it
								if (destroy) GameObject.Destroy(gameObject);
								// check if list is empty
								if (pooledObjectsAll.Count == 0) break;
							}

							// make sure lists are synced
							if (pooledObjectsAll.Count != pooledObjectsActive.Count + pooledObjectsInactive.Count) SortPool();
						}
					}
				}
			}
		}

		/// <summary>
		/// Iterates through the pool and Destroys all GameObjects. Use 'DestroyPool' if you need to dispose of the object pool.
		/// </summary>
        public void DestroyObjects()
		{
			int pooledObjectsCount = pooledObjectsAll.Count;
			// pooledObjectsCount hasn't changed so use it
			for (int i = 0; i < pooledObjectsCount; i++)
			{
				GameObject.Destroy(pooledObjectsAll[i]);
			}

			RemoveObjects ();
		}

        /// <summary>
        /// Removes all objects, or destroys al objects. Also removes AutoShrink.
        /// </summary>
        /// <param name="destroyObjects">'True' = destroy objects. Default is 'true'.</param>
        public void DestroyPool(bool destroyObjects = true)
        {
            if (destroyObjects)
            {
                DestroyObjects();
            }
            else
            {
                RemoveObjects ();
            }

            AutoShrinkRemove();
        }

		/// <summary>
		/// Remove all objects from the pool without destroying them.
		/// </summary>
		public void RemoveObjects()
		{
			pooledObjectsAll.Clear();
			pooledObjectsInactive.Clear();
			pooledObjectsActive.Clear();
		}

		/// <summary>
		/// Initialize pool. This is useful when a pool has been destroyed and you want to use the pool again.
		/// 
		/// Also, this method will injected the GameObject's ObjectPool into any attached MonoBehavior class that implements IObjectForPool.
		/// </summary>
		/// <param name="destroy">Destroy any objects. The Default is 'true'</param>
		public void InitializePool(bool destroy = true)
		{
			if (destroy)
			{
				DestroyObjects();
			}
			else
			{
				RemoveObjects ();
			}

			//create and add objects based on initial size.
			GameObject nObj;
			for (int i = 0; i < _initialPoolSize; i++)
			{
				//instantiate and create a game object with useless attributes. Encapsulating class should set as needed.
				nObj = GameObject.Instantiate(pooledObj, Vector3.zero, Quaternion.identity) as GameObject;

				InjectObjectPool(nObj);

				//make sure the object isn't active.
				nObj.SetActive(false);

				//add the object too our list.
				pooledObjectsAll.Add(nObj);
				pooledObjectsInactive.Add(nObj);

				if (!destroyOnLoad)
				{
					//Don't destroy on load, so we can manage centrally.
					GameObject.DontDestroyOnLoad(nObj);
				}
			}
		}

		/// <summary>
		/// Use this method if Active and Inactive List seem to be out of sync.
		/// </summary>
		public void SortPool()
		{
            pooledObjectsInactive.Clear();
			pooledObjectsActive.Clear();
            List<GameObject> tempObjectsList = new List<GameObject>(pooledObjectsAll);

			GameObject gameObj;
			int pooledObjectsAllCount = pooledObjectsAll.Count;
			for(int i = 0; i < pooledObjectsAllCount; i++)
			{
				gameObj = pooledObjectsAll[i];
                if (gameObj != null)
                {
                    if (gameObj.activeSelf)
                    {
                        pooledObjectsActive.Add(gameObj);
                    }
                    else
                    {
                        pooledObjectsInactive.Add(gameObj);
                    }
                }
                else
                {
                    tempObjectsList.Remove(gameObj);
                }
			}

            pooledObjectsAll = tempObjectsList;
		}

        /// <summary>
		/// Check if this ObjectPool has an AutoShrink attached. Returns true if this Object Pool has an AutoShrink object attached, false if it does not.
        /// </summary>
        /// <returns>Returns true if this Object Pool has an AutoShrink object attached, false if it does not.</returns>
        public bool AutoShrinkCheck()
        {
            return (autoShrink != null);
        }

		/// <summary>
		/// Returns AutoShrink. Use careful, another object may be making changes and managing it. 
		/// You can use 'AutoShrinkCheck' if you only need to verify that an AutoShrink is attached.
		/// </summary>
		/// <returns>Returns the AutoShrink object, if there is one attached.</returns>
		public AutoShrink GetAutoShrink()
		{
			return autoShrink;
		}

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
        public void AutoShrinkAttach(GameObject gameObjectForAddingComponent, uint autoShrinkSampleFrequencySeconds = 5, 
            uint autoShrinkFreguencySeconds = 60, uint autoShrinkBuffer = 0)
        {
            if (this.autoShrink != null) 
            {
                Debug.LogError("This Object Pool already has an AutoShrink object attached. Use 'AutoShrinkRemove' to first remove the attached AutoShrink object.");
            }
            else
            {
                this.autoShrink = gameObjectForAddingComponent.AddComponent<AutoShrink>();
                this.autoShrink.InitAutoShrink (this, autoShrinkSampleFrequencySeconds, autoShrinkFreguencySeconds, autoShrinkBuffer);
            }
        }

        /// <summary>
        /// Removes and destroys the AutoShrink, if there is one attached.
        /// </summary>
        public void AutoShrinkRemove()
        {
            if (this.autoShrink != null) 
            {
                this.autoShrink.Remove(this);
                this.autoShrink = null;
            }
        }

        /// <summary>
        /// This should only be called from the attached AutoShrink via its OnDestroy event.
        /// </summary>
        /// <param name="autoShrink">The attached AutoShrink that was destroyed.</param>
        public void AutoShrinkOnDestroy(AutoShrink autoShrink)
        {
            if (this.autoShrink != null && this.autoShrink == autoShrink) 
            {
                this.autoShrink = null;
            }
        }

		private void InjectObjectPool(GameObject gameObject)
		{
			IObjectForPool objectForPool;
			MonoBehaviour monoBehaviour;
			MonoBehaviour[] monoList;

			monoList = gameObject.GetComponents<MonoBehaviour>();

			for (int i = 0; i < monoList.Length; i++) 
			{
				monoBehaviour = monoList[i];
				if (monoBehaviour is IObjectForPool)
				{
					objectForPool = (IObjectForPool)monoBehaviour;
					objectForPool.objectPool = this;
				}
			}
		}
	}
}