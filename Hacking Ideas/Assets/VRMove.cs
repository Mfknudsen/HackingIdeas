using UnityEngine;

public class VRMove : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    [SerializeField] private float moveSpeed = 1, rotSpeed = 1;
    [SerializeField] private Transform dirTransform;

    private PlayerInput playerInput;
    private Vector2 moveDir = Vector2.zero, rotDir = Vector2.zero;

    private void Start()
    {
        this.playerInput = new PlayerInput();
        this.playerInput.Enable();

        this.playerInput.Player.Move.performed += c => this.moveDir = c.ReadValue<Vector2>();
        this.playerInput.Player.Move.canceled += c => this.moveDir = c.ReadValue<Vector2>();

        this.playerInput.Player.Rot.performed += c =>
        {
            Vector2 dir = c.ReadValue<Vector2>();
            this.rotDir = new Vector2(Mathf.Clamp(dir.x, -1, 1), Mathf.Clamp(dir.y, -1, 1));
        };
        this.playerInput.Player.Rot.canceled += c => this.rotDir = new Vector2(
            Mathf.Clamp(c.ReadValue<Vector2>().x, -1, 1),
            Mathf.Clamp(c.ReadValue<Vector2>().y, -1, 1)
        );

        this.dirTransform.position = this.camTransform.position;
    }

    private void Update()
    {
        Vector3 camPos = this.camTransform.position;
        this.dirTransform.position = camPos;
        Vector3 f = this.camTransform.forward;
        Vector3 forward = new Vector3(f.x, 0, f.z);
        this.dirTransform.LookAt(camPos + forward, Vector3.up);

        this.transform.RotateAround(camPos, Vector3.up, this.rotDir.x * this.rotSpeed * Time.deltaTime);

        Vector3 move = this.dirTransform.forward * this.moveDir.y + this.dirTransform.right * this.moveDir.x;
        move.Normalize();
        this.transform.position += move * (this.moveSpeed * Time.deltaTime);
    }
}