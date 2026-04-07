public class EnemyIdleState : IEnemyState
{
    private EnemyAI enemy;

    public EnemyIdleState(EnemyAI enemy)
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
    }
}