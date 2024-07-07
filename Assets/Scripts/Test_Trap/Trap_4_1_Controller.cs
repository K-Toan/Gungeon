using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_4_1_Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject trap4_2;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float activationDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Đảm bảo Trap_4_2 bị vô hiệu hóa ban đầu
        if (trap4_2 != null)
        {
            trap4_2.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (trap4_2 != null && player != null)
        {
            // Kiểm tra khoảng cách giữa Trap_4_1 và Player
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < activationDistance)
            {
                trap4_2.SetActive(true);
            }
            else
            {
                trap4_2.SetActive(false);
            }
        }
    }
}

