using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionSkillExecutioner
{
    public string skillName;
    public string description;

    public static FusionSkillExecutioner[] SKILL_LIST =
    {
        new FusionSkillExecutioner("Locked", ""),
        new FusionSkillExecutioner("Future Vision", "Battle forecasts become predictable for the rest of the turn (usable once per chapter)."),
        new FusionSkillExecutioner("Plants", ""),
        new FusionSkillExecutioner("Hologram", "Summon a hologram of this unit as an ally, with the same basic stats."),
        new CombatSkill.Spindash(),
        new CombatSkill.Fire(),
        new CombatSkill.Freeze(),
        new FusionSkillExecutioner("Vantage", "Always attack first when at or below half health."),
        new AfterAttackSkill.Sol(),
        new CombatSkill.Luna(),
        new CombatSkill.Astra(),
        new MapSkill.Healing(),
        new CombatSkill.Absorption(),
    };

    public FusionSkillExecutioner(string skillName, string description)
    {
        this.skillName = skillName;
        this.description = description;
    }

    public static bool activateVantage(Unit unit)
    {
        return (unit.fusionSkill1 == Unit.FusionSkill.VANTAGE
            || unit.fusionSkill2 == Unit.FusionSkill.VANTAGE
            || unit.fusionSkillBonus == Unit.FusionSkill.VANTAGE)
            && unit.currentHP <= unit.maxHP / 2;
    }

}
