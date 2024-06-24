using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : DetectionZone
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(detectedObjs.Count > 0)
        {
            animator.SetBool("OpenDoor", true);
        }
        else
        {
            animator.SetBool("OpenDoor", false);
        }
    }
}
