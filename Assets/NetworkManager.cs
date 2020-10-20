using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NetworkManager : MonoBehaviour
{

    public VirtualJoystick virtualJoystick;

    public GameObject playerPrefab;

    public GameObject clientPrefab;

    public GameObject serverPrefab;

    List<Player> players = new List<Player>();

    private Client client;

    private Server server;

    private State state;

    private int playerId = 0;

    private Vector3 movement = Vector3.zero;

    private enum State
    {
        START,

        MOVEMENT,
    }

    void Start()
    {
        if (GameManager.Instance.connectionType.Equals(ConnectionType.CLIENT))
        {
            client = Instantiate(clientPrefab).GetComponent<Client>();
            client.received = null;
            client.startMessage = null;
            client.SendMessage(new ClientStartMessage());
        }

        if (GameManager.Instance.connectionType.Equals(ConnectionType.SERVER))
        {
            server = Instantiate(serverPrefab).GetComponent<Server>();
            server.received = null;
            AddPlayer(server, null, 0, true);
        }
    }

    private Player AddPlayer(Server s, Client c, int id, bool isControlled = false)
    {
        Player player = Instantiate(playerPrefab).GetComponent<Player>();
        player.virtualJoystick = virtualJoystick;
        player.server = s;
        player.client = c;
        player.isControlled = isControlled;
        player.id = id;
        player.transform.position = Random.insideUnitCircle * 3;
        players.Add(player);
        return player;
    }


    void Update()
    {
        if (GameManager.Instance.connectionType.Equals(ConnectionType.SERVER))
        {
            if (server.received != null)
            {
                switch (server.received.type)
                {
                    case MessageType.CLIENT_START:
                        ServerStartMessage startMessage = new ServerStartMessage();

                        int playerId = players.Count;

                        AddPlayer(null, client, playerId);

                        startMessage.id = playerId;
                        startMessage.x = new float[players.Count];
                        startMessage.y = new float[players.Count];

                        for (int i = 0; i < players.Count; i++)
                        {
                            startMessage.x[i] = players[i].transform.position.x;
                            startMessage.y[i] = players[i].transform.position.y;
                        }
                        Debug.LogError("SEND MESSAGE TO EVERYONE");
                        server.SendMessage(startMessage);
                        break;
                    case MessageType.CLIENT_MOVE:
                        ClientMoveMessage clientMoveMessage = server.received as ClientMoveMessage;
                        movement.x = clientMoveMessage.x;
                        movement.y = clientMoveMessage.y;
                        players[clientMoveMessage.id].transform.position = movement;

                        // Share Client_Move with all the other clients
                        break;
                }

            }


            ServerMoveMessage serverMoveMessage = new ServerMoveMessage();
            serverMoveMessage.x = new float[players.Count];
            serverMoveMessage.y = new float[players.Count];
            // send movement to all clients
            for (int i = 0; i < players.Count; i++)
            {
                serverMoveMessage.x[i] = players[i].transform.position.x;
                serverMoveMessage.y[i] = players[i].transform.position.y;
            }
            server.SendMessage(serverMoveMessage);

            server.received = null;
        }

        if (GameManager.Instance.connectionType.Equals(ConnectionType.CLIENT))
        {

            switch (state)
            {
                case State.START:
                    Debug.Log("CLIENT START MESSAGE " + client.startMessage);
                    if (client.startMessage != null)
                    {
                        switch (client.startMessage.type)
                        {
                            case MessageType.SERVER_START:
                                ClientReceivePlayers(true);

                                Debug.Log("CLIENT START MESSAGE X " + client.startMessage.x.Length);
                                Debug.Log("CLIENT START MESSAGE Y " + client.startMessage.y.Length);

                                state = State.MOVEMENT;

                                break;
                        }
                    }

                    break;
                case State.MOVEMENT:
                    if (client.startMessage != null)
                    {
                        switch (client.startMessage.type)
                        {
                            case MessageType.SERVER_START:
                                ClientReceivePlayers(false);
                                break;
                        }
                    }

                    if (client.received != null)
                    {
                        switch (client.received.type)
                        {
                            case MessageType.SERVER_MOVE:
                                ServerMoveMessage serverMoveMessage = client.received as ServerMoveMessage;

                                for (int i = 0; i < serverMoveMessage.x.Length; i++)
                                {
                                    players[i].transform.position = new Vector3(serverMoveMessage.x[i], serverMoveMessage.y[i]);
                                }
                                break;
                        }
                    }
                    break;
            }

            // Remove client received message
            client.received = null;
        }

    }

    public void ClientReceivePlayers(bool controled)
    {
        ServerStartMessage serverStartMessage = client.startMessage;

        // set playerId
        if (controled)
        {
            playerId = serverStartMessage.id;
        }

        // Only add players after players.Count;
        int playersCount = players.Count;
        for (int i = playersCount; i < serverStartMessage.x.Length; i++)
        {
            Player player = AddPlayer(null, client, i);
            // Player isControlled only if startMessage id == lastPlayer
            player.isControlled = controled && (serverStartMessage.id == i);

            player.transform.position = new Vector3(serverStartMessage.x[i], serverStartMessage.y[i]);
        }
    }
}
