using UnityEngine;
using System.Collections;

public class CameraCutsceneTriggerLerp : MonoBehaviour
{
    public Transform playerCamera;    // Drag player camera transform
    public Transform cutsceneCamera;  // Drag cutscene camera transform (position & rotation only)
    public float transitionSpeed = 2f; // How fast to lerp
    public float cutsceneTime = 5f;    // How long to stay in cutscene

    private bool inCutscene = false;
    private Transform camTransform;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private void Start()
    {
        camTransform = playerCamera; // We only move the player camera
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !inCutscene)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        inCutscene = true;

        // Save original position/rotation
        originalPos = camTransform.position;
        originalRot = camTransform.rotation;

        // Smoothly move to cutscene cam
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            camTransform.position = Vector3.Lerp(originalPos, cutsceneCamera.position, t);
            camTransform.rotation = Quaternion.Slerp(originalRot, cutsceneCamera.rotation, t);
            yield return null;
        }

        // Stay at cutscene for X seconds
        yield return new WaitForSeconds(cutsceneTime);

        // Smoothly move back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            camTransform.position = Vector3.Lerp(cutsceneCamera.position, originalPos, t);
            camTransform.rotation = Quaternion.Slerp(cutsceneCamera.rotation, originalRot, t);
            yield return null;
        }

        inCutscene = false;
    }
}
