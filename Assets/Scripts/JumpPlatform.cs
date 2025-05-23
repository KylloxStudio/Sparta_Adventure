using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float _waitingTime;
    private float _curWaitingTime;
    [SerializeField] private float _power;
    [SerializeField] private Slider _waitSlider;
    [SerializeField] private bool _isForward;

    private Coroutine _coroutine;

    private void Start()
    {
        if (_waitSlider != null)
        {
            _waitSlider.value = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_waitingTime != 0f)
            {
                _curWaitingTime = 0f;
                _waitSlider.value = 0f;
            }

            _coroutine = StartCoroutine(Jump());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_waitingTime != 0f)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _curWaitingTime = 0f;
            _waitSlider.value = 0f;
            _waitSlider.gameObject.SetActive(false);
        }
    }

    private IEnumerator Jump()
    {
        if (_waitingTime != 0f)
        {
            _waitSlider.gameObject.SetActive(true);
            while (_curWaitingTime < _waitingTime)
            {
                _curWaitingTime += Time.deltaTime;
                _waitSlider.value = Mathf.Lerp(_waitSlider.value, _curWaitingTime / _waitingTime, Time.deltaTime * 10f);
                yield return null;
            }
        }

        Player player = Singleton<CharacterManager>.Instance().Player;
        Vector3 force = _isForward ? (_power * 0.4f * player.transform.forward) + (Vector3.up * _power) : Vector3.up * _power;
        player.Controller.AddForce(force, ForceMode.Impulse);
        yield break;
    }
}
