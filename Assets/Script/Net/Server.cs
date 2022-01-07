using System; 
using Unity.Collections;
using Unity.Networking.Transport; 
using UnityEngine;

//Would be safer as an actual singleton implementation since you wouldn't be able to accidentally have two singleton connections.
//Look up how to do a singleton implementation.
//Make a branch or a fork where I reimplement this as a singleton?
//MonoSingleton<Server> 
public class Server : MonoBehaviour
{
    #region "Singleton implementation" 
    public static Server Instance { set; get; }

    private void Awake()
    {
        Instance = this; 
    }
    #endregion

    public NetworkDriver driver;                       //I don't know what the network driver is or does (01-06-2022 @11:42)
    private NativeList<NetworkConnection> connections; //A list of all connections
    private bool isActive = false;                     //Is the server active? 
    private const float keepAliveTickRate = 20.0f;     //These prevent the server from dropping suddenly. 
    private float lastKeepAlive;                       //Timestamp recording when we last sent a keep alive tick.
    public Action connectionsDropped;                  //Use if someone's connection drops.  

    public void Init(ushort port) //Will be run on local host or public IPv6.  IP addrss is handled in the Client.  
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4; //Where clients want to connect.  Can also do a loopback only for local hosting
        endPoint.Port = port;

        if (driver.Bind(endPoint) != 0) // Zero is success
        {
             Debug.Log("Unable to bind on port " + endPoint.Port); //Server is notr started.  
             return;
        }
        else 
        {
            driver.Listen();
             Debug.Log("Currently listening on port " + endPoint.Port);
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent); //Set max amount of connections
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
    public void OnDestroy() //We add this to make sure everything shuts down when Unity closes.  Could also do OnApplicationQuit as well 
    {
        ShutDown();
    }

    public void Update()
    {
        if (!isActive) //Break out of the update function if the server is not alive 
            return; 

        KeepAlive(); //Sends a mesage every 20 seconds 

        driver.ScheduleUpdate().Complete(); //Empties the incoming cue of messages  

        CleanUpConnections();   //Are there any existing connections for people who are no longer connected 
        AcceptNewConnections(); //Is someone trying to get in 
        UpdateMessagePump();    //Is someone sending us a message?  Do we have to reply?  
    }
    private void KeepAlive()
    {
        if (Time.deltaTime - lastKeepAlive > keepAliveTickRate) //Reset the 20 second timer every 20 seconds. 
        {
            lastKeepAlive = Time.time; 
            Broadcast(new NetKeepAlive());                      //Broadcast the keep alive message.  
        }
    }
    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)       //If a connection has dropped it is no longer created, and therefore...
            {
                connections.RemoveAtSwapBack(i); //The connection is removed at swap back (whatever that is).
                --i;                             //Don't break the loop.  
            }
        }
    }
    private void AcceptNewConnections ()
    {
        NetworkConnection c; 

        while ((c = driver.Accept()) != default(NetworkConnection)) //As long as the max number of connections has not been reached or if they are not there then add them to the server.
        {
            connections.Add(c); 
        }
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;                     //Used for messages 
        for (int i = 0; i < connections.Length; i++) //There are two people connecting: the host and the client 
        {
            NetworkEvent.Type cmd; 
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data) //Means there is a packet that the server is receiving from some connection.  
                {
                    NetUtility.OnData(stream, connections[i], this); //The data stream, whomever sent it, and the server (this).  
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionsDropped?.Invoke();
                    ShutDown(); //Called if someone disconects.  This is not something you normally do but since this is a two player server this makes sense.  Although maybe it's better to keep the server up just in case.  
                }
            }
        }
    }

    //SERVER SPECIFIC 
    public void SendToClient(NetworkConnection connection, NetMessage msg) //Send a message to a specific person.  
    {
        DataStreamWriter writer;                                           //This is a box.  
        driver.BeginSend(connection, out writer);                          //When we have a box we put the address on top of it so we know where to send it.  
        msg.Serialize(ref writer);                                         //We put stuff inside the box.  
        driver.EndSend(writer);                                            //We deliver the box.  
    }
    public void Broadcast(NetMessage msg)            //Send a message to every person in the server.  
    {
        for (int i = 0; i < connections.Length; i++) //Get a list of everyone.  
        {
            if (connections[i].IsCreated)            //Make sure they have a connection.  
            {
                Debug.Log($"Sending {msg.Code} to : {connections[i].InternalId}");
                SendToClient(connections[i], msg);   //Send the message to everyone.  
            }
        }
    }
}

//"80" is the port to listen to the HTML protocol 
