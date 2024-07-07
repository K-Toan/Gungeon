using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Test_Trap_Controller : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;
    private Rigidbody2D rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 newVelocity = rigid.velocity;

        // Xử lý di chuyển ngang
        if (horizontal != 0)
        {
            newVelocity.x = horizontal * moveSpeed;
            transform.localScale = new Vector3(horizontal > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            newVelocity.x = 0;
        }

        // Xử lý di chuyển dọc
        if (vertical != 0)
        {
            newVelocity.y = vertical * moveSpeed;
        }
        else
        {
            newVelocity.y = 0;
        }


        rigid.velocity = newVelocity;
    }
}
