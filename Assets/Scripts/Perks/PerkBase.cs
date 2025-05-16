public abstract class PerkBase
{
    public abstract void OnApply();
    public abstract void OnRemove();

    public virtual void Update(float deltaTime = 0) { }
    public virtual bool IsExpired => false;
}
