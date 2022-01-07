using System;
using UnityEngine;
using Unity.Networking.Transport;

public enum OpCode //Could be shrunk down to only 1-3 messages (ideally one), but we're not doing that for the sake of clarity. The smallest amount of information you can write into this would be a byte.   We have 0-255 slots to use.  If we were going over to like 257 then we'd try and shrink the thing down.  The smaller the messages the better in the long run.  
{
    KEEP_ALIVE = 1, 
    WELCOME = 2, 
    START_GAME = 3,
    MAKE_MOVE = 4,
    REMATCH = 5
}

public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null) //This happens when you receive the box.  
    {
        NetMessage msg = null;  //This is the content in the box.  
        var opCode = (OpCode)stream.ReadByte(); //Take a peak inside the box.  This identifies what's in the box.  
        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
            case OpCode.WELCOME: msg = new NetWelcome(stream); break; //Without this we would not know how to decode the operation code.  This calls NetWelcome which tells it how to read it.  
            case OpCode.START_GAME: msg = new NetStartGame(stream); break;
            // case OpCode.MAKE_MOVE: msg = new NetMakeMove(stream); break;
            // case OpCode.REMATCH: msg = new NetRematch(stream); break;
            default:
                Debug.LogError("Message received had no OpCode."); //We don't know what's in the box so we throw it out.  This blows everything up.  
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else 
            msg.ReceivedOnClient();
    }

    //NET MESSAGES
    public static Action<NetMessage> C_KEEP_ALIVE; //When we receive keep alive message on the client side.  
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE; //When we receive keep alive message on the server side.  Note we record whatever client sent the message. 
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;
}
