using UnityEngine;

public class DoorControllerScript : MonoBehaviour
{
    public void OpenDoor(Animator DoorAnimator)
    {
        DoorAnimator.SetBool("IsOpening", true);
    }

    public void CloseDoor(Animator DoorAnimator)
    {
        DoorAnimator.SetBool("IsOpening", false);
    }
}
