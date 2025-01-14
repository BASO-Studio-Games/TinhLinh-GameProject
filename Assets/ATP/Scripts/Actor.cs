using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]

public class Actor : MonoBehaviour
{
    [Header("Common:")]
    public ActorStats statsData;

    private bool m_isDead;
    private float m_curHp;


    protected Rigidbody2D m_rb;
    protected Animator m_anim;


    [Header("Events:")]
    public UnityEvent OnInit;
    public UnityEvent<float> OnTakeDamage;
    public UnityEvent OnDead;

    public bool IsDead { get => m_isDead; set => m_isDead = value; }
    public float CurHp
    {
        get => m_curHp;
        set => m_curHp = value;
    }
    protected virtual void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        Init();

        OnInit?.Invoke();
    }

    public virtual void Init()
    {

    }

    public virtual void TakeDamage(float damage)
    {
        if (damage < 0) return;
        m_curHp -= damage;

        Debug.Log($"Enemy took {damage} damage, current HP: {m_curHp}");
        if (m_curHp <= 0)
        {
            m_curHp = 0;
            Die();
        }

        OnTakeDamage?.Invoke(CurHp);
    }

    protected virtual void Die()
    {
        m_isDead = true;
        m_rb.linearVelocity = Vector3.zero;

        OnDead?.Invoke();

        Destroy(gameObject, 0.5f);
    }

}
