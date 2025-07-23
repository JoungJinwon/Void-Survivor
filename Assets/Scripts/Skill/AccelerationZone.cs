using UnityEngine;
using System.Collections;

public class AccelerationZone : MonoBehaviour
{
    private bool isActivating = false;
    private bool isDestroying = false; // 파괴 프로세스 진행 중인지 확인

    private float attackSpeedBoost;
    private float moveSpeedBoost;

    private float zoneRadius;
    private float zoneDuration;

    private Animator animator;

    public void Init(float radius, float duration, float attackSpeed, float moveSpeed)
    {
        zoneRadius = radius;
        zoneDuration = duration;
        attackSpeedBoost = attackSpeed;
        moveSpeedBoost = moveSpeed;

        // 존의 크기 설정
        transform.localScale = new Vector3(zoneRadius, 0.1f, zoneRadius);
        
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused || isDestroying) return;

        zoneDuration -= Time.deltaTime;
        if (zoneDuration <= 0)
        {
            isDestroying = true;
            
            animator.SetTrigger("Disappear");
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // 한 프레임 대기하여 애니메이션 전환이 시작되도록 함
        yield return null;
        
        // Disappear 애니메이션 상태로 전환될 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("AccelZone_Disappear"))
        {
            yield return null;
        }
        
        // 애니메이션 재생 완료까지 대기
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log($"{animationLength}초 후 가속 영역 제거");
        
        yield return new WaitForSeconds(animationLength);

        if (isActivating)
            GameManager.Instance._Player.Decelerate(attackSpeedBoost, moveSpeedBoost);
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                isActivating = true;
                player.Accelerate(attackSpeedBoost, moveSpeedBoost);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.IsAlive)
            {
                isActivating = false;
                player.Decelerate(attackSpeedBoost, moveSpeedBoost);
            }
        }
    }
}
