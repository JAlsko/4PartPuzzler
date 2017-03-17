using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed = 10f;
	public float jumpForce = 500f;

	public GameObject pla, pll, pra, prl; //body part pickup prefabs
	private GameObject la, ll, ra, rl, lll, rll; //body part game objects
	private GameObject gcl, gcm, gcr; //ground check objects
	private GameObject lwct, lwcm, lwcb; //left wall check objects
	private GameObject rwct, rwcm, rwcb; //right wall check objects
	private GameObject mwct, mwcm, mwcb; //mid wall check objects
	public LayerMask groundLayer;
	public bool laa, lla, raa, rla = false; //status of body parts ('laa' -> left arm attached)
	private bool noArms, noLegs = false;
	public bool grounded = false; //variable to track if player is on ground
	public bool onLeftWall, onRightWall = false; //variables to track if player is on a wall to the right/left

	void Start () {
		Physics2D.IgnoreLayerCollision (8, 8, true); //ignore collisions between body parts
		Physics2D.IgnoreLayerCollision (8, 9, true); //ignore collisions between player and pickups

		//get player's body parts
		la = GameObject.Find ("Player Left Arm");
		ll = GameObject.Find ("Player Left Leg");
		ra = GameObject.Find ("Player Right Arm");
		rl = GameObject.Find ("Player Right Leg");
		lll = GameObject.Find ("Player Left Leg Mover");
		rll = GameObject.Find ("Player Right Leg Mover");

		gcl = GameObject.Find ("Left Ground Check");
		gcm = GameObject.Find ("Mid Ground Check");
		gcr = GameObject.Find ("Right Ground Check");

		lwct = GameObject.Find ("Top Left Check");
		lwcm = GameObject.Find ("Mid Left Check");
		lwcb = GameObject.Find ("Bottom Left Check");

		rwct = GameObject.Find ("Top Right Check");
		rwcm = GameObject.Find ("Mid Right Check");
		rwcb = GameObject.Find ("Bottom Right Check");

		mwct = GameObject.Find ("Top Mid Check");
		mwcm = GameObject.Find ("Mid Mid Check");
		mwcb = GameObject.Find ("Bottom Mid Check");

		rla = true; //start with right leg attached
		//lla = true;
		//laa = true;
		//raa = true;

		//enable/disable body parts depending if they're attached
		la.SetActive (laa);
		ll.SetActive (lla);
		ra.SetActive (raa);
		rl.SetActive (rla);
		lll.SetActive (lla);
		rll.SetActive (rla);
	}
	
	void Update () {
		if (Input.GetButtonDown("Jump")) {
			if (!grounded && (noLegs || ((!lla || !rla) && noArms))) {
				Debug.Log ("No parts left to jump with");
			} else {
				if (!grounded && lla && (!noArms || rla)) {
					lla = false; 
					ll.SetActive (false);
					lll.SetActive (false);
					Instantiate (pll, this.transform.position + (Vector3.down * 0f), Quaternion.identity, null);
				} 
				else if (!grounded && rla && (!noArms || lla)) {
					rla = false; 
					rl.SetActive (false);
					rll.SetActive (false);
					Instantiate (prl, this.transform.position + (Vector3.down * 0f), Quaternion.identity, null);
				}
				grounded = false;
				Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
				rb.velocity = new Vector2 (rb.velocity.x, 0);
				rb.AddForce (Vector2.up * jumpForce);
			}
		}

		if (Input.GetButtonDown ("Use")) {
			if (onLeftWall) {
				if (laa && (!noLegs || raa)) {
					laa = false;
					la.SetActive (false);
					Instantiate (pla, this.transform.position + (Vector3.down * 1f), Quaternion.identity, null);
				} 
				else if (raa && (!noLegs || laa)) {
					raa = false;
					ra.SetActive (false);
					Instantiate (pra, this.transform.position + (Vector3.down * 1f), Quaternion.identity, null);
				}
			}
			else if (onRightWall) {
				if (raa && (!noLegs || laa)) {
					raa = false;
					ra.SetActive (false);
					Instantiate (pra, this.transform.position + (Vector3.down * 1f), Quaternion.identity, null);
				} 
				else if (laa && (!noLegs || raa)) {
					laa = false;
					la.SetActive (false);
					Instantiate (pla, this.transform.position + (Vector3.down * 1f), Quaternion.identity, null);
				}
			}
		}
	}

	void FixedUpdate() {
		noArms = !laa && !raa;
		noLegs = !lla && !rla;
		float hMovement = Input.GetAxis ("Horizontal");
		//if ((hMovement > 0 && !onRightWall) || (hMovement < 0 && !onLeftWall) || hMovement == 0) {
		//	transform.Translate (Vector3.right * hMovement * speed * Time.deltaTime);
		//}
		this.GetComponent<Rigidbody2D>().velocity = new Vector2(hMovement * speed, this.GetComponent<Rigidbody2D>().velocity.y);

		if (lla) {
			if (rla) { //left leg and right leg attached, use whole ground check
				grounded = Physics2D.OverlapArea (
					new Vector2 (gcl.transform.position.x, gcl.transform.position.y), 
					new Vector2 (gcr.transform.position.x, gcr.transform.position.y));
			}
			else if (!rla) { //left leg, not right leg attached, use left-mid ground check
				grounded = Physics2D.OverlapArea (
					new Vector2 (gcl.transform.position.x, gcl.transform.position.y), 
					new Vector2 (gcm.transform.position.x, gcm.transform.position.y));
			}
		} 

		else if (!lla) {
			if (rla) { //right leg, not left leg attached, use mid-right ground check
				grounded = Physics2D.OverlapArea (
					new Vector2(gcm.transform.position.x, gcm.transform.position.y), 
					new Vector2(gcr.transform.position.x, gcr.transform.position.y));
			}

		}

		if (laa && lla) { //whole left wall check
			onLeftWall = Physics2D.OverlapArea (
				new Vector2 (lwct.transform.position.x, lwct.transform.position.y),
				new Vector2 (lwcb.transform.position.x, lwcb.transform.position.y), groundLayer);
		} else if (laa && !lla) { //top-mid left wall check
			onLeftWall = Physics2D.OverlapArea (
				new Vector2 (lwct.transform.position.x, lwct.transform.position.y),
				new Vector2 (lwcm.transform.position.x, lwcm.transform.position.y), groundLayer);
		} else if (!laa && lla) { //mid-bottom left wall check
			onLeftWall = Physics2D.OverlapArea (
				new Vector2 (lwcm.transform.position.x, lwcm.transform.position.y),
				new Vector2 (lwcb.transform.position.x, lwcb.transform.position.y), groundLayer);
		} else if (!laa && !lla) {
			if (raa && rla) { //mid top-bottom left wall check
				onLeftWall = Physics2D.OverlapArea (
					new Vector2 (mwct.transform.position.x, mwct.transform.position.y),
					new Vector2 (mwcb.transform.position.x, mwcb.transform.position.y), groundLayer);
			} 
			else if (raa && !rla) { //mid top-mid left wall check
				onLeftWall = Physics2D.OverlapArea (
					new Vector2 (mwct.transform.position.x, mwct.transform.position.y),
					new Vector2 (mwcm.transform.position.x, mwcm.transform.position.y), groundLayer);
			}
		}

		if (raa && rla) { //whole right wall check
			onRightWall = Physics2D.OverlapArea (
				new Vector2 (rwct.transform.position.x, rwct.transform.position.y),
				new Vector2 (rwcb.transform.position.x, rwcb.transform.position.y), groundLayer);
		} else if (raa && !rla) { //top-mid right wall check
			onRightWall = Physics2D.OverlapArea (
				new Vector2 (rwct.transform.position.x, rwct.transform.position.y),
				new Vector2 (rwcm.transform.position.x, rwcm.transform.position.y), groundLayer);
		} else if (!raa && rla) { //mid-bottom right wall check
			onRightWall = Physics2D.OverlapArea (
				new Vector2 (rwcm.transform.position.x, rwcm.transform.position.y),
				new Vector2 (rwcb.transform.position.x, rwcb.transform.position.y), groundLayer);
		} else if (!raa && !rla) {
			if (laa && lla) { //mid top-bottom right wall check
				onRightWall = Physics2D.OverlapArea (
					new Vector2 (mwct.transform.position.x, mwct.transform.position.y),
					new Vector2 (mwcb.transform.position.x, mwcb.transform.position.y), groundLayer);
			} 
			else if (laa && !lla) { //mid top-mid right wall check
				onRightWall = Physics2D.OverlapArea (
					new Vector2 (mwct.transform.position.x, mwct.transform.position.y),
					new Vector2 (mwcm.transform.position.x, mwcm.transform.position.y), groundLayer);
			}
		}



//		if (lla && rla) {
//			grounded = Physics2D.OverlapArea (
//				new Vector2(gcl.transform.position.x, gcl.transform.position.y), 
//				new Vector2(gcr.transform.position.x, gcr.transform.position.y));
//		}
//
//		else if (lla && !rla) {
//			grounded = Physics2D.OverlapArea (
//				new Vector2(gcl.transform.position.x, gcl.transform.position.y), 
//				new Vector2(gcm.transform.position.x, gcm.transform.position.y));
//		}
//
//		else if (!lla && rla) {
//			grounded = Physics2D.OverlapArea (
//				new Vector2(gcm.transform.position.x, gcm.transform.position.y), 
//				new Vector2(gcr.transform.position.x, gcr.transform.position.y));
//		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "LeftArm" && !laa) { laa = true; la.SetActive (true); Destroy (col.gameObject);}
		if (col.gameObject.tag == "LeftLeg" && !lla) { lla = true; ll.SetActive (true); lll.SetActive(true); Destroy (col.gameObject);}
		if (col.gameObject.tag == "RightArm" && !raa) { raa = true; ra.SetActive (true); Destroy (col.gameObject);}
		if (col.gameObject.tag == "RightLeg" && !rla) { rla = true; rl.SetActive (true); rll.SetActive(true); Destroy (col.gameObject);}

	}
}
