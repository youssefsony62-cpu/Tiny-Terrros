using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class HDRPBloomController : MonoBehaviour
{
    public Volume volume;
    private Bloom bloom;
    private bool inZone;

    void Start()
    {
        if (!volume.profile.TryGet(out bloom))
        {
            Debug.LogError("No Bloom found in this HDRP Volume Profile!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) inZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) inZone = false;
    }

    void Update()
    {
        if (bloom != null)
        {
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value,
                                               inZone ? 2.0f : 0.5f,
                                               Time.deltaTime * 2f);
        }
    }
}
