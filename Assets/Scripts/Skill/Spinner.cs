using UnityEngine;

public class Spinner : MonoBehaviour
{
    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        // Enemy 태그가 붙은 오브젝트와 충돌 시 데미지 적용
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (enemy.IsAlive)
                enemy.TakeDamage(damage);
        }
    }
}