using UnityEngine;

public class GhostMovement: EnemyBase
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
            rb.MovePosition(nextPos);

            Vector2 vel = (nextPos - rb.position) / Time.fixedDeltaTime;
            anim.SetFloat("VelX", vel.x);
            anim.SetFloat("VelY", vel.y);
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
        DeathFinished();
    }
}