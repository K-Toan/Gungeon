using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public float HealthPoint = 100;

    private void Start()
    {
        // _hasAnimator = TryGetComponent<Animator>(out _animator);
    }

    public virtual void TakeDamage(float damage)
    {
        HealthPoint -= damage;
        if (HealthPoint > 0)
        {
            // if(_hasAnimator)
                // _animator.SetTrigger("Hit");
        }
        else
        {
            // if(_hasAnimator)
            //     _animator.SetTrigger("Die");
            // Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
