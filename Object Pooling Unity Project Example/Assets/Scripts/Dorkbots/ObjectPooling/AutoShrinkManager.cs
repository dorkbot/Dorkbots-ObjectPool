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
    public class AutoShrinkManager : MonoBehaviour 
    {
        /*
     * a list of auto shrinks
     * 
     * add auto shrink
     * 
     * remove auto shrink
     *  when list is empty remove/destroy this game object
     * 
     * 
     * update cycles through list of auto shrinks
     * 
     * 
     * 
     * 
     * 
     */
        // Use this for initialization

        private static AutoShrinkManager instance;
        private static GameObject containerGameObject;

        // Instance
        private List<AutoShrink> autoShrinks;

        public static void AddAutoShrink(AutoShrink autoShrink)
        {
            if (instance == null)
            {
                containerGameObject = new GameObject();
                instance = containerGameObject.AddComponent<AutoShrinkManager>();
                instance.autoShrinks.Add(autoShrink);
            }
            else
            {
                if (instance.autoShrinks.IndexOf(autoShrink) < 0)
                {
                    instance.autoShrinks.Add(autoShrink);
                }
            }
        }

        public static void RemoveAutoShrink(AutoShrink autoShrink)
        {
            if (instance.autoShrinks.IndexOf(autoShrink) >= 0)
            {
                instance.autoShrinks.Remove(autoShrink);

                if (instance.autoShrinks.Count <= 0)
                {
                    Destroy(containerGameObject);
                    instance = null;
                }
            }
        }

        private static void InstanceDestroyed()
        {
            instance = null;
            containerGameObject = null;
        }

        // Instance
        void Start () 
        {
            autoShrinks = new List<AutoShrink>();
        }

        // Update is called once per frame
        void Update () 
        {
            for (int i = 0; i < autoShrinks.Count; i++)
            {

            }
        }

        void OnDestroy()
        {
            autoShrinks.Clear();
            AutoShrinkManager.InstanceDestroyed();
        }
    }
}