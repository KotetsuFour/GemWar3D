using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapSkill : FusionSkillExecutioner
{
    public MapSkill(string skillName, string description) : base(skillName, description) { }

    public class Healing : MapSkill
    {
        public Healing() : base("Healing Tears", "Use on allies to restore HP equal to your MAG.")
        {

        }
    }
}
