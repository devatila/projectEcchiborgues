public abstract class PerkBase
{
    
    public bool hasWaveDuration;
    public abstract void OnApply();
    public abstract void OnRemove();

    public virtual void Update(float deltaTime = 0) { }
    public virtual void UpdateWaveCount() { }
    public virtual bool IsExpired => false;

    
}
