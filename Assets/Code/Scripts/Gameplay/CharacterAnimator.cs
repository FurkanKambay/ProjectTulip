﻿using System;
using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Gameplay
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] Animator animator;
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip dieSound;

        private IMovement movement;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDie = Animator.StringToHash("die");

        private void FixedUpdate() => animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));

        private void OnDamageTaken(DamageEventArgs damage)
        {
            if (hurtSound) AudioSource.PlayClipAtPoint(hurtSound, damage.Source.transform.position);
            animator.SetTrigger(animHurt);
        }

        private void OnDied(DamageEventArgs damage)
        {
            if (dieSound) AudioSource.PlayClipAtPoint(dieSound, damage.Source.transform.position);
            animator.SetTrigger(animDie);
            DestroyAfterAnimation(.5f);
        }

        private void DestroyAfterAnimation(float extraDelay = 0f)
        {
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, length + extraDelay);
        }

        private void Awake()
        {
            health ??= GetComponent<Health>();
            animator ??= GetComponent<Animator>();
            movement ??= GetComponent<IMovement>();
        }

        private void OnEnable()
        {
            health.DamageTaken += OnDamageTaken;
            health.Died += OnDied;
        }

        private void OnDisable()
        {
            health.DamageTaken -= OnDamageTaken;
            health.Died -= OnDied;
        }
    }
}
