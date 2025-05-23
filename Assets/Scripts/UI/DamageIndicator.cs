using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _color;
    [SerializeField] private float _flashSpeed;

    private Coroutine _coroutine;

    private void Start()
    {
        Singleton<CharacterManager>.Instance().Player.Condition.OnTakeDamage += Flash;
    }

    public void Flash()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _image.enabled = true;
        _image.color = _color;
        _coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float alpha = _color.a;

        while (alpha > 0f)
        {
            alpha -= (_color.a / _flashSpeed) * Time.deltaTime;
            _image.color = new Color(_color.r, _color.g, _color.b, alpha);

            yield return null;
        }

        _image.enabled = false;
        yield break;
    }
}
