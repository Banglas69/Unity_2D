public class EnemyMeleeAttackState : IEnemyState
{
    private EnemyAI enemy;

    public EnemyMeleeAttackState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.StopMoving();
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        enemy.StopMoving();
        enemy.TryMeleeAttack();
    }
}