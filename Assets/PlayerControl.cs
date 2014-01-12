using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    private NetworkPlayer theOwner;
    private float MaxSpeed = 5.0f;
    private bool MouseButtonOneDown = false;
    private Vector3 MouseButtonOneTargetPos;
    private bool MouseButtonTwoDown = false;
    private Vector3 MOuseButtonTwoTargetPos;
    private Vector3 MousePos = Vector2.zero;
    private bool WestButtonDown = false;
    private bool EastButtonDown = false;
    private bool NorthButtonDown = false;
    private bool SouthButtonDown = false;
    private bool ActionButton1Down = false;
    private bool ActionButton2Down = false;
    private bool ActionButton3Down = false;
    private bool ActionButton4Down = false;
    private bool ActionButton5Down = false;


    // Use this for initialization
    void Start()
    {
        if (Network.isClient)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (theOwner != null && Network.player == theOwner && Network.isClient)
        {
            if (Input.GetMouseButtonDown(0))
            {
                networkView.RPC("MouseButtonOne", RPCMode.Server, true, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonUp(0))
            {
                networkView.RPC("MouseButtonOne", RPCMode.Server, false, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonDown(1))
            {
                    networkView.RPC("MouseButtonTwo", RPCMode.Server, true, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonUp(1))
            {
                        networkView.RPC("MouseButtonTwo", RPCMode.Server, false, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                networkView.RPC("WestButton", RPCMode.Server, true);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                networkView.RPC("WestButton", RPCMode.Server, false);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                networkView.RPC("EastButton", RPCMode.Server, true);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                networkView.RPC("EastButton", RPCMode.Server, false);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                networkView.RPC("SouthButton", RPCMode.Server, true);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                networkView.RPC("SouthButton", RPCMode.Server, false);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                networkView.RPC("NorthButton", RPCMode.Server, true);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                networkView.RPC("NorthButton", RPCMode.Server, false);
            }

            networkView.RPC("ActionButton1", RPCMode.Server, Input.GetKeyDown(KeyCode.Alpha1));
            networkView.RPC("ActionButton2", RPCMode.Server, Input.GetKeyDown(KeyCode.Alpha2));
            networkView.RPC("ActionButton3", RPCMode.Server, Input.GetKeyDown(KeyCode.Alpha3));
            networkView.RPC("ActionButton4", RPCMode.Server, Input.GetKeyDown(KeyCode.Alpha4));
            networkView.RPC("ActionButton5", RPCMode.Server, Input.GetKeyDown(KeyCode.Alpha5));

            Vector3 tempVec = Input.mousePosition;
            Vector3 dif = tempVec - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dif.x, dif.y) * Mathf.Rad2Deg;
            networkView.RPC("SetAngle", RPCMode.Server, angle);
        }
//    }
    // Update is called once per frame
//    void Update()
//    {
        if (Network.isServer)
        {
            if(MouseButtonOneDown)
            {
                gameObject.SendMessage("DoAttack", MouseButtonOneTargetPos);
            }
            float newX = 0f;
            float newY = 0f;
            if (WestButtonDown)
            {
                newX = -MaxSpeed;
            } 
            if (EastButtonDown)
            {
                newX = MaxSpeed;

            }
            if (NorthButtonDown)
            {
                newY = MaxSpeed;
            } 
            if (SouthButtonDown)
            {
                newY = -MaxSpeed;
            }
//            Vector3 dir = new Vector3(newXVel, newYVel, 0);
            Vector3 newPos = new Vector3(transform.position.x + newX, transform.position.y + newY, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
//            transform.Translate(MaxSpeed * dir * Time.deltaTime);
        }
    }
    
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
//      Connect.AddDebugLine ("got OnSerializeNetworkView call. Stream is writing: " + stream.isWriting);
//        Vector3 syncVelocity = Vector3.zero;
        Vector3 syncAngle = Vector3.zero;
        if (stream.isWriting)
        {
//            syncVelocity = new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
//            stream.Serialize(ref syncVelocity);
            syncAngle = transform.eulerAngles;
            stream.Serialize(ref syncAngle);

        } else
        {
//            stream.Serialize(ref syncVelocity);
//            rigidbody2D.velocity = new Vector2(syncVelocity.x, syncVelocity.y);
            stream.Serialize(ref syncAngle);
            transform.eulerAngles = syncAngle;
        }
    }

    [RPC]
    void SetCharacter(NetworkPlayer p)
    {
        theOwner = p;
        if (p == Network.player)
        {
            enabled = true;
            Camera.main.SendMessage("SetTarget", transform);
        }
    }

    [RPC]
    void SetAngle(float angle)
    {
        //              Connect.AddDebugLine ("server got new angle: " + angle);
        transform.eulerAngles = new Vector3(0, 0, -angle);
    }

    [RPC]
    void MouseButtonOne(bool down, Vector3 targetPos)
    {
//              Connect.AddDebugLine ("server got mouse button one down: " + down);
        MouseButtonOneDown = down;
        MouseButtonOneTargetPos = targetPos;
    }

    [RPC]
    void MouseButtonTwo(bool down, Vector3 targetPos)
    {
        //              Connect.AddDebugLine ("server got mouse button two down: " + down);
        MouseButtonTwoDown = down;
        MOuseButtonTwoTargetPos = targetPos;
    }
    
    [RPC]
    void WestButton(bool down)
    {
//              Connect.AddDebugLine ("server got west button down: " + down);
        WestButtonDown = down;
    }

    [RPC]
    void EastButton(bool down)
    {
//              Connect.AddDebugLine ("server got east button down: " + down);
        EastButtonDown = down;
    }

    [RPC]
    void NorthButton(bool down)
    {
//              Connect.AddDebugLine ("server got north button down: " + down);
        NorthButtonDown = down;
    }

    [RPC]
    void SouthButton(bool down)
    {
//              Connect.AddDebugLine ("server got south button down: " + down);
        SouthButtonDown = down;
    }
    [RPC]
    void ActionButton1(bool down)
    {
//                      Connect.AddDebugLine ("server got action button 1 down: " + down);
        ActionButton1Down = down;
    }
    [RPC]
    void ActionButton2(bool down)
    {
//                      Connect.AddDebugLine ("server got action button 2 down: " + down);
        ActionButton2Down = down;
    }
    [RPC]
    void ActionButton3(bool down)
    {
//                      Connect.AddDebugLine ("server got action button 3 down: " + down);
        ActionButton3Down = down;
    }
    [RPC]
    void ActionButton4(bool down)
    {
//                      Connect.AddDebugLine ("server got action button 4 down: " + down);
        ActionButton4Down = down;
    }
    [RPC]
    void ActionButton5(bool down)
    {
//                      Connect.AddDebugLine ("server got action button 5 down: " + down);
        ActionButton5Down = down;
    }
}
