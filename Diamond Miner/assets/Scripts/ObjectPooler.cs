using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
	public static ObjectPooler Instance;

	[System.Serializable]
	public struct PoolableObject
	{
		// Number of objects instantiated and stored on startup
		public int pooledAmount;
		// Object to instantiate
		public GameObject obj;
	}

	[SerializeField]
	List<PoolableObject> poolDefinitions = new List<PoolableObject>();

	Dictionary<string, List<GameObject>> poolDictionary;
	Dictionary<string, GameObject> prefabDictionary;

	void Awake()
	{
		Instance = this;

		poolDictionary = new Dictionary<string, List<GameObject>>();
		prefabDictionary = new Dictionary<string, GameObject>();
		
		foreach(PoolableObject p in poolDefinitions)
		{
			// Don't make multiple pools for the same object
			if (poolDictionary.ContainsKey(p.obj.name))
			{
				Debug.LogWarning("Object pooler already contains a pool for this object name!");
			}
			else
			{
				// Instantiate as many objects as requested
				var list = new List<GameObject>();
				for (int i = 0; i < p.pooledAmount; ++i)
				{
					var newObj = CreatePooledObject(p.obj);
					newObj.transform.SetParent(transform);
					list.Add(newObj);
				}
				
				// Add objects to our pool dictionary for later retrieval
				poolDictionary.Add(p.obj.name, list);
				prefabDictionary.Add(p.obj.name, p.obj);
			}
		}
	}

	/// <summary>
	/// GetPooledObject - retrieves an inactive pooled object, or instantiates
	/// a new object and returns it.
	/// </summary>
	/// <param name="name">The name of the object.</param>
	/// <returns>An inactive pooled object, or null if the object type is not in the pool.</returns>
	public GameObject GetPooledObject(string name)
	{
		if (poolDictionary.ContainsKey(name))
		{
			GameObject foundObj = null;
			// Find a deactivated object if available
			foreach(GameObject obj in poolDictionary[name])
			{
				if (!obj.activeInHierarchy)
				{
					foundObj = obj;
					break;
				}
			}

			if (foundObj)
			{
				foundObj.transform.SetParent(null);
			}
			else
			{
				// A deactivated object wasn't available, instantiate a new object
				foundObj = CreatePooledObject(prefabDictionary[name]);
			}
			return foundObj;
		}

		return null;
	}

	/// <summary>
	/// ReturnPooledObject - returns a GameObject to the pool.
	/// </summary>
	/// <param name="obj">The object to return to the pool.</param>
	public void ReturnPooledObject(GameObject obj)
	{
		if (poolDictionary.ContainsKey(obj.name))
		{
			obj.transform.SetParent(transform);
			obj.SetActive(false);
			poolDictionary[obj.name].Add(obj);
		}
		else
		{
			Debug.LogWarning("Tried to return an object to the pool that wasn't part of the pool!");
		}
	}

	/// <summary>
	/// Creates a new object from a prefab, deactivates it, and returns it.
	/// </summary>
	/// <param name="prefab">The GameObject to instantiate.</param>
	/// <returns>The deactivated, instantiated game object.</returns>
	GameObject CreatePooledObject(GameObject prefab)
	{
		GameObject obj = (GameObject)Instantiate(prefab);
		obj.name = prefab.name;
		// Default object to inactive
		obj.SetActive(false);
		return obj;
	}
}