using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

using Dorkbots.ObjectPooling;
using System.Collections.Generic;
using Dorkbots.UserInput.ClickableGameObjects;

public class TestObjectPoolingMain : MonoBehaviour 
{
	[SerializeField]
    private GameObject squarePrefab;

    [SerializeField]
    private InputField intxt_ChangeMaxSize;

    [SerializeField]
    private InputField intxt_ChangeInitSize;

    [SerializeField]
    private Text txt_ActiveObjects;

    [SerializeField]
    private Text txt_InactiveObjects;

    [SerializeField]
    private Text txt_TotalObjects;

    [SerializeField]
    private Toggle tgl_AutoShrink;

	private IObjectPools objectPools;
	private IObjectPool squareObjectPool;

    private uint maxSize = 10;
    private uint initSize = 5;

    private AutoShrink autoShrink;

	// Use this for initialization
	void Start () 
	{
        intxt_ChangeMaxSize.text = maxSize.ToString();
        intxt_ChangeInitSize.text = initSize.ToString();

		objectPools = ObjectPoolingManager.Instance.objectPools;
        squareObjectPool = objectPools.CreateOrGetPool (squarePrefab, initSize, maxSize);

        // add objects from the scene to the object pool
        AddObjects();

        intxt_ChangeMaxSize.text = squareObjectPool.maxPoolSize.ToString();
        intxt_ChangeInitSize.text = squareObjectPool.initialPoolSize.ToString();

        squareObjectPool.AutoShrinkAttach(this.gameObject, 1, 5);
        autoShrink = squareObjectPool.GetAutoShrink();
        autoShrink.StopAutoShrink();

        UpdateHUD();

		StartCoroutine("CreateObjectCoroutine");
	}

    void OnDestroy()
    {
        objectPools.RemoveAllPools();
    }

    // add objects from the scene to the object pool
    private void AddObjects()
    {
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            AddListeners(transform.GetChild(i).gameObject);
        }
        AddChildObjects.Add(this.gameObject);
    }

    private void AddListeners(GameObject gameObject)
    {
        gameObject.GetComponent<TestObjectPoolingMoveObject> ().BecameInvisibleEvent += ObjectBecameInvisibleHandler;
        gameObject.GetComponent<ClickableGameObject> ().MouseUp += ObjectMouseUp;
    }

    private void RemoveListeners(GameObject gameObject)
    {
        gameObject.GetComponent<TestObjectPoolingMoveObject> ().BecameInvisibleEvent -= ObjectBecameInvisibleHandler;
        gameObject.GetComponent<ClickableGameObject> ().MouseUp -= ObjectMouseUp;
    }

	private IEnumerator CreateObjectCoroutine()
	{
		yield return new WaitForSeconds(1);

        CreateObject();

		StartCoroutine("CreateObjectCoroutine");
	}

    private void CreateObject()
    {
        GameObject gameObject = squareObjectPool.GetObject ();
        if (gameObject)
        {
            //Debug.Log("CreateObject -> Got Game Object!!!");
            gameObject.transform.parent = this.transform;
            gameObject.transform.position = new Vector3 (UnityEngine.Random.Range (-6, 6), 0, 10);
            gameObject.transform.rotation = Quaternion.identity;

            AddListeners(gameObject);

            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        txt_ActiveObjects.text = "Active: " + squareObjectPool.activeCount;
        txt_InactiveObjects.text = "Inactive: " + squareObjectPool.inactiveCount;
        txt_TotalObjects.text = "Total: " + squareObjectPool.count;
    }

	// Event handlers
	private void ObjectBecameInvisibleHandler(object sender, EventArgs e)
	{
		//Debug.Log ("ObjectBecameInvisibleHandler -> sender = " + sender);
		TestObjectPoolingMoveObject testObject = (TestObjectPoolingMoveObject)sender;
        RemoveListeners(testObject.gameObject);

        // if object is not in the pool then it must have been destroyed and or removed from the pool
        if(squareObjectPool.ContainsObjectCheck(testObject.gameObject))
        {
            squareObjectPool.DeactivateObject (testObject.gameObject);
        }
        else
        {
            // Don't add the object back into the pool.
           // Debug.Log("ObjectBecameInvisibleHandler -> Object not found in pool!!!");
            GameObject.Destroy(testObject.gameObject);
        }
		
	}

    private void ObjectMouseUp(object sender, EventArgs e)
    {
        //Debug.Log ("ObjectMouseUp -> sender = " + sender);
        ClickableGameObject clickObject = (ClickableGameObject)sender;
        RemoveListeners(clickObject.gameObject);

        // if object is not in the pool then it must have been destroyed and or removed from the pool
        if(squareObjectPool.ContainsObjectCheck(clickObject.gameObject))
        {
            squareObjectPool.DeactivateObject (clickObject.gameObject);
        }
        else
        {
            // Don't add the object back into the pool.
            // Debug.Log("ObjectMouseUp -> Object not found in pool!!!");
            GameObject.Destroy(clickObject.gameObject);
        }
    }


	// Button calls
	public void BtnChangeInitSize()
    {
        initSize = System.UInt32.Parse(intxt_ChangeInitSize.text);
        squareObjectPool.initialPoolSize = initSize;

        intxt_ChangeMaxSize.text = squareObjectPool.maxPoolSize.ToString();
        intxt_ChangeInitSize.text = squareObjectPool.initialPoolSize.ToString();

        UpdateHUD();
	}

	public void BtnChangeMaxSize()
	{
        maxSize = System.UInt32.Parse(intxt_ChangeMaxSize.text);
        squareObjectPool.maxPoolSize = maxSize;

        intxt_ChangeMaxSize.text = squareObjectPool.maxPoolSize.ToString();
        intxt_ChangeInitSize.text = squareObjectPool.initialPoolSize.ToString();

        UpdateHUD();
	}

	public void BtnRemoveObjects()
	{
        squareObjectPool.RemoveObjects();

        UpdateHUD();
	}

	public void BtnShrinkPool()
	{
		squareObjectPool.Shrink();

        UpdateHUD();
	}

	public void BtnDestroyObjects()
	{
        squareObjectPool.DestroyObjects();

        UpdateHUD();
	}

    public void BtnInitPool()
    {
        squareObjectPool.InitializePool();

        UpdateHUD();
    }

    public void BtnCreateObject()
    {
        CreateObject();
    }

	public void BtnDestroyObject()
	{
        int childrenCount = transform.childCount;
        GameObject gameObject;
        if(childrenCount == 1)
        {
            gameObject = transform.GetChild( 0 ).gameObject;
        }
        else
        {
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 0; i < childrenCount; i++)
            {
                gameObject = transform.GetChild( i ).gameObject;
                if(squareObjectPool.ContainsObjectCheck(gameObject))
                {
                    gameObjects.Add(gameObject);
                }
            }

            childrenCount = gameObjects.Count;

            if (childrenCount > 0)
            {
                int randNum = UnityEngine.Random.Range (0, childrenCount - 1);
                gameObject = gameObjects[ randNum ];
                RemoveListeners(gameObject);
                squareObjectPool.DestroyObject(gameObject);
            }
        }
	}

    public void TglAutoShrink(bool value)
    {
        if (tgl_AutoShrink.isOn)
        {
             autoShrink.StartAutoShrink();
        }
        else
        {
            autoShrink.StopAutoShrink();
        }
    }
}