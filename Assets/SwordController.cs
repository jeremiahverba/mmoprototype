using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

    public Transform Sword;
    private GameObject Owner;
    private float AttackStartTime;
    private float AttackDuration = 0.1f;//s
    private float AttackDamageValue = 1.0f;

	// Use this for initialization
	void Start () {       
        AttackStartTime = Time.time;
//        Connect.AddDebugLine("sword started: " + AttackStartTime);
	}
	
	// Update is called once per frame
	void Update () {
        float dif = Time.time - AttackStartTime;
//        Connect.AddDebugLine("sword update: " + dif + ">?" + AttackDuration);
        if (dif > AttackDuration)
        {
            Network.Destroy(gameObject);
        }
	}

    void SetOwner(GameObject owner)
    {
        Owner = owner;

    }
    void OnTriggerEnter2D(Collider2D other)
    {       
//        Connect.AddDebugLine("Sword detecting collision!");
//        Connect.AddDebugLine("other: " + other.gameObject);
//        Connect.AddDebugLine("owner: " + Owner);
//        Connect.AddDebugLine("owner is other? " + (Owner == other.gameObject));
        if (other.gameObject.tag == "Players"  && other.gameObject != Owner)
        {
//            Connect.AddDebugLine("Sword dealing damage to target.");
            other.gameObject.SendMessage("TakeDamage", AttackDamageValue);
            Vector2 force = new Vector2(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
            other.rigidbody2D.AddForce(force);
        }
    }
}
