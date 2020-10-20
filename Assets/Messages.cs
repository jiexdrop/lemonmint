using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum MessageType
{
    CLIENT_START,
    SERVER_START,

    CLIENT_MOVE,
    SERVER_MOVE,
}

[Serializable]
public class Message
{
    public MessageType type;
}

[Serializable]
public class ClientStartMessage : Message
{
    public ClientStartMessage()
    {
        type = MessageType.CLIENT_START;
    }
}

[Serializable]
public class ServerStartMessage : Message
{

    public int id;
    public float[] x;
    public float[] y;
    public ServerStartMessage()
    {
        type = MessageType.SERVER_START;
    }
}


[Serializable]
public class ClientMoveMessage : Message
{
    public float x;
    public float y;
    public int id;

    public ClientMoveMessage()
    {
        type = MessageType.CLIENT_MOVE;
    }
}

[Serializable]
public class ServerMoveMessage : Message
{
    public float[] x;
    public float[] y;

    public ServerMoveMessage()
    {
        type = MessageType.SERVER_MOVE;
    }
}