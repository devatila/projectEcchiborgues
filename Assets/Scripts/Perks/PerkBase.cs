using UnityEngine;
public abstract class PerkBase
{
    
    public bool hasWaveDuration;
    public bool oneTimeForWave;
    public bool removeOnExecute;
    protected bool alreadyExectuedInThisWave;
    protected int waveCounts;
    protected int maxWaves;
    protected bool isActive;

    protected PerkSO perkso;

    public PerkBase(PerkSO so)
    {
        this.hasWaveDuration = so.hasValidateTime;
        this.maxWaves = so.wavesDuration;
    }

    public abstract void OnApply();
    public abstract void OnRemove();

    public virtual void Update(float deltaTime = 0) { }
    public virtual void UpdateWaveCount() 
    {
        alreadyExectuedInThisWave = false;

        if (hasWaveDuration)
        {
            Debug.Log("Chamou ao fim da orda: " + waveCounts);
            waveCounts++;
            Debug.Log(IsExpired + " " + waveCounts + " " + (waveCounts >= maxWaves));
        }
    }
    public virtual bool IsExpired => hasWaveDuration == true ? waveCounts >= maxWaves : !isActive;

    
}
