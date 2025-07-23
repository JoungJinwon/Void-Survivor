using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private bool isExploding = false;

    private const float explosionDelay = 5f;
    public const float explosionDuration = 1f;
    private float mineTimer;

    private AudioSource _AudioSource;

    public float mineDamage;
    public float explosionRadius;

    [Header("SFX")]
    public AudioClip mineTickSound;
    public AudioClip explosionSound;

    [Header("VFX")]
    public GameObject explosionEffect;

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;

        if (mineTimer > explosionDelay && !isExploding)
            StartCoroutine(Explode());
        else
            mineTimer += Time.deltaTime;
    }

    public void InitMine(float damage, float radius)
    {
        mineTimer = 0f;

        mineDamage = damage;
        explosionRadius = radius;

        _AudioSource = GetComponent<AudioSource>();
        if (mineTickSound != null)
        {
            _AudioSource.clip = mineTickSound;
            _AudioSource.loop = true;
            _AudioSource.Play();
        }
    }

    private IEnumerator Explode()
    {
        isExploding = true;
        Debug.Log("지뢰 폭발!");

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // 폭발 시 렌더러 비활성화
        }

        // 폭발 시각 효과 시작
        PlayExplosionEffect();

        List<Enemy> attackedEnimies = new List<Enemy>();
        float explosionTimer = explosionDuration;

        while (explosionTimer > 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out Enemy enemy))
                {
                    if (attackedEnimies.Contains(enemy))
                        continue; // 이미 공격한 적은 제외

                    enemy.TakeDamage(mineDamage);
                    attackedEnimies.Add(enemy);
                }
            }

            explosionTimer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    
    private void PlayExplosionEffect()
    {
        // 사운드 효과
        if (explosionSound != null)
        {
            _AudioSource.clip = explosionSound;
            _AudioSource.loop = false;
            _AudioSource.Play();
        }

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, explosionDuration); // 폭발 지속시간 후 제거
        }
    }

    private void OnDrawGizmos()
    {
        // 폭발 반경 시각화
        if (isExploding)
        {
            Gizmos.color = Color.red; // 폭발 중일 때 빨간색
        }
        else
        {
            Gizmos.color = Color.yellow; // 대기 중일 때 노란색
        }
        
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}