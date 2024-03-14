using UnityEngine;
using UnityEngine.InputSystem;



//***NOTE: ***


[RequireComponent(typeof(CharacterController))]
public class PlayerController : Subject
{
#region Private Fields
    PlayerControl _inputs;
    Vector2 _move;
    Camera _camera;
    Vector3 _camForward, _camRight;
#endregion

#region Serialize Fields

    [SerializeField] float _speed;

    [Header("Joystick")]
    [SerializeField] Joystick _joystick;

    [Header("Character Controller")]
    [SerializeField] CharacterController _controller;

    [Header("Movement")]
    [SerializeField] float _gravity = -30.0f;
    [SerializeField] float _jumpHeight = 3.0f;
    [SerializeField] Vector3 _velocity;

    [Header("Ground Detection")]
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundCheckRadius = 0.5f;

    [SerializeField] LayerMask _groundMask;
    [SerializeField] bool _isGrounded;
    [Header("Respawn Transform")]
    [SerializeField] Transform _respawnPoint;
#endregion

    void Awake()
    {
        _camera = Camera.main;
        _controller = GetComponent<CharacterController>();
        _inputs = new PlayerControl();
        _inputs.Player.Move.performed += context => _move = context.ReadValue<Vector2>();
        _inputs.Player.Move.canceled += context => _move = Vector2.zero;
        _inputs.Player.Jump.performed += context => Jump();
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

        void OnDisable() => _inputs.Disable();

    void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);

        if(_isGrounded == true && _velocity.y < 0.0f)
        {
            _velocity.y = -2.0f;
        }

        _move = _joystick.Direction;
        _camForward = _camera.transform.forward;
        _camRight = _camera.transform.right;
        _camForward.y = 0.0f;
        _camForward.y = 0.0f;
        _camForward.Normalize();
        _camRight.Normalize();

        Vector3 movement = (_camRight * _move.x + _camForward * _move.y) * _speed * Time.fixedDeltaTime; 
        if (!_controller.enabled) { return; }
        _controller.Move(movement);
        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }



    void Jump()
    {
        if (_isGrounded) 
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
            NotifyObservers(PlayerEnums.Jump);
        }
    }

    //void DebugMessage(InputAction.CallbackContext context)
    //{
    //    Debug.Log($"Move Performed {context.ReadValue<Vector2>().x}, {context.ReadValue<Vector2>().y}");
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("deathZone"))
        {
            MovePlayerPosition(_respawnPoint.position);
            NotifyObservers(PlayerEnums.Died);
        }
    }

    public void MovePlayerPosition(Vector3 position)
    {
        _controller.enabled = false;
        //sets respawn positon to the respawnPoint position.
        transform.position = position;
        _controller.enabled = true;
    }
}
