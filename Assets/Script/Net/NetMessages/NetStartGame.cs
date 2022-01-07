using Unity.Networking.Transport; 

public class NetStartGame : NetMessage
{
    public NetStartGame() //Making the box 
    {
        Code = OpCode.START_GAME; 
    }
    public NetStartGame(DataStreamReader reader) //Receiving the box 
    {
        Code = OpCode.START_GAME; 
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        //
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_START_GAME?.Invoke(this); //The "?" standing for "is anyone listening" helps me understand optionals a bit more.  
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}
