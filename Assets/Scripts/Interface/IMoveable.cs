using UnityEngine;

public interface IMoveable
{
    public float _moveSpeed { get; set; }
    public void Move(Vector2 direction);
}
