using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatSkill : FusionSkillExecutioner
{
	//Make values negative to make defensive skills
	public static int EXTRAMT = 0;
	public static int EXTRADEF = 1;
	public static int EXTRAHIT = 2;
	public static int EXTRAAVO = 3;
	public static int EXTRACRIT = 4;
	public static int EXTRACOUNT = 5;

	public CombatSkill(string skillName, string description) : base (skillName, description) {}

	public abstract int[] tryActivate(Unit user, Unit dfd, bool myAttack);

	public class Spindash : CombatSkill
	{
		public Spindash() : base("Spindash", "Skill% chance of adding your strength to your attack power.") {}
		public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
		{
			if (!myAttack)
			{
				return null;
			}
			if (Random.Range(0, 100) < user.skill)
			{
				return new int[] { user.strength, 0, 0, 0, 0, 0 };
			}
			return null;
		}
	}
	public class Fire : CombatSkill
    {
		public Fire () : base ("Fire", "") {}
        public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
        {
			return null;
        }
    }
	public class Freeze : CombatSkill
	{
		public Freeze() : base("Freeze", "") {}
		public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
		{
			return null;
		}
	}
	public class Luna : CombatSkill
	{
		public Luna() : base("Luna", "Skill% chance of halving your opponent's defense for an attack.") {}
		public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
		{
			if (!myAttack)
			{
				return null;
			}
			if (Random.Range(0, 100) < user.skill)
			{
				return new int[] { 0, -(dfd.defense / 2), 0, 0, 0, 0 };
			}
			return null;
		}
	}
	public class Astra : CombatSkill
	{
		public Astra() : base("Astra", "Skill% chance of attacking 5 times instead of 1.") {}
		public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
		{
			if (!myAttack)
            {
				return null;
			}
			if (Random.Range(0, 100) < user.skill)
            {
				return new int[] { 0, 0, 0, 0, 0, 4 };
            }
			return null;
		}
	}
	public class Absorption : CombatSkill
	{
		public Absorption() : base("Absorption", "Skill% chance of doubling your defense for an attack.") { }
		public override int[] tryActivate(Unit user, Unit dfd, bool myAttack)
		{
			if (myAttack)
			{
				return null;
			}
			if (Random.Range(0, 100) < user.skill)
			{
				return new int[] { 0, user.defense, 0, 0, 0, 0 };
			}
			return null;
		}
	}
}
