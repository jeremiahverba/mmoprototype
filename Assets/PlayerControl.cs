using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

		private float MaxSpeed = 5.0f;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				float newXVel = 0f;
				float newYVel = 0f;
				if (Input.GetKey(KeyCode.A)) {
						newXVel += -MaxSpeed;
				} 
				if (Input.GetKey(KeyCode.D)) {
						newXVel += MaxSpeed;

				}
				if (Input.GetKey(KeyCode.W)) {
						newYVel += MaxSpeed;
				} 
				if (Input.GetKey(KeyCode.S)) {
						newYVel += -MaxSpeed;
				}
				rigidbody2D.velocity = new Vector2 (newXVel, newYVel);
//		Vector2 mouse2 = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		Vector3 tempVec = Input.mousePosition;
//		tempVec.z = tempVec.y;
//		tempVec.y = 0;
		Vector3 dif = tempVec - Camera.main.WorldToScreenPoint (transform.position);
		float angle = Mathf.Atan2 (dif.x, dif.y) * Mathf.Rad2Deg;
//		Quaternion q = new Quaternion(0, 0, 1, angle);
		transform.eulerAngles = new Vector3 (0, 0, -angle);

		}
}
