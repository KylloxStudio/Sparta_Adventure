using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float _checkRate;
    private float _lastCheckTime;
    [SerializeField] private float _maxCheckDistacne;
    [SerializeField] private LayerMask _layerMask;

    private GameObject _curInteractableObject;
    private IInteractable _curInteractable;

    [SerializeField] private GameObject _promptTextObject;
    private TextMeshProUGUI _promptText;
    private Camera _mainCamera;
    private CameraMovement _cameraMovement;

    private void Start()
    {
        _mainCamera = Camera.main;
        _cameraMovement = _mainCamera.GetComponentInParent<CameraMovement>();
        _promptText = _promptTextObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Time.time - _lastCheckTime <= _checkRate)
        {
            return;
        }

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, _maxCheckDistacne * (_cameraMovement.IsThirdPersonView ? 2.5f : 1f), _layerMask.value))
        {
            if (hit.collider.gameObject != _curInteractableObject)
            {
                _curInteractableObject = hit.collider.gameObject;
                _curInteractable = hit.collider.GetComponent<IInteractable>();
                SetPromptText(true);
            }
        }
        else
        {
            _curInteractableObject = null;
            _curInteractable = null;
            SetPromptText(false);
        }

        _lastCheckTime = Time.time;
    }

    private void SetPromptText(bool active)
    {
        if (active)
        {
            _promptText.text = _curInteractable.GetInteractPrompt();
            _promptTextObject.SetActive(true);
        }
        else
        {
            _promptText.text = string.Empty;
            _promptTextObject.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _curInteractable != null)
        {
            _curInteractable.OnInteract();
            _curInteractableObject = null;
            _curInteractable = null;
            SetPromptText(false);
        }
    }
}
