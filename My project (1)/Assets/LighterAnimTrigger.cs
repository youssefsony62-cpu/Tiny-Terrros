using UnityEngine;

public class LighterAnimTrigger : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("PlayLight");
        }
    }
}
