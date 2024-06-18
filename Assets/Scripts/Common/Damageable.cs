using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public int HealthPoint = 100;

    private bool _hasAnimator;
    private Animator _animator;

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
    }

    public virtual void TakeDamage(int damage)
    {
        HealthPoint -= damage;
        if (HealthPoint > 0)
        {
            if(_hasAnimator)
                _animator.SetTrigger("Hit");
        }
        else
        {
            if(_hasAnimator)
                _animator.SetTrigger("Die");
            // Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
