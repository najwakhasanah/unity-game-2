using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    
    private PlayerAnimations playerAnimations;

    private void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Update()
    {
        if (stats.Health <= 0f)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(float amount)
    {
        if (stats.Health <= 0f) return;
        stats.Health -= amount;
        DamageManager.Instance.ShowDamageText(amount, transform);
        if (stats.Health <= 0f)
        {
            stats.Health = 0f;
            PlayerDead();
        }
    }
    
    private void PlayerDead()
    {
        playerAnimations.SetDeadAnimation();
    }
}
