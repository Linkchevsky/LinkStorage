using Mirror;
using UnityEngine;

public class PlayerMovementScript : NetworkBehaviour
{
    private Vector2 Direction;

    [SerializeField] private float PlayerSpeed = 3f;


    private void Start()
    {
        if (!isLocalPlayer) return;
        transform.parent = GameObject.Find("StartShip").transform;
    }

    private void Update()
    {
        Direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.localPosition += new Vector3(Direction.x, Direction.y, 0f) * PlayerSpeed * Time.deltaTime;
    }
}
