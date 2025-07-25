using UnityEngine;

public class TransformChanger : MonoBehaviour
{
    private float timer = 0f;

    public enum ChangeType
    {
        Simple,
        Float
    }

    public ChangeType changeType = ChangeType.Simple;

    [Header("Simple Transform Change")]
    [SerializeField] private Vector3 positionChangeOffset; // 새 위치
    [SerializeField] private Vector3 rotationChangeOffset; // 새 회전값

    [Header("Float Transform Change")]
    [SerializeField] private float floatOffset;
    [SerializeField] private float floatSpeed = 1f;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (changeType == ChangeType.Simple)
        {
            ChangePosition(positionChangeOffset * Time.fixedDeltaTime);
            ChangeRotation(rotationChangeOffset * Time.fixedDeltaTime);
        }
        else if (changeType == ChangeType.Float)
        {
            FloatPosition(floatOffset * Time.fixedDeltaTime);
        }
    }

    private void ChangePosition(Vector3 offset)
    {
        transform.position += offset;
    }

    private void ChangeRotation(Vector3 offset)
    {
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x + offset.x,
            transform.rotation.eulerAngles.y + offset.y,
            transform.rotation.eulerAngles.z + offset.z
        );
    }

    private void FloatPosition(float offset)
    {
        float newY = Mathf.Sin(timer * floatSpeed) * offset;
        transform.position += new Vector3(0, newY, 0);
    }
}
