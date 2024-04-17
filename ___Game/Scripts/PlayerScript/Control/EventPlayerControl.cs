using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventPlayerControl : MonoBehaviour
{
    // ��� ������� ���������� ������������� input system � ������� ��������������� (������� ������ � ������� ��������) - ��������cz ���� "PlayerInput", �� ������� ���������� ������� (�.�. ������ �� ��������). ��� ����� ������ ������
    public static Action<Vector2> s_playerMoveDirection;

    public static Action<InputAction.CallbackContext> s_playerLeftClick;
    public static Action<InputAction.CallbackContext> s_playerRightClick;

    public static Action s_playerCloseClick;

    public void AssignmentOfDirection(InputAction.CallbackContext context) => s_playerMoveDirection?.Invoke(context.ReadValue<Vector2>());

    public void OnLeftClick(InputAction.CallbackContext context) => s_playerLeftClick?.Invoke(context);
    public void OnRightClick(InputAction.CallbackContext context) => s_playerRightClick?.Invoke(context);

    public void OnCloseClick() => s_playerCloseClick?.Invoke();
}
