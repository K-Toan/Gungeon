using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_5_Controller : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float attackRange = 5f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player_Test_1").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            animator.SetBool("isActive", true);
        }
        else
        {
            animator.SetBool("isActive", false);
        }
    }
}
