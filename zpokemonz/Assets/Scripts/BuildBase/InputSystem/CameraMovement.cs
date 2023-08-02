using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform _trans;
    [Range(-3f, -0.5f)]
    [SerializeField] float moveSpeed = -0.5f;
    [SerializeField] float left;
    [SerializeField] float right;
    [SerializeField] float top;
    [SerializeField] float bottom;
    public float X()
    {
        return Mathf.Clamp(_trans.position.x + moveSpeed * Input.GetAxis("Mouse X"), left, right);
    }
    public float Y()
    {
        return Mathf.Clamp(_trans.position.y + moveSpeed * Input.GetAxis("Mouse Y"), bottom, top);
    }
    public void MoveCamera()
    {
        _trans.position = new Vector3(X(), Y(), -5);
    }
}