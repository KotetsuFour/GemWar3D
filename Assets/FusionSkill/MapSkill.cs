using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapSkill : FusionSkillExecutioner
{
    public MapSkill(string skillName, string description, int maxUsesPerMap) : base(skillName, description)
    {
        this.maxUsesPerMap = maxUsesPerMap;
    }
    public abstract MapSkillInputType getQualification();
    public abstract MapSkillInputType[] getInputTypes();
    public abstract void activateEffect(Unit user, object[] input);
    public int maxUsesPerMap;
    public void incrementUses(Unit user)
    {
        if (SKILL_LIST[(int)user.fusionSkill1] == this)
        {
            user.timesUsedMapSkill1++;
        }
        else if (SKILL_LIST[(int)user.fusionSkill2] == this)
        {
            user.timesUsedMapSkill2++;
        }
        else if (SKILL_LIST[(int)user.fusionSkillBonus] == this)
        {
            user.timesUsedMapSkillBonus++;
        }
    }
    public class Healing : MapSkill
    {
        public Healing() : base("Healing Tears", "Use to restore HP to an adjacent ally equal to your MAG + 10.", int.MaxValue) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ADJACENT_ALLY;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ADJACENT_ALLY };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            Unit unit = (Unit)input[0];
            unit.heal(user.magic + 10);
            ParticleAnimation particles = AssetDictionary.getParticles("heal");
            Object.Instantiate(particles, unit.model.transform.position, particles.transform.rotation);
        }
    }
    public class Warp : MapSkill
    {
        public Warp() : base("Warp", "Use to teleport an adjacent ally to another traversable tile.", 3) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ADJACENT_ALLY;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ADJACENT_ALLY, MapSkillInputType.TRAVERSABLE_TILE };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
            incrementUses(user);
        }
    }
    public class Rescue : MapSkill
    {
        public Rescue() : base("Rescue", "Use on an ally to teleport them to an adjacent tile.", 3) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ANY_ALLY;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ANY_ALLY, MapSkillInputType.ADJACENT_TRAVERSABLE_TILE };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
            incrementUses(user);
        }
    }
    public class Revive : MapSkill
    {
        public Revive() : base("Revive", "Use to extract an ally from their gemstone to an adjacent tile.", 1) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.HELD_ALLY_GEM;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ADJACENT_TRAVERSABLE_TILE };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
            incrementUses(user);
        }
    }
    public class Unlock : MapSkill
    {
        public Unlock() : base("Unlock", "Use to open an adjacent door.", int.MaxValue) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ADJACENT_DOOR;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ADJACENT_DOOR };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
        }
    }
    public class Thief : MapSkill
    {
        public Thief() : base("Thief", "Use to steal from a chest on the map.", 3) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ANY_CHEST;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ANY_CHEST };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
            incrementUses(user);
        }
    }
    public class Song : MapSkill
    {
        public Song() : base("Song", "Use to invigorate an adjacent ally to move again.", int.MaxValue) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.ADJACENT_ALLY;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.ADJACENT_ALLY };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
        }
    }
    public class Rewarp : MapSkill
    {
        public Rewarp() : base("Rewarp", "Use to teleport to another traversable tile.", 3) { }
        public override MapSkillInputType getQualification()
        {
            return MapSkillInputType.WHENEVER;
        }
        public override MapSkillInputType[] getInputTypes()
        {
            return new MapSkillInputType[] { MapSkillInputType.TRAVERSABLE_TILE };
        }
        public override void activateEffect(Unit user, object[] input)
        {
            //TODO
            incrementUses(user);
        }
    }

    public enum MapSkillInputType
    {
        ADJACENT_ALLY, TRAVERSABLE_TILE, ANY_ALLY, ADJACENT_TRAVERSABLE_TILE,
        HELD_ALLY_GEM, CONFIRM, ADJACENT_DOOR, ANY_CHEST, WHENEVER
    }
}
