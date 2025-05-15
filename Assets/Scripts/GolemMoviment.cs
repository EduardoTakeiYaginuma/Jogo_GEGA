using UnityEngine;

public class GolemMovement : EnemyBase
{
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
            Vector2 vel = (nextPos - rb.position) / Time.fixedDeltaTime;
            anim.SetFloat("VelX", vel.x);
            anim.SetFloat("VelY", vel.y);
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
        player.GetComponent<GameController>()?.TakeDamage(20);
        Kill();
        base.AttackFinished();
    }

    public override void Kill()
    {
        if (isDead) return;
        isDead = true;
        DeathFinished();
    }
}