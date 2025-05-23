using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera _mainCamera;
    private Player _player;
    private Rigidbody _rigid;
    private Animator _animator;

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashPower;
    [SerializeField] private float _accelerationRate;
    public float MoveSpeed { get; private set; }
    private float _curSpeed;
    public float SpeedBonus { get; set; }
    public float MaxSpeed => MoveSpeed + SpeedBonus;
    private Vector2 _moveInput;
    private Vector3 _moveVector;
    public bool IsMoving { get; private set; }
    [SerializeField] private float _dashStamina;
    [SerializeField] private float _dashStaminaDecreaseRate;
    public bool IsDashing { get; private set; }

    [SerializeField] private float _jumpPower;
    public float DefaultJumpPower => _jumpPower;
    public float JumpPower => DefaultJumpPower + JumpPowerBonus;
    public float JumpPowerBonus { get; set; }
    public bool IsJumping { get; private set; }

    [SerializeField] private LayerMask _groundLayerMask;
    private bool _wasGroundedPrevFrame;

    [SerializeField] private float _smoothness;

    private bool _isLockedCameraRot;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _player = GetComponent<Player>();
        _rigid = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MoveSpeed = _walkSpeed;
    }

    private void Update()
    {
        if (_player.Condition.UICondition.Health.CurValue <= 0f)
        {
            Singleton<GameManager>.Instance().OnResetEvent();
            _player.Condition.UICondition.Health.Set(_player.Condition.UICondition.Health.MaxValue);
            return;
        }

        _animator.SetFloat("MoveSpeed", _curSpeed);

        _animator.SetBool("IsJumping", IsJumping);
        _animator.SetBool("IsFalling", !_wasGroundedPrevFrame && !IsGrounded());

        if (IsDashing)
        {
            if (_player.Condition.UICondition.Stamina.CurValue <= 0f)
            {
                StopDash();
            }
            else
            {
                _player.Condition.UICondition.Stamina.Subtract(_dashStaminaDecreaseRate * Time.deltaTime);
            }
        }

        if (!_wasGroundedPrevFrame && IsGrounded()) // 땅에 닿는 순간
        {
            if (IsJumping)
            {
                IsJumping = false;
            }
        }

        _wasGroundedPrevFrame = IsGrounded();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _moveInput = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                _moveInput = Vector2.zero;
                break;
        }

        IsMoving = _moveInput.magnitude != 0f;
    }

    private void Move()
    {
        Vector3 lookForward = new Vector3(_mainCamera.transform.forward.x, 0f, _mainCamera.transform.forward.z).normalized;
        Vector3 lookRight = new Vector3(_mainCamera.transform.right.x, 0f, _mainCamera.transform.right.z).normalized;
        Vector3 moveDir = (lookForward * _moveInput.y + lookRight * _moveInput.x).normalized;

        if (IsMoving)
        {
            if (IsDashing) // 대시
            {
                _curSpeed = Mathf.Min(_curSpeed + (2f * _accelerationRate * Time.deltaTime), MaxSpeed);
            }
            else if (_curSpeed > _walkSpeed) // 대시 후 대시 중지
            {
                _curSpeed = Mathf.Max(_curSpeed - (_accelerationRate * Time.deltaTime), MaxSpeed);
            }
            else // 걷는 중
            {
                _curSpeed = Mathf.Min(_curSpeed + (_accelerationRate * Time.deltaTime), MaxSpeed);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), _smoothness * Time.deltaTime);
        }
        else
        {
            _curSpeed = Mathf.Max(_curSpeed - ((IsDashing ? 5f : 1f) * _accelerationRate * Time.deltaTime), 0f);
        }

        _moveVector.x = moveDir.x * _curSpeed;
        _moveVector.z = moveDir.z * _curSpeed;
        _moveVector.y = _rigid.velocity.y;

        //_rigid.velocity = _moveVector;
        _rigid.MovePosition(_rigid.position + _moveVector * Time.deltaTime);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                StartDash();
                break;
            case InputActionPhase.Canceled:
                StopDash();
                break;
        }
    }

    private void StartDash()
    {
        if (!IsGrounded() || _player.Condition.UICondition.Stamina.CurValue <= _dashStamina)
        {
            return;
        }

        if (IsMoving)
        {
            IsDashing = true;
            MoveSpeed = _dashSpeed;
        }
        else
        {
            _curSpeed = _dashSpeed;
        }

        _player.Condition.UICondition.Stamina.Subtract(_dashStamina);
        _rigid.AddForce(transform.forward * _dashPower, ForceMode.Impulse);
    }

    private void StopDash()
    {
        IsDashing = false;
        MoveSpeed = _walkSpeed;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _isLockedCameraRot = true;
                break;
            case InputActionPhase.Canceled:
                _isLockedCameraRot = false;
                break;
        }
    }

    private void Look()
    {
        if (_isLockedCameraRot || IsMoving)
        {
            return;
        }

        Vector3 dir = Vector3.Scale(_mainCamera.transform.forward, new Vector3(1f, 0f, 1f));
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _smoothness * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _wasGroundedPrevFrame && IsGrounded())
        {
            Jump(JumpPower);
        }
    }

    public void Jump(float power)
    {
        IsJumping = true;
        _rigid.AddForce(Vector3.up * power, ForceMode.Impulse);
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        _rigid.AddForce(force, forceMode);
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, _groundLayerMask.value))
            {
                return true;
            }
        }

        return false;
    }
}
