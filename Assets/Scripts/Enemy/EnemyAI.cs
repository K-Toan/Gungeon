using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    private GameObject player;
    private LayerMask playerLayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Keep the across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate 
            Destroy(gameObject);
        }

        player = GameManager.Instance.Player;
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public bool CheckLineOfSight(float range)
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, range))
        {
            if (hit.transform == player)
                return true;
        }
        return false;
    }
}