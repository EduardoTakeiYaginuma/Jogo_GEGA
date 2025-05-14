using UnityEngine;

public class VampireMovement : EnemyBase
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
    }

    protected override void HandleMovement(Vector2 dir, float dist)
    {
        float speed = isAttacking ? attackMoveSpeed : moveSpeed;
        if (dist > stopDistance)
        {
            Vector2 nextPos = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            Vector2 delta = (nextPos - rb.position) / Time.fixedDeltaTime;
            anim.SetFloat("VelX", delta.x);
            anim.SetFloat("VelY", delta.y);
            rb.MovePosition(nextPos);
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
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
    }

    public override void Kill()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");
        DeathFinished();  
    }
}