using UnityEngine;
using System.Collections;

// GameObjects being pooled should extend this class
public class PoolObject : MonoBehaviour {
	
	public virtual void OnObjectReuse() {

	}

	protected void Destroy() {
		gameObject.SetActive(false);
	}

}