using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScript01 : MonoBehaviour {

	public float time = .5f;

	void Start () {
		Invoke ("EnableCollision", time);
	}
	
	void Update () {
		
	}

	void EnableCollision() {
		this.GetComponent<BoxCollider2D> ().enabled = true;
	}
}
