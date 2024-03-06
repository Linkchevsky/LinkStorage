using Mirror;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMoveControl : MonoCache
{
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _playerSpeed = 5f;

    private void Awake()
    {
        Enabled += Enable;
        Disabled += Disable;
    }

    private void Enable() => EventPlayerControl.s_playerMoveDirection += AssignmentOfDirection;
    private void Disable() => EventPlayerControl.s_playerMoveDirection -= AssignmentOfDirection;
    public void AssignmentOfDirection(Vector2 direction) => _direction = direction;


    public override void OnTick() => Move();

    private void Move() => transform.localPosition += new Vector3(_direction.x, _direction.y, 0f) * _playerSpeed * Time.deltaTime;
}
