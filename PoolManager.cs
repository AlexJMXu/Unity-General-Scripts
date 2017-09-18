using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {
	Dictionary<int, Queue<GameInstance>> poolDictionary = new Dictionary<int, Queue<GameInstance>>();

	static PoolManager _instance;

	public static PoolManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<PoolManager>();
			}
			return;
		}
	}

	public void CreatePool(GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID();

		if (!poolDictionary.ContainsKey(poolKey)) {
			poolDictionary.Add(poolKey, new Queue<GameObject>());

			GameObject poolHolder = new GameObject (prefab.name + " pool");
			poolHolder.transform.parent = transform;

			for (int i = 0; i < poolSize; i++) {
				ObjectInstance newObject = new ObjectInstance((GameObject) Instantiate (prefab));
				poolDictionary[poolKey].Enqueue(newObject);
				newObject.SetParent(poolHolder.transform);
			}
		}
	}

	public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation) {
		int poolKey = prefab.GetInstanceID();

		if (poolDictionary.ContainsKey(poolKey)) {
			ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
			poolDictionary[poolKey].Enqueue(objectToReuse);

			objectToReuse.Reuse(position, rotation);
		}
	}

	public class ObjectInstance {
		GameObject gameObject;
		Transform transform;

		bool hasPoolObjectComponent;
		PoolObject poolObjectScript;

		public ObjectInstance(GameObject objectInstance) {
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive(false);

			if (gameObject.GetComponent<PoolObject>()) {
				hasPoolObjectComponent = true;
				poolObjectScript = gameObject.GetComponent<PoolObject>();
			}
		}

		public void Reuse(Vector3 position, Quaternion rotation) {
			gameObject.SetActive(true);
			transform.position = position;
			transform.rotation = rotation;

			if (hasPoolObjectComponent) {
				poolObjectScript.OnObjectReuse();
			}
		}

		public void SetParent(Transform parent) {
			transform.parent = parent;
		}
	}
}