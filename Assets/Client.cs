using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{

    BinaryFormatter formatter = new BinaryFormatter();

    UdpClient client;

    IPEndPoint epServer;

    public Message received = null;

    public ServerStartMessage startMessage = null;

    // Start is called before the first frame update
    void Awake()
    {
        string ipAddress = GameManager.Instance.IP;

        client = new UdpClient();
        epServer = new IPEndPoint(IPAddress.Parse(ipAddress), GameManager.PORT);
        client.Connect(epServer);

        Thread thread = new Thread(new ThreadStart(ReceiveData));
        thread.Start();
    }

    public void SendMessage(Message message)
    {
        byte[] clientMessageAsByteArray = new byte[GameManager.PACKET_LENGTH];

        MemoryStream ms = new MemoryStream(clientMessageAsByteArray);

        formatter.Serialize(ms, message);

        client.Send(clientMessageAsByteArray, clientMessageAsByteArray.Length);
    }

    private void ReceiveData()
    {
        while (true)
        {
            byte[] receivedData = client.Receive(ref epServer);

            MemoryStream ms = new MemoryStream(receivedData);

            received = formatter.Deserialize(ms) as Message;

            if (received.type.Equals(MessageType.SERVER_START))
            {
                startMessage = received as ServerStartMessage;
            }
        }
    }


}
