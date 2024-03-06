using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private DoorControllerScript DoorController;

    [SerializeField] private Animator DoorAnimator;

    private List<Collider2D> ListCollision = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Bot"))
        {
            if (ListCollision.Count == 0)
                ListCollision.Add(Collision);
            else
            {
                foreach (Collider2D _collision in ListCollision)
                    if (_collision == Collision) return;
                ListCollision.Add(Collision);
            }
            DoorController.OpenDoor(DoorAnimator);
        }
    }

    private void OnTriggerExit2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Bot"))
        {
            if (ListCollision.Count == 1)
                ListCollision.Remove(Collision);
            else
            {
                foreach (Collider2D _collision in ListCollision)
                    if (_collision == Collision)
                    {
                        ListCollision.Remove(Collision);
                        return;
                    }
            }
            DoorController.CloseDoor(DoorAnimator);
        }
    }
}
