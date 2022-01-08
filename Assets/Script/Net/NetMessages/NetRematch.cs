using Unity.Networking.Transport; 

public class NetRematch : NetMessage
{
    public int teamID; 
    public byte wantRematch; 

    public NetRematch() //Making the box 
    {
        Code = OpCode.REMATCH; 
    }
    public NetRematch(DataStreamReader reader) //Receiving the box 
    {
        Code = OpCode.REMATCH; 
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamID);
        writer.WriteByte(wantRematch);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}
