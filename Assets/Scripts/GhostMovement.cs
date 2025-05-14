using UnityEngine;

public class GhostMovement : EnemyBase
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
            anim.SetFloat("VelX", (nextPos - rb.position).x / Time.fixedDeltaTime);
            anim.SetFloat("VelY", (nextPos - rb.position).y / Time.fixedDeltaTime);
            rb.MovePosition(nextPos);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("VelX", 0);
            anim.SetFloat("VelY", 0);
        }
    }

    public override void AttackFinished()
    {
        if (isDead) return;
        player.GetComponent<GameController>()?.TakeDamage(5);
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