using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    [SerializeField]
    private int _startingHp;

    [SerializeField]
    private int _hp;

    [SerializeField]
    private int _currentBlock;

    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private GameObject _hitMarker;
    private SpriteRenderer _hitMarkerRenderer;

    public bool IsDead { get; private set; }

    public int Health {
        get {
            return _hp;
        }
    }

    private static Player _instance;

	public static Player Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Player>();
			}
			return _instance;
		}
	}
    
	public delegate void DeathAction();
	public static event DeathAction OnDeath;

    public int StartingHealth {
        get {
            return _startingHp;
        }
    }

    public int Block
    {
        get
        {
            return _currentBlock;
        }
        set 
        {
            _currentBlock = value;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _hp = _startingHp;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hitMarkerRenderer = _hitMarker.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_hp < 0 && !IsDead)
        {
            JustDieAlready();
        }
    }

    private void JustDieAlready()
    {
        IsDead = true;
        OnDeath();
    }

    public void StartTurn()
    {
        _currentBlock = 0;
        Debug.Log("Set players block to 0 at start of turn.");
    }

    public void TakeDamage(int damageValue)
    {
        var mitigatedDamageValue = Math.Max(0, damageValue - _currentBlock);
        _currentBlock = Math.Max(0, _currentBlock - damageValue);

        if (mitigatedDamageValue > 0)
        {
            _hp -= mitigatedDamageValue;
            StartCoroutine(FadeRed(0, 0.1f, true));
            StartCoroutine(FadeHitMarker(1f, .1f, () => Invoke("FadeHitMarkerOut", 1f)));
        }

        Debug.Log($"Player struck with {damageValue} damage, mitigated to {mitigatedDamageValue}. New block {_currentBlock}. New hp {_hp}.");
    }

    private void FadeHitMarkerOut()
    {
        StartCoroutine(FadeHitMarker(0, .4f, null));
    }

    public void GainBlock(int blockValue)
    {
        _currentBlock += blockValue;

        Debug.Log($"Player gained {blockValue} block. New block value {_currentBlock}.");
    }

    IEnumerator FadeHitMarker(float newAlphaValue, float aTime, Action onFinish)
    {
        float alpha = _hitMarkerRenderer.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, newAlphaValue, t));
            _hitMarkerRenderer.color = newColor;
            yield return null;
        }

        if (onFinish != null)
        {
            onFinish();
        }
    }

    IEnumerator FadeRed(float aValue, float aTime, bool toRed)
    {
        float gb = _spriteRenderer.color.g;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, Mathf.Lerp(gb, aValue, t), Mathf.Lerp(gb, aValue, t), 1);
            _spriteRenderer.color = newColor;
            yield return null;
        }

        if (toRed)
        {
            StartCoroutine(FadeRed(1.0f, 0.1f, false));
        }

        if (!toRed)
        {
            _spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}
