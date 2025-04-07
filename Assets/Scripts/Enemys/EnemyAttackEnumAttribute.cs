using System;

[AttributeUsage(AttributeTargets.Class,Inherited = false)]
public class EnemyAttackEnumAttribute : Attribute
{
    public EnemyBase.EnemyTypes enemyType;
    public Type enumType;

    public EnemyAttackEnumAttribute(EnemyBase.EnemyTypes enemyType, Type enumType)
    {
        this.enemyType = enemyType;
        this.enumType = enumType;
    }
}
