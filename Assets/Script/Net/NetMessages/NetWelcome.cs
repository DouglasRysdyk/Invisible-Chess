using Unity.Networking.Transport; 
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int AssignedTeam { set; get; } //New stuff we're putting in the box.  Note it's an int.  
                                          //The server does not need to know what your assigned team is.  The server will handle that.  The client will not handle that.  

    public NetWelcome() //Making the box 
    {
        Code = OpCode.WELCOME; 
    }
    public NetWelcome(DataStreamReader reader) //Receiving the box 
    {
        Code = OpCode.WELCOME; 
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssignedTeam); //Normal data type declaration rules apply.  If you put an int in...  
    }
    public override void Deserialize(DataStreamReader reader)
    {
        AssignedTeam = reader.ReadInt(); //...you need to get an int out.  Otherwise the data will be read incorrectly and there will be corruption issues.  
                                         //The byte was already read in the NetUtility::OnData
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this); //The "?" standing for "is anyone listening" helps me understand optionals a bit more.  
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
