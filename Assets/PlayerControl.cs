using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
		private float MaxSpeed = 5.0f;
		private bool MouseButtonOneDown = false;
		private bool MouseButtonTwoDown = false;
		private bool WestButtonDown = false;
		private bool EastButtonDown = false;
		private bool NorthButtonDown = false;
		private bool SouthButtonDown = false;
		private Vector3 MousePos = Vector2.zero;

		// Use this for initialization
		void Start ()
		{
	
		}

		void Update ()
		{
				if (Network.isClient) {
						if (Input.GetMouseButtonDown (0)) {
								networkView.RPC ("MouseButtonOne", RPCMode.Server, true);
						}
						if (Input.GetMouseButtonUp (0)) {
								networkView.RPC ("MouseButtonOne", RPCMode.Server, false);
						}
						if (Input.GetKeyDown (KeyCode.A)) {
								networkView.RPC ("WestButton", RPCMode.Server, true);
						}
						if (Input.GetKeyUp (KeyCode.A)) {
								networkView.RPC ("WestButton", RPCMode.Server, false);
						}
						if (Input.GetKeyDown (KeyCode.D)) {
								networkView.RPC ("EastButton", RPCMode.Server, true);
						}
						if (Input.GetKeyUp (KeyCode.D)) {
								networkView.RPC ("EastButton", RPCMode.Server, false);
						}
						if (Input.GetKeyDown (KeyCode.S)) {
								networkView.RPC ("SouthButton", RPCMode.Server, true);
						}
						if (Input.GetKeyUp (KeyCode.S)) {
								networkView.RPC ("SouthButton", RPCMode.Server, false);
						}
						if (Input.GetKeyDown (KeyCode.W)) {
								networkView.RPC ("NorthButton", RPCMode.Server, true);
						}
						if (Input.GetKeyUp (KeyCode.W)) {
								networkView.RPC ("NorthButton", RPCMode.Server, false);
						}
						Vector3 tempVec = Input.mousePosition;
						Vector3 dif = tempVec - Camera.main.WorldToScreenPoint (transform.position);
						float angle = Mathf.Atan2 (dif.x, dif.y) * Mathf.Rad2Deg;
						Connect.AddDebugLine ("sending our facing angle to server.");
						networkView.RPC ("SetAngle", RPCMode.Server, angle);
				}
		}
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (Network.isServer) {
						float newXVel = 0f;
						float newYVel = 0f;
						if (WestButtonDown) {
								newXVel += -MaxSpeed;
						} 
						if (EastButtonDown) {
								newXVel += MaxSpeed;

						}
						if (NorthButtonDown) {
								newYVel += MaxSpeed;
						} 
						if (SouthButtonDown) {
								newYVel += -MaxSpeed;
						}
						rigidbody2D.velocity = new Vector2 (newXVel, newYVel);
//		Vector2 mouse2 = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
//			Vector3 tempVec = MousePos;//Input.mousePosition;
//		tempVec.z = tempVec.y;
//		tempVec.y = 0;
//						Vector3 dif = tempVec - Camera.main.WorldToScreenPoint (transform.position);
//						float angle = Mathf.Atan2 (dif.x, dif.y) * Mathf.Rad2Deg;
//		Quaternion q = new Quaternion(0, 0, 1, angle);
//						transform.eulerAngles = new Vector3 (0, 0, -angle);

				}
		}
	
		void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
		{
//		Connect.AddDebugLine ("got OnSerializeNetworkView call. Stream is writing: " + stream.isWriting);
				Vector3 syncVelocity = Vector3.zero;
				Vector3 syncAngle = Vector3.zero;
				if (stream.isWriting) {
						syncVelocity = new Vector3 (rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
						stream.Serialize (ref syncVelocity);
						syncAngle = transform.eulerAngles;
						stream.Serialize (ref syncAngle);

				} else {
						stream.Serialize (ref syncVelocity);
						rigidbody2D.velocity = new Vector2 (syncVelocity.x, syncVelocity.y);
						stream.Serialize (ref syncAngle);
						transform.eulerAngles = syncAngle;
				}
		}

		[RPC]
		void MouseButtonOne (bool down)
		{
//				Connect.AddDebugLine ("server got mouse button one down: " + down);
				MouseButtonOneDown = down;
		}

		[RPC]
		void WestButton (bool down)
		{
//				Connect.AddDebugLine ("server got west button down: " + down);
				WestButtonDown = down;
		}

		[RPC]
		void EastButton (bool down)
		{
//				Connect.AddDebugLine ("server got east button down: " + down);
				EastButtonDown = down;
		}

		[RPC]
		void NorthButton (bool down)
		{
//				Connect.AddDebugLine ("server got north button down: " + down);
				NorthButtonDown = down;
		}

		[RPC]
		void SouthButton (bool down)
		{
//				Connect.AddDebugLine ("server got south button down: " + down);
				SouthButtonDown = down;
		}

		[RPC]
	void SetAngle (float angle)
		{
		Connect.AddDebugLine ("server got new angle: " + angle);
				transform.eulerAngles = new Vector3 (0, 0, -angle);
		}
}
