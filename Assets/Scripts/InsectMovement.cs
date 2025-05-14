using UnityEngine;

public class InsectMovement : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Die");
        anim.SetFloat("Speed", 0);
    }

    protected override void HandleMovement(Vector2 dir, float dist)
    {
        float speed = isAttacking ? attackMoveSpeed : moveSpeed;
        float move = dist > stopDistance ? speed : 0f;
        if (move > 0f)
            rb.MovePosition(rb.position + dir * move * Time.fixedDeltaTime);
        anim.SetFloat("Speed", move);
    }

    protected override void HandleAttack(float dist)
    {
        if (isAttacking) return;
        if (dist <= attackRange)
        {
            isAttacking = true;
            if (attackClip) AudioSource.PlayClipAtPoint(attackClip, transform.position);
            anim.SetTrigger("Attack");
        }
    }

    public override void AttackFinished()
    {
        if (isDead) return;
        player.GetComponent<GameController>()?.TakeDamage(10);
        Kill();
        base.AttackFinished();
        Debug.Log("Vampire AttackFinished()");
    }
}