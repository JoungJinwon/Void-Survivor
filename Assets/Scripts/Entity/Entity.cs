using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Stats")]
    [SerializeField] protected int health;
    [SerializeField] protected int damage;

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Attack(Entity target)
    {
        target.TakeDamage(damage);
    }

    protected virtual void Die()
    {
        // Handle character death (e.g., play animation, disable character)
        gameObject.SetActive(false);
    }
}