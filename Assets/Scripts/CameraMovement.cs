using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraMovement : MonoBehaviour
{
    private Player _player;
    private Transform _objectToFollow;
    private Transform _mainCamera;

    [SerializeField] private float _followSpeed;

    [SerializeField] private float _sensitivity;

    [SerializeField] private float _minClampAngleFPV;
    [SerializeField] private float _maxClampAngleFPV;
    [SerializeField] private float _minClampAngleTPV;
    [SerializeField] private float _maxClampAngleTPV;
    private float _minClampAngle;
    private float _maxClampAngle;

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;
    private float _finalDistance;

    [SerializeField] private LayerMask _ignoreLayerMask;

    [SerializeField] private float _smoothness;

    private float _rotationX;
    private float _rotationY;

    [SerializeField] private Vector3 _nomalizedPosFPV;
    [SerializeField] private Vector3 _nomalizedPosTPV;
    private Vector3 _nomalizedPos;
    private Vector3 _finalPos;
    private Vector2 _inputDir;

    private bool _isThirdPersonView;

    private void Start()
    {
        _player = Singleton<CharacterManager>.Instance().Player;
        _objectToFollow = _player.transform;
        _mainCamera = Camera.main.transform;

        transform.SetPositionAndRotation(_objectToFollow.position, _objectToFollow.rotation);

        _rotationX = transform.localRotation.eulerAngles.x;
        _rotationY = transform.localRotation.eulerAngles.y;

        _finalDistance = _maxDistance;

        _player.CharacterModel.SetActive(_isThirdPersonView);
    }

    private void Update()
    {
        _minClampAngle = _isThirdPersonView ? _minClampAngleTPV : _minClampAngleFPV;
        _maxClampAngle = _isThirdPersonView ? _maxClampAngleTPV : _maxClampAngleFPV;
        _nomalizedPos = _isThirdPersonView ? _nomalizedPosTPV : _nomalizedPosFPV;
    }

    private void LateUpdate()
    {
        Look();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _inputDir = context.ReadValue<Vector2>();
    }

    private void Look()
    {
        _rotationX += -_inputDir.y * _sensitivity * Time.deltaTime;
        _rotationY += _inputDir.x * _sensitivity * Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, _minClampAngle, _maxClampAngle);

        Quaternion rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _smoothness * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, _objectToFollow.position, _followSpeed * Time.deltaTime);

        _finalPos = transform.TransformPoint(_nomalizedPos * _maxDistance);

        int layerMask = _ignoreLayerMask.value;
        if (Physics.Linecast(transform.position, _finalPos, out RaycastHit rayHit, ~layerMask))
        {
            _finalDistance = Mathf.Clamp(rayHit.distance, _minDistance, _maxDistance);
        }
        else
        {
            _finalDistance = _maxDistance;
        }

        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _nomalizedPos * _finalDistance, _smoothness * Time.deltaTime);
    }

    public void OnChangedView(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isThirdPersonView = !_isThirdPersonView;
            _player.CharacterModel.SetActive(_isThirdPersonView);
        }
    }
}
