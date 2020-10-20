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

public class Server : MonoBehaviour
{

    BinaryFormatter formatter = new BinaryFormatter();

    UdpClient server;

    public Message received = null;

    private List<IPEndPoint> epClients = new List<IPEndPoint>();

    void Awake()
    {
        server = new UdpClient(GameManager.PORT);

        Thread thread = new Thread(new ThreadStart(ReceiveData));
        thread.Start();
    }

    private void ReceiveData()
    {
        while (true)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, GameManager.PORT);

            byte[] receivedData = server.Receive(ref remoteEP);

            MemoryStream ms = new MemoryStream(receivedData);

            received = formatter.Deserialize(ms) as Message;

            if (!epClients.Contains(remoteEP))
            {
                epClients.Add(remoteEP);
            }

        }
    }

    public void SendMessage(Message message)
    {
        foreach (IPEndPoint endPoint in epClients)
        {

            byte[] serverMessageAsByteArray = new byte[GameManager.PACKET_LENGTH];

            MemoryStream ms = new MemoryStream(serverMessageAsByteArray);

            formatter.Serialize(ms, message);

            server.Send(serverMessageAsByteArray, serverMessageAsByteArray.Length, endPoint);

        }
    }

}
