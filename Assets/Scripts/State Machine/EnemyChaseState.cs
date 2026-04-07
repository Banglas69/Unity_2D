public class EnemyChaseState : IEnemyState
{
    private EnemyAI enemy;

    public EnemyChaseState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        enemy.MoveTowardsPlayer();
    }
}