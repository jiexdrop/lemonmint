using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    public bool isControlled = false;

    public int id = -1;

    public Server server;

    public Client client;

    public Vector3 movement;

    public VirtualJoystick virtualJoystick;

    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3();
    }



    // Update is called once per frame
    void Update()
    {
        if (isControlled) // is server
        {
            Vector2 playerInput;
            playerInput.x = GameManager.NotNull(Input.GetAxis("Horizontal"), virtualJoystick.InputVector.x);
            playerInput.y = GameManager.NotNull(Input.GetAxis("Vertical"), virtualJoystick.InputVector.y);
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);
            Vector3 velocity = new Vector3(playerInput.x, playerInput.y);
            Vector3 displacement = velocity * speed * Time.deltaTime;
            transform.localPosition += displacement;

            if(client != null)
            {
                ClientMoveMessage message = new ClientMoveMessage();
                message.id = id;
                message.x = transform.localPosition.x;
                message.y = transform.localPosition.y;
                client.SendMessage(message);
            }
        } else // is client
        {
            //if (client.received.type.Equals(MessageType.MOVE))
            //{
            //    MoveMessage moveMessage = client.received as MoveMessage;
            //    movement.x = moveMessage.x;
            //    movement.y = moveMessage.y;
            //    transform.localPosition = movement;
            //}
        }
    }
}
