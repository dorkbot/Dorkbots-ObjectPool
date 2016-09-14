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