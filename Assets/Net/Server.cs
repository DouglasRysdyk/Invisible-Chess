using System; 
using Unity.Collections;
using Unity.Networking.Transport; 
using UnityEngine;

//Would be safer as an actual singleton implementation since you wouldn't be able to accidentally have two singleton connections.
//Look up how to do a singleton implementation.
//Make a branch or a fork where I reimplement this as a singleton?
public class Server : MonoBehaviour
{
    #region "Singleton implementation" 
    public static Server Instance { set; get; }

    private void Awake()
    {
        Instance = this; 
    }
    #endregion

    //I don't know what the network driver is or does (01-06-2022 @11:42)
    public NetworkDriver driver; 
    //A list of all connections
    private NativeList<NetworkConnection> connections; 

    //Is the server active? 
    private bool isActive = false; 
    //These prevent the server from dropping suddenly.  
    private const float keepAliveTickRate = 20.0f; 
    //Timestamp recording when we last sent a keep alive tick.
    private float lastKeepAlive; 

    //Use if someone's connection drops.  
    public Action connectionsDropped; 

    //METHODS
    //Will be run on local host or public IPv6.  
    //I guess we deal with the IP address later? (01-06-2022 @11:45)
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        //Where clients want to connect 
        //Can also do a loopback only for local hosting
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (driver.Bind(endPoint) != 0 )
        {
             Debug.Log("Unable to bind on port " + endPoint.Port);
             return;
        }
        else 
        {
            driver.Listen();
             Debug.Log("Currently listening on port " + endPoint.Port);
        }

        //Set max amount of connections
        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true; 
    }
    public void ShutDown()
    {
        if (isActive)
        {
            driver.Dispose();
            connections.Dispose();
            isActive = false; 
        }
    }
    public void OnDestroy()
    {
        Shutdown();
    }

    public void Update()
    {
        //Break out of the update function if the server is not alive 
        if (!isActive)
            return; 

        //Sends a mesage every 20 seconds 
//        KeepAlive(); 

        //Empties the incoming cue of messages  
        driver.ScheduleUpdate().Complete();

        CleanUpConnections(); //Are there any existing connections for people who are no longer connected 
        AcceptNewConnections(); //Is someone trying to get in 
        UpdateMessagePump(); //Is someone sending us a message?  Do we have to reply?  
    }
    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            //If a connection has dropped it is no longer created, and therefore...
            if (!connections[i].IsCreated)
            {
                //The connection is removed at swap back (whatever that is).
                connections.RemoveAtSwapBack(i); 
                //Don't break the loop.  
                --i; 
            }
        }
    }
    private void AcceptNewConnections ()
    {
        NetworkConnection c; 

        //As long as the max number of connections has not been reached or if they are not there then add them to the server.  
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c); 
        }
    }
    private void UpdateMessagePump()
    {
        //Used for messages 
        DataStreamReader stream; 
        //There are two people connecting: the host and the client 
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd; 
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
//                    NetUtility.OnData(stream, connections[i], this);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionsDropped?.Invoke();
                    //This is not something you normally do but since this is a two player server this makes sense.
                    ShutDown(); 
                }
            }
        }
    }

    //SERVER SPECIFIC 
    //Send a message to a specific person.  
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;                  //This is a box.  
        driver.BeginSend(connection, out writer); //When we have a box we put the address on top of it so we know where to send it.  
//        msg.Serialize(ref writer);                //We put stuff inside the box.  
        driver.EndSend(writer);                   //We deliver the box.  
    }
    //Send a message to every person in the server.  
    public void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++) //Get a list of everyone.  
        {
            if (connections[i].IsCreated)            //Make sure they have a connection.  
            {
//                Debug.Log($"Sending {msg.Code} to : {connections[i].InternalId}");
                SendToClient(connections[i], msg);   //Send the message to everyone.  
            }
        }
    }
}

//"80" is the port to listen to the HTML protocol 
