using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Connect : MonoBehaviour
{
    public Transform Character;
    private const string QUERY_PHASE = "queryphase";
    private const string QUERYWAITING_PHASE = "querywaitingphase";
    private const string NEEDSERVER_PHASE = "needserverphase";
    private const string SERVERSTARTING_PHASE = "serverstartingphase";
    private const string SERVERRUNNING_PHASE = "serverrunningphase";
    private const string SERVERWAITING_PHASE = "serverwaitingphase";
    private const string JOIN_PHASE = "joinphase";
    private const string ERROR_PHASE = "errorphase";
    private const string CONNECTING_PHASE = "connectingphase";
    private const string WAITINGTOCONNECT_PHASE = "waitingtoconnectphase";
    private const string CONNECTED_PHASE = "connectedphase";
    private const string DISCONNECTED_PHASE = "disconnectedphase";
    private string gameName = "com.crotchpunchstudios.mmoprototype";
    private string bootPhase = QUERY_PHASE;
    private static string debugString;
    private HostData[] hostList;
    private Dictionary<NetworkPlayer, Transform> clientPlayers;
    private Vector2 ScrollPos;
    void Start()
    {
        clientPlayers = new Dictionary<NetworkPlayer, Transform>();
    }

    void OnGUI()
    {
        if (Network.isServer || (Network.isClient && Input.GetKey(KeyCode.BackQuote)))
        {
            ScrollPos = GUILayout.BeginScrollView(ScrollPos);            
            GUILayout.TextArea(debugString);                    
            GUILayout.EndScrollView();
        }
        if (bootPhase == CONNECTED_PHASE)
        {
//          GUI.BeginGroup(new Rect((Screen.width - 500) / 2, (Screen.height - 500) / 2, 500, 500));
//          GUILayout.Label("Enter your name:");
//          string name = "";
//          name = GUILayout.TextField(name, 15);
//          if(GUILayout.Button("Spawn")){
//              RequestPlayer(name);
//          }
//          GUI.EndGroup();
        }

    }

    public static void AddDebugLine(string line)
    {
        debugString += line + "\n";
    }
    // Update is called once per frame
    void Update()
    {
        switch (bootPhase)
        {
            case QUERY_PHASE:
                debugString += "Checking to see if a server exists already..." + "\n";
                MasterServer.ClearHostList();
                MasterServer.RequestHostList(gameName);
                bootPhase = QUERYWAITING_PHASE;
                break;
            case QUERYWAITING_PHASE:
                break;
            case NEEDSERVER_PHASE:
                        
                bool useNat = !Network.HavePublicAddress();
                debugString += "No server found. Attempting to start one with game name: " + gameName + " with nat setting: " + useNat + "\n";
                Network.InitializeServer(100, 2000, useNat);
                bootPhase = SERVERSTARTING_PHASE;
                Camera.main.orthographicSize = 30; // Expand the camera to see the entire map
                break;
            
            case SERVERRUNNING_PHASE:
                debugString += "Server waiting for players: " + bootPhase + "\n";
                bootPhase = SERVERWAITING_PHASE;
                break;  
            
            case SERVERWAITING_PHASE:
                break;
            
            case JOIN_PHASE:
                debugString += "Server found: " + hostList [0].gameName + ".  Attempting connection!\n";
                debugString += "host info:\n";
                debugString += hostList [0].gameName + "\n";
                debugString += hostList [0].gameType + "\n";
                debugString += hostList [0].guid.ToString() + "\n";
                foreach (string s in hostList[0].ip)
                {
                    debugString += s + ".";
                }
                debugString += "\n";
                debugString += hostList [0].port + "\n";
                debugString += hostList [0].playerLimit + "\n";
                debugString += hostList [0].useNat + "\n";
                debugString += hostList [0].passwordProtected + "\n";
                debugString += hostList [0].comment + "\n";
                debugString += hostList [0].connectedPlayers + "\n";
                debugString += hostList [0].ToString() + "\n";

                Network.Connect(hostList [0]);
                bootPhase = CONNECTING_PHASE;
                break;
            
            case CONNECTING_PHASE:
                debugString += "Waiting to connect...\n";
                bootPhase = WAITINGTOCONNECT_PHASE;
                break;
            
            case WAITINGTOCONNECT_PHASE:
                break;
            
            case CONNECTED_PHASE:
                break;
            
            default:
                break;
        }
    }

    void OnServerInitialized()
    {
        debugString += "Server initialized, beginning host registration.\n";
        MasterServer.RegisterHost(gameName, "Test Instance of MMOPrototype", "some optional comment"); 
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        switch (mse)
        {
            case MasterServerEvent.HostListReceived:
                if (bootPhase == QUERYWAITING_PHASE)
                {
                    hostList = MasterServer.PollHostList();
                    debugString += "hostList.length: " + hostList.Length + "\n";
                    bootPhase = hostList.Length != 0 ? JOIN_PHASE : NEEDSERVER_PHASE;
                }
                break;
            case MasterServerEvent.RegistrationSucceeded:
                
                if (mse == MasterServerEvent.RegistrationSucceeded)
                {
                    debugString += "Server Registered Successfully!\n";
                    bootPhase = SERVERRUNNING_PHASE;
                }
                break;
        }
    }

    void OnConnectedToServer()
    {
        debugString += "We're Connected!\n";
        bootPhase = CONNECTED_PHASE;
    }

    void OnPlayerConnected(NetworkPlayer p)
    {
        debugString += "A Player Connected: " + p.ipAddress + "\n";
//        Network.Instantiate(Character, Vector3.zero, Quaternion.identity, 0);
        SpawnPlayer(p);
    }
    
    void SpawnPlayer(NetworkPlayer p)
    {
        Transform spawnPoint;
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");
        int index = Random.Range(0, spawns.Length);
        spawnPoint = spawns [index].transform;
        Transform newCharacter = (Transform)Network.Instantiate(Character, spawnPoint.position, spawnPoint.rotation, 0);
        NetworkView client = newCharacter.networkView;
        client.RPC("SetCharacter", RPCMode.AllBuffered, p);
        clientPlayers.Add(p, newCharacter);
    }

    void OnFailedToConnect(NetworkConnectionError e)
    {
        debugString += "Failed To Connect: " + e + "\n";
    }

    void OnPlayerDisconnected(NetworkPlayer p)
    {
        debugString += "A Player Disconnected: " + p.ipAddress + "\n";

        Transform characterToDestroy = clientPlayers[p];
        Destroy(characterToDestroy.gameObject);
        clientPlayers.Remove(p);

        Network.RemoveRPCs(p);
        Network.DestroyPlayerObjects(p);
    }
}
