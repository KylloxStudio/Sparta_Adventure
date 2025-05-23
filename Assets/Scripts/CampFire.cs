using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireParticle;
    [SerializeField] private Light _light;

    [SerializeField] private bool _isBurning;
    [SerializeField] private int _damage;
    [SerializeField] private float _cooltime;

    private Coroutine _coroutine;
    private List<IDamagable> _damagableThings = new List<IDamagable>();

    private void Start()
    {
        SetBurning(_isBurning);
        _coroutine = StartCoroutine(CheckDamagable());
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isBurning)
        {
            return;
        }

        if (other.TryGetComponent(out IDamagable damagable))
        {
            _damagableThings.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isBurning)
        {
            return;
        }

        if (other.TryGetComponent(out IDamagable damagable))
        {
            _damagableThings.Remove(damagable);
        }
    }

    public void SetBurning(bool active)
    {
        _fireParticle.gameObject.SetActive(active);
        _light.gameObject.SetActive(active);
        _isBurning = active;
    }

    private IEnumerator CheckDamagable()
    {
        while (true)
        {
            if (!_isBurning)
            {
                _damagableThings.Clear();
                yield return null;
                continue;
            }

            if (_damagableThings.Count > 0)
            {
                GiveDamage();
            }
            
            yield return new WaitForSeconds(_cooltime);
        }
    }

    private void GiveDamage()
    {
        for (int i = 0; i < _damagableThings.Count; i++)
        {
            _damagableThings[i].TakePhysicalDamage(_damage);
        }
    }
}
