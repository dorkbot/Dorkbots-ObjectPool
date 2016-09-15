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
using Dorkbots.ObjectPooling;
using System;

namespace Dorkbots.ObjectPooling
{
    /// <summary>
    /// Finds the average for active objects in an Object Pool, then shrinks the Object Pool to that average size. 
    /// Prevents an Object Pool from filling up with inactive objects that consume memory.
    /// </summary>
    public class AutoShrink : MonoBehaviour 
    {
        private uint _sampleFrequencySeconds;
        private uint _autoShrinkFreguencySeconds;
        private uint _buffer;
        private List<int> activeSampleArray; 
        private IObjectPool _objectPool;

        void OnDestroy()
        {
            if (_objectPool != null)
            {
                StopAutoShrink();
                _objectPool.AutoShrinkOnDestroy(this);
                _objectPool = null;
            }
        }

        // GETTER/SETTERS
        public IObjectPool objectPool
        {
            get{return _objectPool;}
        }

        /// <summary>
        /// The sample frequency in seconds, the delay between samples of the Object Pool's Active Objects. Change takes affect on next scheduled sample.
        /// Use 'StartAutoShrink' to apply change immediately.
        /// </summary>
        public uint sampleFrequencySeconds
        {
            get{return _sampleFrequencySeconds;}
            set{_sampleFrequencySeconds = value;}
        }

        /// <summary>
        /// Frequency in seconds for finding an average and shrinking the Object Pool to that average. Change takes affect on next scheduled Auto Shrink.
        /// Use 'StartAutoShrink' to apply change immediately.
        /// </summary>
        public uint autoShrinkFreguencySeconds
        {
            get{return _autoShrinkFreguencySeconds;}
            set{_autoShrinkFreguencySeconds = value;}
        }

        /// <summary>
        /// How many extra inactive objects to leave.
        /// </summary>
        public uint buffer
        {
            get{return _buffer;}
            set{_buffer = value;}
        }

        /// <summary>
        /// Starts Auto Shrink. An Object Pool should only have one AutoShrink object encapsulating it.
        /// </summary>
        /// <param name="objectPool">Object Pool to be auto shrunk.</param>
        /// <param name="sampleFrequencySeconds">The sample frequency in seconds, the delay between samples of the Object Pool's Active Objects.</param>
        /// <param name="autoShrinkFreguencySeconds">Frequency in seconds for finding an average and shrinking the Object Pool to that average.</param>
        /// <param name="buffer">How many extra inactive objects to leave.</param>
        public void InitAutoShrink(IObjectPool objectPool, uint sampleFrequencySeconds = 5, uint autoShrinkFreguencySeconds = 60, uint buffer = 0)
        {
            if (objectPool.GetAutoShrink() != this)
            {
                Debug.LogError("This AutoShrink is not attached to the ObjectPool. Be sure to set up the AutoShrink via 'ObjectPool.AutoShrinkAttach'!");
            }
            else
            {
                activeSampleArray = new List<int>();

                StopAutoShrink();

                this._objectPool = objectPool;
                this._sampleFrequencySeconds = sampleFrequencySeconds;
                this._autoShrinkFreguencySeconds = autoShrinkFreguencySeconds;
                this._buffer = buffer;

                StartAutoShrink();
            }
        }

        /// <summary>
        /// Starts Auto Shrink.
        /// </summary>
        public void StartAutoShrink()
        {
            if (_objectPool != null)
            {
                StopAutoShrink();

                StartCoroutine("RandomStartDelay");
            }
        }

        /// <summary>
        /// Stop Auto Shrinking.
        /// </summary>
        public void StopAutoShrink()
        {
            if (_objectPool != null)
            {
				StopCoroutine("RandomStartDelay");

                StopCoroutine("SampleActiveCount");

                StopCoroutine("PerformAutoShrink");

                activeSampleArray.Clear();
            }
        }

        /// <summary>
        /// Destroys and removes the AutoShrink. Destroy the this AutoShrink via 'ObjectPool.AutoShrinkRemove'!
        /// </summary>
        /// <param name="objectPool">The attached ObjectPool. Only the attached ObjectPool should make this call.</param>
        public void Remove(IObjectPool objectPool)
        {
            if (objectPool.GetAutoShrink() != this)
            {
                Debug.LogError("This AutoShrink is not attached to the ObjectPool. Remove this AutoShrink via 'ObjectPool.AutoShrinkRemove'!");
            }
            else
            {
                StopAutoShrink();
                _objectPool = null;
                Destroy(this);
            }
        }

		// We don't want all AutoShrink Objects to perform AutoShrink at the same time even if their frequencies match.
		private IEnumerator RandomStartDelay()
		{
			yield return new WaitForSeconds (UnityEngine.Random.Range (0, 10));

            StartCoroutine("SampleActiveCount");

			StartCoroutine("PerformAutoShrink");
		}

        private IEnumerator SampleActiveCount()
        {
            yield return new WaitForSeconds(_sampleFrequencySeconds);

            activeSampleArray.Add((int)_objectPool.activeCount);

            StartCoroutine("SampleActiveCount");
        }

        private IEnumerator PerformAutoShrink()
        {
			yield return new WaitForSeconds(_autoShrinkFreguencySeconds);

			// no need to perform if the object pool has no inactive objects.
			if (_objectPool.inactiveCount > 0) 
			{
				int total = 0;

				foreach(int num in activeSampleArray)
				{
					//Debug.Log("num = " + num);
					total += num;
				}

				float average = total / activeSampleArray.Count;
				//Debug.Log("average =  " + average);

				// adding buffer, just in case, keep inactive, as a buffer.
				double roundAverage = Math.Round(average, 0, MidpointRounding.AwayFromZero) + _buffer;
				//Debug.Log("roundAverage = " + roundAverage);

				_objectPool.ShrinkToSize((uint)roundAverage, false);
			}
            
            activeSampleArray.Clear();

            StartCoroutine("PerformAutoShrink");
        }
    }
}