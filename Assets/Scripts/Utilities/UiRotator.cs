using UnityEngine;

public class UiRotator : MonoBehaviour
{
    private void LateUpdate()
{
    transform.forward = Camera.main.transform.forward;
}
}
