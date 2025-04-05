
public class PlayerCharacter : OnTheTruck, IDamageable
{
    public Damageable damageable;

    public int maxHp;
    public int MaxHp => maxHp;

    public void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        InitDamageable();
    }

    public override void OnRemoved()
    {
        Clear();
        ObjectManager.Ins.Kill(gameObject);
    }

    public void InitDamageable()
    {
        damageable.Init(this);
        damageable.OnDamage += OnDamageHandler;
        damageable.OnDead += OnDeadHandler;
    }

    public void TakeDamage(int damage)
    {
        damageable.TakeDamage(damage);
    }

    public void OnDamageHandler(int damage)
    {
        
    }

    public void OnDeadHandler()
    {
        OnRemoved();
    }

    private void Clear()
    {
        damageable.OnDamage -= OnDamageHandler;
        damageable.OnDead -= OnDeadHandler;
    }

    
}
