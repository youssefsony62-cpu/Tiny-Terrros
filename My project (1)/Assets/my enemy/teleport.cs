using UnityEngine;

public class ThinManStepTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportDistance = 4f;     // how far forward to teleport
    public string teleportTrigger = "teleport"; // your Animator trigger (optional)
    public float teleportDelay = 0.1f;      // short delay before move

    private Animator anim;
    private bool canTeleport = true;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if collider is tagged for teleport
        if (canTeleport && other.CompareTag("StepTeleport"))
        {
            StartCoroutine(TeleportForward());
        }
    }

    private System.Collections.IEnumerator TeleportForward()
    {
        canTeleport = false;

        if (anim && !string.IsNullOrEmpty(teleportTrigger))
            anim.SetTrigger(teleportTrigger);

        yield return new WaitForSeconds(teleportDelay);

        transform.position += transform.forward * teleportDistance;

        yield return new WaitForSeconds(1f); // cooldown
        canTeleport = true;
    }
}
