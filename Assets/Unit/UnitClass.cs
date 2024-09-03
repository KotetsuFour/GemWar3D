public class UnitClass
{
    public UnitType[] unitTypes;
	public string className;
	public UnitClass promotion;

	public int minMaxHP;
	public int minStrength;
	public int minMagic;
	public int minSkill;
	public int minSpeed;
	public int minLuck;
	public int minDefense;
	public int minResistance;
	public int minConstitution;
	public int minMovement;
	public int rawEXPReward;

	public int id;

	public static UnitClass lord = new UnitClass("Lord", 18, 4, 0, 2, 3, 0, 2, 0, 5, 6, new UnitType[] { }, 30, 0);
	public static UnitClass servant = new UnitClass("Servant", 20, 3, 0, 5, 6, 0, 2, 0, 6, 6, new UnitType[] { }, 30, 1);
	public static UnitClass soldier = new UnitClass("Soldier", 20, 4, 0, 4, 5, 0, 2, 0, 8, 6, new UnitType[] { UnitType.QUARTZ }, 30, 2);
	public static UnitClass architect = new UnitClass("Architect", 20, 4, 0, 4, 5, 0, 2, 0, 8, 6, new UnitType[] { UnitType.HEAVY }, 30, 3);
	public static UnitClass diplomat = new UnitClass("Diplomat", 18, 0, 2, 2, 3, 0, 0, 2, 4, 6, new UnitType[] { UnitType.NOBLE }, 30, 4);
	public static UnitClass guard = new UnitClass("Guard", 16, 1, 0, 1, 6, 0, 0, 0, 5, 5, new UnitType[] { }, 30, 5);
	public static UnitClass priestess = new UnitClass("Priestess", 16, 2, 0, 3, 5, 0, 1, 0, 4, 6, new UnitType[] { }, 30, 6);
	public static UnitClass pilot = new UnitClass("Pilot", 16, 2, 3, 3, 6, 0, 2, 3, 4, 8, new UnitType[] { UnitType.FLYING }, 30, 7);
	public static UnitClass elite_quartz = new UnitClass("Elite Quartz", 20, 7, 1, 5, 9, 0, 5, 1, 7, 6, new UnitType[] { UnitType.QUARTZ }, 45, 8);
	public static UnitClass topaz_fusion = new UnitClass("Topaz Fusion", 26, 7, 1, 5, 2, 0, 12, 1, 10, 6, new UnitType[] { }, 45, 9);

	public static UnitClass[] unitClassIndex = new UnitClass[] { lord, servant, soldier, architect, diplomat, guard, priestess, pilot,
				elite_quartz, topaz_fusion };

	public UnitClass(string className, int minMaxHP, int minStrength, int minMagic, int minSkill,
			int minSpeed, int minLuck, int minDefense, int minResistance, int minConstitution,
			int minMovement, UnitType[] unitTypes, int rawEXPReward, int id)
    {
		this.className = className;
		this.minMaxHP = minMaxHP;
		this.minStrength = minStrength;
		this.minMagic = minMagic;
		this.minSkill = minSkill;
		this.minSpeed = minSpeed;
		this.minLuck = minLuck;
		this.minDefense = minDefense;
		this.minResistance = minResistance;
		this.minConstitution = minConstitution;
		this.minMovement = minMovement;
		this.unitTypes = unitTypes;
		this.rawEXPReward = rawEXPReward;
		this.id = id;
	}

	public bool isFlying()
    {
		foreach (UnitType type in unitTypes)
        {
			if (type == UnitType.FLYING)
            {
				return true;
            }
        }
		return false;
    }

	public enum UnitType
    {
        FLYING, HEAVY, QUARTZ, ORGANIC, NOBLE
    }
}
