using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AfterAttackSkill : FusionSkillExecutioner
{
    public static int MYHPCHANGE = 0;
    public static int YOURHPCHANGE = 1;
    public AfterAttackSkill(string skillName, string description) : base (skillName, description) {}

    public abstract int[] skillEffect(Unit user, Unit other, bool hit, int damage, bool myAttack);

    public class Sol : AfterAttackSkill
    {
        public Sol() : base("Sol", "After you damage your opponent, Skill% chance of healing that amount of damage to yourself.") { }

        public override int[] skillEffect(Unit user, Unit other, bool hit, int damage, bool myAttack)
        {
            if (!myAttack)
            {
                return null;
            }
            if (hit && damage > 0 && Random.Range(0, 100) < user.skill)
            {
                return new int[] { damage, 0 };
            }
            return null;
        }
    }
}
