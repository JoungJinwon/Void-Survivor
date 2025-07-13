using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;

    private Vector3 moveInput;
    private Vector3 currentVelocity;

    private Player _player;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused)
        {
            StopPlayer();
            return;
        }
        
        if (_rigidbody != null)
            Move();
    }

    // 입력을 받아 이동할 방향을 계산하는 함수
    public void OnMove(InputAction.CallbackContext context)
    {
        // 현재 입력값 받아오기
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = new Vector3(input.x, 0, input.y).normalized;
    }

    private void Move()
    {
        Vector3 targetVelocity = moveInput * defaultMoveSpeed * _player.GetPlayerMoveSpeed();

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            (moveInput.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        currentVelocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = currentVelocity;
    }

    private void StopPlayer()
    {
        currentVelocity = Vector3.zero;
        _rigidbody.linearVelocity = currentVelocity;
    }
}
