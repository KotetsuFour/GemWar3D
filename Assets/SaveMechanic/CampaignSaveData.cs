using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CampaignSaveData
{
    public int iron;
    public int steel;
    public int silver;
	public int bonusEXP;

	public string[] unitName;
	public int[] unitClass;
	public string[] description;
	public int[] maxHP;
	public int[] currentHP;
	public int[] strength;
	public int[] magic;
	public int[] skill;
	public int[] speed;
	public int[] luck;
	public int[] defense;
	public int[] resistance;
	public int[] constitution;
	public int[] movement;

	public int[] hpGrowth;
	public int[] strengthGrowth;
	public int[] magicGrowth;
	public int[] skillGrowth;
	public int[] speedGrowth;
	public int[] luckGrowth;
	public int[] defenseGrowth;
	public int[] resistanceGrowth;
	public int[] level;
	public int[] experience;

	public int[] personalItemId;
	public int[] personalItemUsesLeft;
	public int[] heldWeaponId;
	public int[] heldWeaponUsesLeft;
	public int[] heldItemId;
	public int[] heldItemUsesLeft;

	public int[] weaponType;
	public int[] proficiency;

	public bool[] isEssential;
	public bool[] isLeader;
	public int[] equipped; //0 = personal, 1 = held, 2 = none
	public bool[] isExhausted;

	public string[][] deathQuote;

	public float[][] palette;

	public int[] supportId1;
	public int[] supportId2;
	public int[] fusionSkill1;
	public int[] fusionSkill2;
	public int[] fusionSkillBonus;
	public int[] affinity;

	public int[] battles;
	public int[] wins;
	public int[] losses;

	public string[] punitName;
	public int[] punitClass;
	public string[] pdescription;
	public int[] pmaxHP;
	public int[] pcurrentHP;
	public int[] pstrength;
	public int[] pmagic;
	public int[] pskill;
	public int[] pspeed;
	public int[] pluck;
	public int[] pdefense;
	public int[] presistance;
	public int[] pconstitution;
	public int[] pmovement;

	public int[] phpGrowth;
	public int[] pstrengthGrowth;
	public int[] pmagicGrowth;
	public int[] pskillGrowth;
	public int[] pspeedGrowth;
	public int[] pluckGrowth;
	public int[] pdefenseGrowth;
	public int[] presistanceGrowth;
	public int[] plevel;
	public int[] pexperience;

	public int[] ppersonalItemId;
	public int[] ppersonalItemUsesLeft;
	public int[] pheldWeaponId;
	public int[] pheldWeaponUsesLeft;
	public int[] pheldItemId;
	public int[] pheldItemUsesLeft;

	public int[] pweaponType;
	public int[] pproficiency;

	public bool[] pisEssential;
	public bool[] pisLeader;
	public int[] pequipped; //0 = personal, 1 = held, 2 = none
	public bool[] pisExhausted;

	public string[][] pdeathQuote;

	public float[][] ppalette;

	public int[] psupportId1;
	public int[] psupportId2;
	public int[] pfusionSkill1;
	public int[] pfusionSkill2;
	public int[] pfusionSkillBonus;
	public int[] pAffinity;

	public int[] pBattles;
	public int[] pWins;
	public int[] pLosses;

	public int[] supportAmounts;
	public int[] supportLevels;

	public int scene;

	public int[][] convoyIds;
	public int[][] convoyDurabilities;

	public int savefile;

	public int chapterPrep;
	public int[] positions;

	public CampaignSaveData()
    {
        iron = StaticData.iron;
        steel = StaticData.steel;
        silver = StaticData.silver;
		bonusEXP = StaticData.bonusEXP;

		unitName = new string[StaticData.members.Count];
		unitClass = new int[StaticData.members.Count];
		description = new string[StaticData.members.Count];
		maxHP = new int[StaticData.members.Count];
		currentHP = new int[StaticData.members.Count];
		strength = new int[StaticData.members.Count];
		magic = new int[StaticData.members.Count];
		skill = new int[StaticData.members.Count];
		speed = new int[StaticData.members.Count];
		luck = new int[StaticData.members.Count];
		defense = new int[StaticData.members.Count];
		resistance = new int[StaticData.members.Count];
		constitution = new int[StaticData.members.Count];
		movement = new int[StaticData.members.Count];

		hpGrowth = new int[StaticData.members.Count];
		strengthGrowth = new int[StaticData.members.Count];
		magicGrowth = new int[StaticData.members.Count];
		skillGrowth = new int[StaticData.members.Count];
		speedGrowth = new int[StaticData.members.Count];
		luckGrowth = new int[StaticData.members.Count];
		defenseGrowth = new int[StaticData.members.Count];
		resistanceGrowth = new int[StaticData.members.Count];
		level = new int[StaticData.members.Count];
		experience = new int[StaticData.members.Count];

		personalItemId = new int[StaticData.members.Count];
		personalItemUsesLeft = new int[StaticData.members.Count];
		heldWeaponId = new int[StaticData.members.Count];
		heldWeaponUsesLeft = new int[StaticData.members.Count];
		heldItemId = new int[StaticData.members.Count];
		heldItemUsesLeft = new int[StaticData.members.Count];

		weaponType = new int[StaticData.members.Count];;
		proficiency = new int[StaticData.members.Count];;

		isEssential = new bool[StaticData.members.Count];
		isLeader = new bool[StaticData.members.Count];
		equipped = new int[StaticData.members.Count]; //0 = personal, 1 = held, 2 = none
		isExhausted = new bool[StaticData.members.Count];

		deathQuote = new string[StaticData.members.Count][];

		palette = new float[StaticData.members.Count][];

		supportId1 = new int[StaticData.members.Count];
		supportId2 = new int[StaticData.members.Count];
		fusionSkill1 = new int[StaticData.members.Count];
		fusionSkill2 = new int[StaticData.members.Count];
		fusionSkillBonus = new int[StaticData.members.Count];
		affinity = new int[StaticData.members.Count];

		battles = new int[StaticData.members.Count];
		wins = new int[StaticData.members.Count];
		losses = new int[StaticData.members.Count];

		for (int q = 0; q < StaticData.members.Count; q++)
        {
			Unit m = StaticData.members[q];

			unitName[q] = m.unitName;
			unitClass[q] = m.unitClass.id;
			description[q] = m.description;
			maxHP[q] = m.maxHP;
			currentHP[q] = m.currentHP;
			strength[q] = m.strength;
			magic[q] = m.magic;
			skill[q] = m.skill;
			speed[q] = m.speed;
			luck[q] = m.luck;
			defense[q] = m.defense;
			resistance[q] = m.resistance;
			constitution[q] = m.constitution;
			movement[q] = m.movement;

			hpGrowth[q] = m.hpGrowth;
			strengthGrowth[q] = m.strengthGrowth;
			magicGrowth[q] = m.magicGrowth;
			skillGrowth[q] = m.skillGrowth;
			speedGrowth[q] = m.speedGrowth;
			luckGrowth[q] = m.luckGrowth;
			defenseGrowth[q] = m.defenseGrowth;
			resistanceGrowth[q] = m.resistanceGrowth;
			level[q] = m.level;
			experience[q] = m.experience;

			personalItemId[q] = m.personalItem == null ? -1 : m.personalItem.id;
			personalItemUsesLeft[q] = m.personalItem == null ? -1 : m.personalItem.usesLeft;
			heldWeaponId[q] = m.heldWeapon == null ? -1 : m.heldWeapon.id;
			heldWeaponUsesLeft[q] = m.heldWeapon == null ? -1 : m.heldWeapon.usesLeft;
			heldItemId[q] = m.heldItem == null ? -1 : m.heldItem.id;
			heldItemUsesLeft[q] = m.heldItem == null ? -1 : m.heldItem.usesLeft;

			weaponType[q] = (int)m.weaponType;
			proficiency[q] = m.proficiency;

			isEssential[q] = m.isEssential;
			isLeader[q] = m.isLeader;
			equipped[q] = m.equipped; //0 = personal, 1 = held, 2 = none
			isExhausted[q] = m.isExhausted;

			deathQuote[q] = m.deathQuote;

			palette[q] = new float[m.palette.Count * 3];
			for (int w = 0; w < m.palette.Count; w++)
            {
				palette[q][w * 3] = m.palette[q].r;
				palette[q][(w * 3) + 1] = m.palette[q].g;
				palette[q][(w * 3) + 2] = m.palette[q].b;
			}

			supportId1[q] = m.supportId1;
			supportId2[q] = m.supportId2;
			fusionSkill1[q] = (int)m.fusionSkill1;
			fusionSkill2[q] = (int)m.fusionSkill2;
			fusionSkillBonus[q] = (int)m.fusionSkillBonus;
			affinity[q] = (int)m.affinity;

			battles[q] = m.battles;
			wins[q] = m.wins;
			losses[q] = m.losses;
		}

		punitName = new string[StaticData.prisoners.Count];
		punitClass = new int[StaticData.prisoners.Count];
		pdescription = new string[StaticData.prisoners.Count];
		pmaxHP = new int[StaticData.prisoners.Count];
		pcurrentHP = new int[StaticData.prisoners.Count];
		pstrength = new int[StaticData.prisoners.Count];
		pmagic = new int[StaticData.prisoners.Count];
		pskill = new int[StaticData.prisoners.Count];
		pspeed = new int[StaticData.prisoners.Count];
		pluck = new int[StaticData.prisoners.Count];
		pdefense = new int[StaticData.prisoners.Count];
		presistance = new int[StaticData.prisoners.Count];
		pconstitution = new int[StaticData.prisoners.Count];
		pmovement = new int[StaticData.prisoners.Count];

		phpGrowth = new int[StaticData.prisoners.Count];
		pstrengthGrowth = new int[StaticData.prisoners.Count];
		pmagicGrowth = new int[StaticData.prisoners.Count];
		pskillGrowth = new int[StaticData.prisoners.Count];
		pspeedGrowth = new int[StaticData.prisoners.Count];
		pluckGrowth = new int[StaticData.prisoners.Count];
		pdefenseGrowth = new int[StaticData.prisoners.Count];
		presistanceGrowth = new int[StaticData.prisoners.Count];
		plevel = new int[StaticData.prisoners.Count];
		pexperience = new int[StaticData.prisoners.Count];

		ppersonalItemId = new int[StaticData.prisoners.Count];
		ppersonalItemUsesLeft = new int[StaticData.prisoners.Count];
		pheldWeaponId = new int[StaticData.prisoners.Count];
		pheldWeaponUsesLeft = new int[StaticData.prisoners.Count];
		pheldItemId = new int[StaticData.prisoners.Count];
		pheldItemUsesLeft = new int[StaticData.prisoners.Count];

		pweaponType = new int[StaticData.prisoners.Count]; ;
		pproficiency = new int[StaticData.prisoners.Count]; ;

		pisEssential = new bool[StaticData.prisoners.Count];
		pisLeader = new bool[StaticData.prisoners.Count];
		pequipped = new int[StaticData.prisoners.Count]; //0 = personal, 1 = held, 2 = none
		pisExhausted = new bool[StaticData.prisoners.Count];

		pdeathQuote = new string[StaticData.prisoners.Count][];

		ppalette = new float[StaticData.prisoners.Count][];

		psupportId1 = new int[StaticData.prisoners.Count];
		psupportId2 = new int[StaticData.prisoners.Count];
		pfusionSkill1 = new int[StaticData.prisoners.Count];
		pfusionSkill2 = new int[StaticData.prisoners.Count];
		pfusionSkillBonus = new int[StaticData.prisoners.Count];
		pAffinity = new int[StaticData.prisoners.Count];

		pBattles = new int[StaticData.prisoners.Count];
		pWins = new int[StaticData.prisoners.Count];
		pLosses = new int[StaticData.prisoners.Count];

		for (int q = 0; q < StaticData.prisoners.Count; q++)
		{
			Unit m = StaticData.prisoners[q].unit;

			punitName[q] = m.unitName;
			punitClass[q] = m.unitClass.id;
			pdescription[q] = m.description;
			pmaxHP[q] = m.maxHP;
			pcurrentHP[q] = m.currentHP;
			pstrength[q] = m.strength;
			pmagic[q] = m.magic;
			pskill[q] = m.skill;
			pspeed[q] = m.speed;
			pluck[q] = m.luck;
			pdefense[q] = m.defense;
			presistance[q] = m.resistance;
			pconstitution[q] = m.constitution;
			pmovement[q] = m.movement;

			phpGrowth[q] = m.hpGrowth;
			pstrengthGrowth[q] = m.strengthGrowth;
			pmagicGrowth[q] = m.magicGrowth;
			pskillGrowth[q] = m.skillGrowth;
			pspeedGrowth[q] = m.speedGrowth;
			pluckGrowth[q] = m.luckGrowth;
			pdefenseGrowth[q] = m.defenseGrowth;
			presistanceGrowth[q] = m.resistanceGrowth;
			plevel[q] = m.level;
			pexperience[q] = m.experience;

			ppersonalItemId[q] = m.personalItem == null ? -1 : m.personalItem.id;
			ppersonalItemUsesLeft[q] = m.personalItem == null ? -1 : m.personalItem.usesLeft;
			pheldWeaponId[q] = m.heldWeapon == null ? -1 : m.heldWeapon.id;
			pheldWeaponUsesLeft[q] = m.heldWeapon == null ? -1 : m.heldWeapon.usesLeft;
			pheldItemId[q] = m.heldItem == null ? -1 : m.heldItem.id;
			pheldItemUsesLeft[q] = m.heldItem == null ? -1 : m.heldItem.usesLeft;

			pweaponType[q] = (int)m.weaponType;
			pproficiency[q] = m.proficiency;

			pisEssential[q] = m.isEssential;
			pisLeader[q] = m.isLeader;
			pequipped[q] = m.equipped; //0 = personal, 1 = held, 2 = none
			pisExhausted[q] = m.isExhausted;

			pdeathQuote[q] = m.deathQuote;

			ppalette[q] = new float[m.palette.Count * 3];
			for (int w = 0; w < m.palette.Count; w++)
			{
				ppalette[q][w * 3] = m.palette[q].r;
				ppalette[q][(w * 3) + 1] = m.palette[q].g;
				ppalette[q][(w * 3) + 2] = m.palette[q].b;
			}

			psupportId1[q] = m.supportId1;
			psupportId2[q] = m.supportId2;
			pfusionSkill1[q] = (int)m.fusionSkill1;
			pfusionSkill2[q] = (int)m.fusionSkill2;
			pfusionSkillBonus[q] = (int)m.fusionSkillBonus;
			pAffinity[q] = (int)m.affinity;

			pBattles[q] = m.battles;
			pWins[q] = m.wins;
			pLosses[q] = m.losses;
		}

		supportAmounts = new int[SupportLog.supportLog.Length];
		supportLevels = new int[SupportLog.supportLog.Length];
		for (int q = 0; q < SupportLog.supportLog.Length; q++)
        {
			supportAmounts[q] = SupportLog.supportLog[q].supportAmount;
			supportLevels[q] = (int)SupportLog.supportLog[q].level;
		}
		scene = StaticData.scene;
		convoyIds = new int[StaticData.getConvoyIds().Length][];
		convoyDurabilities = new int[StaticData.getConvoyDurabilities().Length][];
		for (int q = 0; q < convoyIds.Length; q++)
        {
			convoyIds[q] = StaticData.getConvoyIds()[q].ToArray();
			convoyDurabilities[q] = StaticData.getConvoyDurabilities()[q].ToArray();
        }
		savefile = StaticData.savefile;
		chapterPrep = StaticData.chapterPrep;
		positions = StaticData.positions;
	}

	public void unload()
    {
		StaticData.iron = iron;
		StaticData.steel = steel;
		StaticData.silver = silver;
		StaticData.bonusEXP = bonusEXP;

		StaticData.members.Clear();
		for (int q = 0; q < unitName.Length; q++)
        {
			Unit mem = new Unit();
			mem.constructor(unitName[q], UnitClass.unitClassIndex[unitClass[q]], description[q], maxHP[q], strength[q], magic[q],
			skill[q], speed[q], luck[q], defense[q], resistance[q], constitution[q], movement[q],
			hpGrowth[q], strengthGrowth[q], magicGrowth[q], skillGrowth[q], speedGrowth[q], luckGrowth[q],
			defenseGrowth[q], resistanceGrowth[q], personalItemId[q] == -1 ? null : Item.itemIndex[personalItemId[q]].clone(), (Weapon.WeaponType)weaponType[q], proficiency[q],
			Unit.UnitTeam.PLAYER, supportId1[q], supportId2[q], (Unit.Affinity)affinity[q], palette[q]);
			mem.currentHP = currentHP[q];
			mem.level = level[q];
			mem.experience = experience[q];
			if (mem.personalItem != null)
            {
				mem.personalItem.usesLeft = personalItemUsesLeft[q];
            }
			if (heldWeaponId[q] != -1)
            {
				mem.heldWeapon = (Weapon)Item.itemIndex[heldWeaponId[q]].clone();
				mem.heldWeapon.usesLeft = heldWeaponUsesLeft[q];
            }
			if (heldItemId[q] != -1)
            {
				mem.heldItem = Item.itemIndex[heldItemId[q]].clone();
				mem.heldItem.usesLeft = heldItemUsesLeft[q];
            }

			mem.isEssential = isEssential[q];
			mem.isLeader = isLeader[q];
			mem.equipped = equipped[q]; //0 = personal, 1 = held, 2 = none
			mem.isExhausted = isExhausted[q];

			mem.deathQuote = deathQuote[q];

			mem.fusionSkill1 = (Unit.FusionSkill)fusionSkill1[q];
			mem.fusionSkill2 = (Unit.FusionSkill)fusionSkill2[q];
			mem.fusionSkillBonus = (Unit.FusionSkill)fusionSkillBonus[q];

			mem.battles = battles[q];
			mem.wins = wins[q];
			mem.losses = losses[q];

			StaticData.members.Add(mem);
		}

		StaticData.prisoners.Clear();
		for (int q = 0; q < punitName.Length; q++)
		{
			Unit mem = new Unit();
			mem.constructor(punitName[q], UnitClass.unitClassIndex[punitClass[q]], pdescription[q], pmaxHP[q], pstrength[q], pmagic[q],
			pskill[q], pspeed[q], pluck[q], pdefense[q], presistance[q], pconstitution[q], pmovement[q],
			phpGrowth[q], pstrengthGrowth[q], pmagicGrowth[q], pskillGrowth[q], pspeedGrowth[q], pluckGrowth[q],
			pdefenseGrowth[q], presistanceGrowth[q], ppersonalItemId[q] == -1 ? null : Item.itemIndex[ppersonalItemId[q]].clone(), (Weapon.WeaponType)pweaponType[q], pproficiency[q],
			Unit.UnitTeam.ENEMY, psupportId1[q], psupportId2[q], (Unit.Affinity)pAffinity[q], ppalette[q]);
			mem.currentHP = currentHP[q];
			mem.level = plevel[q];
			mem.experience = pexperience[q];
			if (mem.personalItem != null)
			{
				mem.personalItem.usesLeft = ppersonalItemUsesLeft[q];
			}
			if (pheldWeaponId[q] != -1)
			{
				mem.heldWeapon = (Weapon)Item.itemIndex[pheldWeaponId[q]].clone();
				mem.heldWeapon.usesLeft = pheldWeaponUsesLeft[q];
			}
			if (pheldItemId[q] != -1)
			{
				mem.heldItem = Item.itemIndex[pheldItemId[q]].clone();
				mem.heldItem.usesLeft = pheldItemUsesLeft[q];
			}

			mem.isEssential = pisEssential[q];

			mem.isLeader = pisLeader[q];
			mem.equipped = pequipped[q]; //0 = personal, 1 = held, 2 = none
			mem.isExhausted = pisExhausted[q];

			mem.deathQuote = pdeathQuote[q];

			mem.fusionSkill1 = (Unit.FusionSkill)pfusionSkill1[q];
			mem.fusionSkill2 = (Unit.FusionSkill)pfusionSkill2[q];
			mem.fusionSkillBonus = (Unit.FusionSkill)pfusionSkillBonus[q];

			mem.battles = pBattles[q];
			mem.wins = pWins[q];
			mem.losses = pLosses[q];

			StaticData.prisoners.Add(new Gemstone(mem));
		}
		for (int q = 0; q < SupportLog.supportLog.Length; q++)
        {
			SupportLog.supportLog[q].supportAmount = supportAmounts[q];
			SupportLog.supportLog[q].level = (SupportData.SupportLevel)supportLevels[q];
		}
		StaticData.scene = scene;
		if (StaticData.scene == 0)
        {
			StaticData.scene = 1;
        }
		for (int q = 0; q < StaticData.convoyIds.Length; q++)
        {
			StaticData.convoyIds[q] = new List<int>(convoyIds[q]);
			StaticData.convoyDurabilities[q] = new List<int>(convoyDurabilities[q]);
		}
		StaticData.savefile = savefile;
		StaticData.chapterPrep = chapterPrep;
		StaticData.positions = positions;
	}
}
