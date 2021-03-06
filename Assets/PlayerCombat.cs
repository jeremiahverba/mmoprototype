﻿using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour {

    public Transform SwordEntity;
    public float Health = 3.0f;
    private float LastAttackTime;
    public float AttackCooldown = 0.25f; // s

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Network.isServer)
        {           
            if (Health <= 0)
            {
                Connect.AddDebugLine("Player has died.");
                GameObject client = GameObject.FindWithTag("Client");
                client.SendMessage("RespawnPlayer", transform);
                Network.Destroy(gameObject);
            }
        }
        GUIText healthDisplay = GetComponent("GUIText") as GUIText;
        healthDisplay.text = Health.ToString();
	}
    void OnSerializeNetworkView(BitStream b, NetworkMessageInfo i)
    {
        float health = 0f;
        if (b.isWriting)
        {
            health = Health;
            b.Serialize(ref health);
        } else
        {
            b.Serialize(ref health);
            Health = health;
        }
    }
    void TakeDamage(float damage)
    {
        Connect.AddDebugLine("Player Taking Damage: " + damage);
        Health -= damage;
    }

    void DoAttack(Vector3 targetPos)
    {

        if (Network.isServer)
        {
            if(Time.time - LastAttackTime > AttackCooldown)
            {
                LastAttackTime = Time.time;
//                float newX = 1 * Mathf.Cos(transform.eulerAngles.z);
//                float newY = 1 * Mathf.Sin(transform.eulerAngles.z);
                targetPos.z = 0;
                Vector3 dif = targetPos - transform.position;
                dif.Normalize();
                Vector3 newPos = transform.position + dif;
//                Connect.AddDebugLine("player at: " + transform.position.ToString());
//                Connect.AddDebugLine("aiming at: " + targetPos.ToString());
//                Connect.AddDebugLine("making sword at: " + newPos.ToString());
                Transform sword = (Transform)Network.Instantiate(SwordEntity, newPos, transform.rotation, 1); 
                sword.SendMessage("SetOwner", gameObject);
            }
        }
    }
}
