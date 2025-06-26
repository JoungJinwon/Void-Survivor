using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;

    private Vector3 moveInput;
    private Vector3 currentVelocity;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;


    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
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
        Vector3 targetVelocity = moveInput * moveSpeed;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            (moveInput.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        currentVelocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = currentVelocity;
    }
}
