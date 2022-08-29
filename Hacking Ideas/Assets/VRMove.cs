using UnityEngine;

public class VRMove : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    [SerializeField] private float moveSpeed = 1, rotSpeed = 1;

    private PlayerInput playerInput;
    private Vector2 moveDir = Vector2.zero, rotDir = Vector2.zero;
    private Transform dirTransform;

    private void Start()
    {
        this.playerInput = new PlayerInput();
        this.playerInput.Enable();

        this.playerInput.Player.Move.performed += c => moveDir = c.ReadValue<Vector2>();
        this.playerInput.Player.Move.canceled += c => moveDir = c.ReadValue<Vector2>();

        this.playerInput.Player.Rot.performed += c =>
        {
            Vector2 dir = c.ReadValue<Vector2>();
            this.rotDir = new Vector2(Mathf.Clamp(dir.x, -1, 1), Mathf.Clamp(dir.y, -1, 1));
        };
        this.playerInput.Player.Rot.canceled += c => this.rotDir = new Vector2(
            Mathf.Clamp(c.ReadValue<Vector2>().x, -1, 1),
            Mathf.Clamp(c.ReadValue<Vector2>().y, -1, 1)
        );

        dirTransform = new GameObject("Direction Transform").transform;
        dirTransform.position = camTransform.position;
    }

    private void Update()
    {
        Vector3 camPos = camTransform.position;
        dirTransform.position = camPos;
        dirTransform.LookAt(camPos + camTransform.forward, Vector3.up);

        transform.RotateAround(camPos, Vector3.up, rotDir.x * rotSpeed * Time.deltaTime);

        Vector3 move = dirTransform.forward * this.moveDir.y + dirTransform.right * this.moveDir.x;
        move.Normalize();
        this.transform.position += move * (this.moveSpeed * Time.deltaTime);
    }
}