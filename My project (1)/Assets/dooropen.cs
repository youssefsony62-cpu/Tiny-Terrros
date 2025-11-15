using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator; // drag your door animator here in Inspector
    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            doorAnimator.SetTrigger("Open");
            isOpen = true;
        }
    }
}
