public abstract class PerkBase
{
    
    public bool hasWaveDuration;
    public bool oneTimeForWave;
    public bool removeOnExecute;
    protected bool alreadyExectuedInThisWave;
    protected int waveCounts;
    protected int maxWaves;

    public abstract void OnApply();
    public abstract void OnRemove();

    public virtual void Update(float deltaTime = 0) { }
    public virtual void UpdateWaveCount() 
    {
        alreadyExectuedInThisWave = false;

        if (hasWaveDuration)
        {
            waveCounts++;
        }
    }
    public virtual bool IsExpired => false;

    
}
