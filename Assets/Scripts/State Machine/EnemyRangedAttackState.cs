public class EnemyRangedAttackState : IEnemyState
{
    private EnemyAI enemy;

    public EnemyRangedAttackState(EnemyAI enemy)
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
        enemy.TryShoot();
    }
}