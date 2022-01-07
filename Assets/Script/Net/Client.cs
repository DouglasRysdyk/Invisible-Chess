using System; 
using Unity.Collections;
using Unity.Networking.Transport; 
using UnityEngine;

public class Client : MonoBehaviour
{
    #region "Singleton implementation" 
    public static Client Instance { set; get; }

    private void Awake()
    {
        Instance = this; 
    }
    #endregion

    public NetworkDriver driver;          //I don't know what the network driver is or does (01-06-2022 @11:42)
    private NetworkConnection connection; //A list of all connections
    private bool isActive = false;        //Is the server active? 
    public Action connectionsDropped;     //Use if someone's connection drops. 

    public void Init(string ip, ushort port) //Now takes an IP address.  
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port); //Where clients want to connect.  Can also do a loopback only for local hosting

        connection = driver.Connect(endPoint);

        Debug.Log("Attempting to connect to server on " + endPoint.Address);

        isActive = true; 

        RegisterToEvent(); //Keeps track of keep alive messages 
    }
    public void ShutDown()
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            isActive = false; 
            connection = default(NetworkConnection); //Reset the connection.  May not be necessary.  
        }
    }
    public void OnDestroy()
    {
        ShutDown();
    }

    public void Update()
    {
        if (!isActive) //Break out of the update function if the server is not alive 
            return; 

        driver.ScheduleUpdate().Complete(); //Empties the incoming cue of messages  
        CheckAlive();                       //If the server is down we don't know that, hence why we need to check 

        UpdateMessagePump(); 
    }
    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong, lost connection to the server.");
            connectionsDropped?.Invoke();
            ShutDown();
        }
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream; //Used for messages 
        NetworkEvent.Type cmd; 
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                /* 
                //How to send data to server from the client like what team you want to be on.  
                var s = new NetWelcome();
                s.AssignedTeam = 5; 
                SendToServer(s);
                //For this chess game the value doesn't matter.  
                */
                SendToServer(new NetWelcome());
                Debug.Log("We're connected yay!");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection)); 
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from the server.");
                connection = default(NetworkConnection);
                connectionsDropped?.Invoke(); //The ? is an optional.  It checks if the value is null.  
                ShutDown();
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;                  //Create the box.  
        driver.BeginSend(connection, out writer); //Write the addres 
        msg.Serialize(ref writer);                //Fill the box with content we need to send.  
        driver.EndSend(writer);                   //Deliver the box.  
    }

    //EVENT PARSING
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive; 
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive; 
    }

    private void OnKeepAlive(NetMessage nm)
    {
        //Send it back to keep both sides alive.
        SendToServer(nm);
    }
}
