using Unity.Networking.Transport; 

public class NetMessage 
{
    public OpCode Code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer) //Putting stuff in the box.  
    {
        writer.WriteByte((byte)Code);
    }
    public virtual void Deserialize(DataStreamReader reader) //The client has received the box and is now unpacking it, and putting everything in its proper place.  
    {
        //
    }

    public virtual void ReceivedOnClient() //There is only one server who can send us messages.  
    {
        //
    }
    public virtual void ReceivedOnServer(NetworkConnection cnn) //What client sent us the message?  This allows us to respond to them directly.  
    {
        //
    }
}
