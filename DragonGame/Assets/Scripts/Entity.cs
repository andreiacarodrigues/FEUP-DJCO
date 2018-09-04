using System;
using System.Collections.Generic;

public class EntityTypeStat
{
    public readonly Dictionary<int, EntityStat> SpecialLevels = new Dictionary<int, EntityStat>();
	public Func<int, EntityStat> OtherLevelsFunction;

	public EntityStat GetStats(int level)
	{
		return SpecialLevels.ContainsKey(level) ? SpecialLevels[level] : OtherLevelsFunction(level);
	}
}

public class EntityStat
{
	public int MaxHealth { get; private set; }
	public int Attack { get; private set; }
	public int Experience { get; private set; }
	public int Level { get; private set; }

	public EntityStat(int level, int maxHealth, int attack, int experience)
	{
		Level = level;
		MaxHealth = maxHealth;
		Attack = attack;
		Experience = experience;
	}
}

public class Entity
{
	public EntityStat Stats;
	public float CurrentHealth;

	protected static readonly Dictionary<string, EntityTypeStat> EntityTypeInfo = new Dictionary<string, EntityTypeStat>();

    static Entity()
    {
        EntityTypeStat mage = new EntityTypeStat { OtherLevelsFunction = GetMageStats };
        EntityTypeInfo["Mage"] = mage;
        mage.SpecialLevels[1] = new EntityStat(1, 15, 5, 10);
        mage.SpecialLevels[2] = new EntityStat(2, 30, 10, 20);
        mage.SpecialLevels[3] = new EntityStat(3, 40, 12, 30);
        mage.SpecialLevels[4] = new EntityStat(3, 40, 12, 30);
        mage.SpecialLevels[5] = new EntityStat(3, 75, 21, 30); // Prince

        EntityTypeStat dragon = new EntityTypeStat {OtherLevelsFunction = GetPlayerStats};
		EntityTypeInfo["Dragon"] = dragon;
        dragon.SpecialLevels[1] = new EntityStat(1, 40, 5, 30);         // 1 + 2
        dragon.SpecialLevels[2] = new EntityStat(2, 80, 5, 40);         // 2 + 2
        dragon.SpecialLevels[3] = new EntityStat(3, 120, 8, 80);        // 2 + 2 + 4
        dragon.SpecialLevels[4] = new EntityStat(4, 150, 12, 160);      // 4 + 4 + 4 + 4
        dragon.SpecialLevels[5] = new EntityStat(5, 225, 15, 270);      // 6 + 6 + 6 + 9
        dragon.SpecialLevels[6] = new EntityStat(6, 250, 17, 500);      // Can't
    }

    public void InitStats(string entityTypeName, int level)
	{
		Stats = EntityTypeInfo[entityTypeName].GetStats(level);
		CurrentHealth = Stats.MaxHealth;
	}

	private static EntityStat GetMageStats(int level)
	{
        // Level, Health, Attack, Exp
		return new EntityStat(level, 20*level, 2*level, level);
	}

	private static EntityStat GetPlayerStats(int level)
	{
        // Level, Health, Attack, ExpNeeded
		return new EntityStat(level, 2000*level, 20*level, 2*level);
	}
}

public class PlayerEntity : Entity
{
	public int CurrentExperience;

	public new void InitStats(string entityTypeName, int level)
	{
		base.InitStats(entityTypeName, level);
		CurrentExperience = 0;
	}

	public void LevelUp()
	{
		InitStats("Dragon", Stats.Level + 1);
	}
}