using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
	public int[] forecast;
    private LinkedList<BattleEvent> attacks;
	private AfterEffect finalState;
	private LinkedListNode<BattleEvent> currentEvent;

	public UnitModel atk;
	public UnitModel dfd;
	public Weapon atkWep;
	public Weapon dfdWep;
	public Tile atkTile;
	public Tile dfdTile;

	public int distance;

	public static int DOUBLE_ATTACK_THRESHOLD = 4;

	public static int ATKHP = 0;
	public static int ATKMT = 1;
	public static int ATKDEF = 2;
	public static int ATKHIT = 3;
	public static int ATKCRIT = 4;
	public static int ATKCOUNT = 5;
	public static int DFDHP = 6;
	public static int DFDMT = 7;
	public static int DFDDEF = 8;
	public static int DFDHIT = 9;
	public static int DFDCRIT = 10;
	public static int DFDCOUNT = 11;

	public static List<int> RNGSTORE;
	private static int currentRandom0To99;

	/**
	 * [0] = atkHP
	 * [1] = atkMt
	 * [2] = atkDef
	 * [3] = atkHit
	 * [4] = atkCrit
	 * [5] = atkStandardAttackCount
	 * [6] = dfdHP
	 * [7] = dfdMt
	 * [8] = dfdDef
	 * [9] = dfdHit
	 * [10] = dfdCrit
	 * [11] = dfdStandardAttackCount
     */
	public static int[] getForecast(UnitModel atk, UnitModel dfd,
        List<Unit> atkAllies, List<Unit> dfdAllies,
        Weapon atkWep, Weapon dfdWep, Tile atkTile, Tile dfdTile)
    {
		int distance = Mathf.Abs(atkTile.x - dfdTile.x) + Mathf.Abs(atkTile.y - dfdTile.y);
		Debug.Log($"Distance: {distance}");
		Unit atkUnit = atk.getUnit();
		Unit dfdUnit = dfd.getUnit();

        int[] ret = new int[12];

        //atkHP
        ret[ATKHP] = atkUnit.currentHP;

		//atkMt
		ret[ATKMT] = atkWep != null ? (atkWep.magic ? atkUnit.magic + (atkWep.might * (atkWep.isEffectiveAgainst(dfdUnit) ? 3 : 1))
			: atkUnit.strength + (atkWep.might * (atkWep.isEffectiveAgainst(dfdUnit) ? 3 : 1)))
			: atkUnit.strength;

		//atkDef
		ret[ATKDEF] = dfdWep != null && dfdWep.magic ? atkUnit.resistance + (atkWep is Armor && atkWep.magic ? ((Armor)atkWep).protection : 0)
			: atkUnit.defense + (atkWep != null && atkWep is Armor && !atkWep.magic ? ((Armor)atkWep).protection : 0);

		//atkHit
		ret[ATKHIT] = atkUnit.getBaseAccuracy() + (atkWep == null ? 0 : atkWep.hit)
			+ (atkWep != null && dfdWep != null && atkWep.isAdvantageousAgainst(dfdWep) ? 10 : 0)
			- ((atkWep != null && dfdWep != null && dfdWep.isAdvantageousAgainst(atkWep) ? 10 : 0) + dfdUnit.getBaseAvoidance() + dfdTile.getAvoidBonus());

		//atkCrit
		ret[ATKCRIT] = atkUnit.getBaseCrit() + (atkWep == null ? 0 : atkWep.crit) - dfdUnit.luck;

		//atkStandardAttackCount
		int atkSpeed = atkUnit.speed - (atkWep == null ? 0 : Mathf.Max(0, atkWep.weight - atkUnit.constitution));
		int dfdSpeed = dfdUnit.speed - (dfdWep == null ? 0 : Mathf.Max(0, dfdWep.weight - dfdUnit.constitution));
		ret[ATKCOUNT] = (atkSpeed - dfdSpeed >= DOUBLE_ATTACK_THRESHOLD ? 2 : 1) * (atkWep != null && atkWep.brave ? 2 : 1);

		//dfdHP
		ret[DFDHP] = dfdUnit.currentHP;

		//dfdDef
		ret[DFDDEF] = atkWep != null && atkWep.magic ? dfdUnit.resistance + (dfdWep is Armor && dfdWep.magic ? ((Armor)dfdWep).protection : 0)
			: dfdUnit.defense + (dfdWep != null && dfdWep is Armor && !dfdWep.magic ? ((Armor)dfdWep).protection : 0);

		if ((dfdWep == null && distance == 1)
			|| (dfdWep != null && dfdWep.minRange <= distance && dfdWep.maxRange >= distance))
        {
			//dfdMt
			ret[DFDMT] = dfdWep != null ? (dfdWep.magic ? dfdUnit.magic + (dfdWep.might * (dfdWep.isEffectiveAgainst(atkUnit) ? 3 : 1))
				: dfdUnit.strength + (dfdWep.might * (dfdWep.isEffectiveAgainst(atkUnit) ? 3 : 1)))
				: dfdUnit.strength;

			//dfdHit
			ret[DFDHIT] = dfdUnit.getBaseAccuracy() + (dfdWep == null ? 0 : dfdWep.hit)
			+ (atkWep != null && dfdWep != null && dfdWep.isAdvantageousAgainst(atkWep) ? 10 : 0)
			- ((atkWep != null && dfdWep != null && atkWep.isAdvantageousAgainst(dfdWep) ? 10 : 0) + atkUnit.getBaseAvoidance() + atkTile.getAvoidBonus());

			//dfdCrit
			ret[DFDCRIT] = dfdUnit.getBaseCrit() + (dfdWep == null ? 0 : dfdWep.crit) - atkUnit.luck;

			//dfdStandardAttackCount
			ret[DFDCOUNT] = (dfdSpeed - atkSpeed >= DOUBLE_ATTACK_THRESHOLD ? 2 : 1) * (dfdWep != null && dfdWep.brave ? 2 : 1);

		}
		else
        {
			ret[DFDCOUNT] = 0;
        }
		handleSupports(atkUnit, dfdUnit, atkAllies, dfdAllies, ret);

		return ret;
    }
	public static int[] getSparringForecast(UnitModel atk, UnitModel dfd,
		Weapon atkWep, Weapon dfdWep)
    {
		Unit atkUnit = atk.getUnit();
		Unit dfdUnit = dfd.getUnit();

		int[] ret = new int[12];

		//atkHP
		ret[ATKHP] = atkUnit.currentHP;

		//atkMt
		ret[ATKMT] = atkWep != null ? (atkWep.magic ? atkUnit.magic + (atkWep.might * (atkWep.isEffectiveAgainst(dfdUnit) ? 3 : 1))
			: atkUnit.strength + (atkWep.might * (atkWep.isEffectiveAgainst(dfdUnit) ? 3 : 1)))
			: atkUnit.strength;

		//atkDef
		ret[ATKDEF] = dfdWep != null && dfdWep.magic ? atkUnit.resistance + (atkWep is Armor && atkWep.magic ? ((Armor)atkWep).protection : 0)
			: atkUnit.defense + (atkWep != null && atkWep is Armor && !atkWep.magic ? ((Armor)atkWep).protection : 0);

		//atkHit
		ret[ATKHIT] = atkUnit.getBaseAccuracy() + (atkWep == null ? 0 : atkWep.hit)
			+ (atkWep != null && dfdWep != null && atkWep.isAdvantageousAgainst(dfdWep) ? 10 : 0)
			- ((atkWep != null && dfdWep != null && dfdWep.isAdvantageousAgainst(atkWep) ? 10 : 0) + dfdUnit.getBaseAvoidance());

		//atkCrit
		ret[ATKCRIT] = atkUnit.getBaseCrit() + (atkWep == null ? 0 : atkWep.crit) - dfdUnit.luck;

		//atkStandardAttackCount
		int atkSpeed = atkUnit.speed - (atkWep == null ? 0 : Mathf.Max(0, atkWep.weight - atkUnit.constitution));
		int dfdSpeed = dfdUnit.speed - (dfdWep == null ? 0 : Mathf.Max(0, dfdWep.weight - dfdUnit.constitution));
		ret[ATKCOUNT] = (atkSpeed - dfdSpeed >= DOUBLE_ATTACK_THRESHOLD ? 2 : 1) * (atkWep != null && atkWep.brave ? 2 : 1);

		//dfdHP
		ret[DFDHP] = dfdUnit.currentHP;

		//dfdDef
		ret[DFDDEF] = atkWep != null && atkWep.magic ? dfdUnit.resistance + (dfdWep is Armor && dfdWep.magic ? ((Armor)dfdWep).protection : 0)
			: dfdUnit.defense + (dfdWep != null && dfdWep is Armor && !dfdWep.magic ? ((Armor)dfdWep).protection : 0);

		//dfdMt
		ret[DFDMT] = dfdWep != null ? (dfdWep.magic ? dfdUnit.magic + (dfdWep.might * (dfdWep.isEffectiveAgainst(atkUnit) ? 3 : 1))
			: dfdUnit.strength + (dfdWep.might * (dfdWep.isEffectiveAgainst(atkUnit) ? 3 : 1)))
			: dfdUnit.strength;

		//dfdHit
		ret[DFDHIT] = dfdUnit.getBaseAccuracy() + (dfdWep == null ? 0 : dfdWep.hit)
		+ (atkWep != null && dfdWep != null && dfdWep.isAdvantageousAgainst(atkWep) ? 10 : 0)
		- ((atkWep != null && dfdWep != null && atkWep.isAdvantageousAgainst(dfdWep) ? 10 : 0) + atkUnit.getBaseAvoidance());

		//dfdCrit
		ret[DFDCRIT] = dfdUnit.getBaseCrit() + (dfdWep == null ? 0 : dfdWep.crit) - atkUnit.luck;

		//dfdStandardAttackCount
		ret[DFDCOUNT] = (dfdSpeed - atkSpeed >= DOUBLE_ATTACK_THRESHOLD ? 2 : 1) * (dfdWep != null && dfdWep.brave ? 2 : 1);

		return ret;
	}
	public Battle(UnitModel atk, UnitModel dfd,
        List<Unit> atkAllies, List<Unit> dfdAllies,
        Weapon atkWep, Weapon dfdWep, Tile atkTile, Tile dfdTile)
    {
		this.atk = atk;
		this.dfd = dfd;
		this.atkWep = atkWep;
		this.dfdWep = dfdWep;
		this.atkTile = atkTile;
		this.dfdTile = dfdTile;

		forecast = getForecast(atk, dfd, atkAllies, dfdAllies, atkWep, dfdWep, atkTile, dfdTile);
		distance = Mathf.Abs(atkTile.x - dfdTile.x) + Mathf.Abs(atkTile.y - dfdTile.y);
		currentRandom0To99 = 0;

		Unit atkUnit = atk.getUnit();
		Unit dfdUnit = dfd.getUnit();

		int atkCount = forecast[ATKCOUNT];
		int dfdCount = forecast[DFDCOUNT];

		attacks = new LinkedList<BattleEvent>();
		attacks.AddFirst(new InitialStep(forecast, atkWep, dfdWep));

		if (forecast[DFDCOUNT] > 0
			&& FusionSkillExecutioner.activateVantage(dfdUnit)
			&& !FusionSkillExecutioner.activateVantage(atkUnit))
        {
			int initial = Mathf.Max(1, dfdCount / 2);
			for (int q = 0; q < initial; q++)
            {
				if (attacks.Last.Value.dfdFinalWepDurability > 0)
                {
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
				}
			}
			for (int q = 0; q < atkCount; q++)
            {
				if (attacks.Last.Value.atkFinalWepDurability > 0)
				{
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
				}
			}
			for (int q = 0; q < dfdCount / 2; q++)
			{
				if (attacks.Last.Value.dfdFinalWepDurability > 0)
				{
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
				}
			}
		}
		else
        {
			int initial = Mathf.Max(1, atkCount / 2);
			for (int q = 0; q < initial; q++)
			{
				if (attacks.Last.Value.atkFinalWepDurability > 0)
				{
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
				}
			}
			for (int q = 0; q < dfdCount; q++)
			{
				if (attacks.Last.Value.dfdFinalWepDurability > 0)
				{
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
				}
			}
			for (int q = 0; q < atkCount / 2; q++)
			{
				if (attacks.Last.Value.atkFinalWepDurability > 0)
				{
					ActivationStep act = new ActivationStep(attacks.Last.Value);
					attacks.AddLast(act);
					act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
				}
			}
		}

		finalState = (AfterEffect)attacks.Last.Value;

		LinkedListNode<BattleEvent> current = attacks.First;
		while (current != null)
        {
			if (current.Value is AfterEffect && (current.Value.atkFinalHP <= 0 || current.Value.dfdFinalHP <= 0))
            {
				finalState = (AfterEffect)current.Value;
				break;
            }
			current = current.Next;
        }
		/*
		foreach (BattleEvent part in attacks)
        {
			Debug.Log($"ATK: {part.atkFinalHP}, DFD: {part.dfdFinalHP}");
        }
		*/
	}
	public Battle()
    {

    }
	public class SparBattle : Battle
    {
		public SparBattle(UnitModel atk, UnitModel dfd, Weapon atkWep, Weapon dfdWep)
        {
			forecast = getSparringForecast(atk, dfd, atkWep, dfdWep);
			currentRandom0To99 = 0;

			Unit atkUnit = atk.getUnit();
			Unit dfdUnit = dfd.getUnit();

			int atkCount = forecast[ATKCOUNT];
			int dfdCount = forecast[DFDCOUNT];

			attacks = new LinkedList<BattleEvent>();
			attacks.AddFirst(new InitialStep(forecast, atkWep, dfdWep));
			for (int round = 0;
				round < 10 && attacks.Last.Value.atkFinalHP > 0 && attacks.Last.Value.dfdFinalHP > 0;
				round++)
            {
				if (forecast[DFDCOUNT] > 0
					&& FusionSkillExecutioner.activateVantage(dfdUnit)
					&& !FusionSkillExecutioner.activateVantage(atkUnit))
				{
					int initial = Mathf.Max(1, dfdCount / 2);
					for (int q = 0; q < initial; q++)
					{
						if (attacks.Last.Value.dfdFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
						}
					}
					for (int q = 0; q < atkCount; q++)
					{
						if (attacks.Last.Value.atkFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
						}
					}
					for (int q = 0; q < dfdCount / 2; q++)
					{
						if (attacks.Last.Value.dfdFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
						}
					}
				}
				else
				{
					int initial = Mathf.Max(1, atkCount / 2);
					for (int q = 0; q < initial; q++)
					{
						if (attacks.Last.Value.atkFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
						}
					}
					for (int q = 0; q < dfdCount; q++)
					{
						if (attacks.Last.Value.dfdFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, false);
						}
					}
					for (int q = 0; q < atkCount / 2; q++)
					{
						if (attacks.Last.Value.atkFinalWepDurability > 0)
						{
							ActivationStep act = new ActivationStep(attacks.Last.Value);
							attacks.AddLast(act);
							act.execute(attacks, forecast, atkUnit, dfdUnit, true, true, true);
						}
					}
				}
			}
			finalState = (AfterEffect)attacks.Last.Value;

			LinkedListNode<BattleEvent> current = attacks.First;
			while (current != null)
			{
				if (current.Value is AfterEffect && (current.Value.atkFinalHP <= 0 || current.Value.dfdFinalHP <= 0))
				{
					finalState = (AfterEffect)current.Value;
					break;
				}
				current = current.Next;
			}
		}
	}
	public int getATKInitialHP()
    {
		return attacks.First.Value.atkInitialHP;
    }
	public int getDFDInitialHP()
	{
		return attacks.First.Value.dfdInitialHP;
	}
	public int getATKFinalHP()
    {
		return finalState.atkFinalHP;
    }
	public int getDFDFinalHP()
    {
		return finalState.dfdFinalHP;
    }
	public bool atkTookDamage()
    {
		foreach (BattleEvent be in attacks)
        {
			if (be.atkFinalHP < be.atkInitialHP)
            {
				return true;
            }
        }
		return false;
    }
	public bool dfdTookDamage()
	{
		foreach (BattleEvent be in attacks)
		{
			if (be.dfdFinalHP < be.dfdInitialHP)
			{
				return true;
			}
		}
		return false;
	}
	public int atkFinalWepDurability()
    {
		return finalState.atkFinalWepDurability;
    }
	public int dfdFinalWepDurability()
	{
		return finalState.dfdFinalWepDurability;
	}
	public int atkFinalWepEXP()
    {
		return finalState.atkFinalWepEXP;
    }
	public int dfdFinalWepEXP()
    {
		return finalState.dfdFinalWepEXP;
    }
	public class BattleEvent
    {
		public int atkInitialHP;
		public int dfdInitialHP;
		public int atkFinalHP;
		public int dfdFinalHP;
		public int atkFinalWepDurability;
		public int dfdFinalWepDurability;
		public bool isATKAttacking;
		public int atkFinalWepEXP;
		public int dfdFinalWepEXP;
	}

	public class Attack : BattleEvent
    {
		public bool hit;
		public bool crit;
		public int damage;
		public Attack(ActivationStep act, int[] forecast, bool atkTurn)
        {
			atkInitialHP = act.atkFinalHP;
			dfdInitialHP = act.dfdFinalHP;
			atkFinalHP = atkInitialHP;
			dfdFinalHP = dfdInitialHP;
			atkFinalWepDurability = act.atkFinalWepDurability;
			dfdFinalWepDurability = act.dfdFinalWepDurability;
			isATKAttacking = atkTurn;
			atkFinalWepEXP = act.atkFinalWepEXP;
			dfdFinalWepEXP = act.dfdFinalWepEXP;
			if (atkTurn)
            {
				int might = forecast[ATKMT];
				int defense = forecast[DFDDEF];
				int hitChance = forecast[ATKHIT];
				int critChance = forecast[ATKCRIT];

				if (act.atkResult != null)
                {
					might += act.atkResult[CombatSkill.EXTRAMT];
					defense += act.atkResult[CombatSkill.EXTRADEF];
					hitChance += act.atkResult[CombatSkill.EXTRAHIT];
					critChance += act.atkResult[CombatSkill.EXTRACRIT];
				}

				if (act.dfdResult != null)
                {
					might += act.dfdResult[CombatSkill.EXTRAMT];
					defense += act.dfdResult[CombatSkill.EXTRADEF];
					hitChance += act.dfdResult[CombatSkill.EXTRAHIT];
					critChance += act.dfdResult[CombatSkill.EXTRACRIT];
				}

				damage = Mathf.Max(0, might - defense);

				if (trueHit() < hitChance)
                {
					hit = true;
					if (randomNumber0To99() < critChance)
                    {
						crit = true;
						damage *= 3;
                    }

					dfdFinalHP -= damage;
					atkFinalWepDurability--;
					atkFinalWepEXP++;
                }
			}
			else
            {
				int might = forecast[DFDMT];
				int defense = forecast[ATKDEF];
				int hitChance = forecast[DFDHIT];
				int critChance = forecast[DFDCRIT];

				if (act.atkResult != null)
				{
					might += act.atkResult[CombatSkill.EXTRAMT];
					defense += act.atkResult[CombatSkill.EXTRADEF];
					hitChance += act.atkResult[CombatSkill.EXTRAHIT];
					critChance += act.atkResult[CombatSkill.EXTRACRIT];
				}

				if (act.dfdResult != null)
				{
					might += act.dfdResult[CombatSkill.EXTRAMT];
					defense += act.dfdResult[CombatSkill.EXTRADEF];
					hitChance += act.dfdResult[CombatSkill.EXTRAHIT];
					critChance += act.dfdResult[CombatSkill.EXTRACRIT];
				}

				damage = Mathf.Max(0, might - defense);

				if (trueHit() < hitChance)
				{
					hit = true;
					if (randomNumber0To99() < critChance)
					{
						crit = true;
						damage *= 3;
					}

					atkFinalHP -= damage;
					dfdFinalWepDurability--;
					dfdFinalWepEXP++;
				}
			}
		}
    }

    public class AfterEffect : BattleEvent
    {
		private Attack prev;
		public Unit.FusionSkill atkSkill;
		public Unit.FusionSkill dfdSkill;
		public AfterEffect (Attack prev)
        {
			this.prev = prev;
			atkInitialHP = prev.atkFinalHP;
			dfdInitialHP = prev.dfdFinalHP;
			atkFinalHP = atkInitialHP;
			dfdFinalHP = dfdInitialHP;
			atkFinalWepDurability = prev.atkFinalWepDurability;
			dfdFinalWepDurability = prev.dfdFinalWepDurability;
			isATKAttacking = prev.isATKAttacking;
			atkFinalWepEXP = prev.atkFinalWepEXP;
			dfdFinalWepEXP = prev.dfdFinalWepEXP;
		}

		public void execute(Unit atk, Unit dfd, bool atkTurn)
        {
			int[] atkResult = null;
			int[] dfdResult = null;

			if (FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkillBonus] is AfterAttackSkill)
			{
				atkResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkillBonus])
					.skillEffect(atk, dfd, prev.hit, prev.damage, atkTurn);
				if (atkResult != null)
				{
					atkSkill = atk.fusionSkillBonus;
				}
			}
			if (atkResult == null && FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill1] is AfterAttackSkill)
			{
				atkResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill1])
					.skillEffect(atk, dfd, prev.hit, prev.damage, atkTurn);
				if (atkResult != null)
				{
					atkSkill = atk.fusionSkill1;
				}
			}
			if (atkResult == null && FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill2] is AfterAttackSkill)
			{
				atkResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill2])
					.skillEffect(atk, dfd, prev.hit, prev.damage, atkTurn);
				if (atkResult != null)
				{
					atkSkill = atk.fusionSkill2;
				}
			}

			if (FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkillBonus] is AfterAttackSkill)
			{
				dfdResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkillBonus])
					.skillEffect(dfd, atk, prev.hit, prev.damage, !atkTurn);
				if (dfdResult != null)
				{
					dfdSkill = dfd.fusionSkillBonus;
				}
			}
			if (dfdResult == null && FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill1] is AfterAttackSkill)
			{
				dfdResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill1])
					.skillEffect(dfd, atk, prev.hit, prev.damage, !atkTurn);
				if (dfdResult != null)
				{
					dfdSkill = dfd.fusionSkill1;
				}
			}
			if (dfdResult == null && FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill2] is AfterAttackSkill)
			{
				dfdResult = ((AfterAttackSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill2])
					.skillEffect(dfd, atk, prev.hit, prev.damage, !atkTurn);
				if (dfdResult != null)
				{
					dfdSkill = dfd.fusionSkill2;
				}
			}

			if (atkResult != null)
            {
				atkFinalHP += atkResult[AfterAttackSkill.MYHPCHANGE];
				dfdFinalHP += atkResult[AfterAttackSkill.YOURHPCHANGE];
            }
			if (dfdResult != null)
			{
				dfdFinalHP += dfdResult[AfterAttackSkill.MYHPCHANGE];
				atkFinalHP += dfdResult[AfterAttackSkill.YOURHPCHANGE];
			}

		}
	}

	public class ActivationStep : BattleEvent
    {
		public Unit.FusionSkill atkSkill;
		public Unit.FusionSkill dfdSkill;
		public int[] atkResult = null;
		public int[] dfdResult = null;

		public ActivationStep(BattleEvent prev)
        {
			atkInitialHP = prev.atkFinalHP;
			dfdInitialHP = prev.dfdFinalHP;
			atkFinalHP = atkInitialHP;
			dfdFinalHP = dfdInitialHP;
			atkFinalWepDurability = prev.atkFinalWepDurability;
			dfdFinalWepDurability = prev.dfdFinalWepDurability;
			atkFinalWepEXP = prev.atkFinalWepEXP;
			dfdFinalWepEXP = prev.dfdFinalWepEXP;
		}

		public void execute(LinkedList<BattleEvent> events, int[] forecast, Unit atk, Unit dfd,
			bool atkActs, bool dfdActs, bool atkTurn)
        {
			isATKAttacking = atkTurn;
			if (atkActs)
            {
				if (FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkillBonus] is CombatSkill)
				{
					atkResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkillBonus])
						.tryActivate(atk, dfd, atkTurn);
					if (atkResult != null)
					{
						atkSkill = atk.fusionSkillBonus;
					}
				}
				if (atkResult == null && FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill1] is CombatSkill)
				{
					atkResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill1])
						.tryActivate(atk, dfd, atkTurn);
					if (atkResult != null)
					{
						atkSkill = atk.fusionSkill1;
					}
				}
				if (atkResult == null && FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill2] is CombatSkill)
				{
					atkResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)atk.fusionSkill2])
						.tryActivate(atk, dfd, atkTurn);
					if (atkResult != null)
					{
						atkSkill = atk.fusionSkill2;
					}
				}
			}

			if (dfdActs)
            {
				if (FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkillBonus] is CombatSkill)
				{
					dfdResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkillBonus])
						.tryActivate(dfd, atk, !atkTurn);
					if (dfdResult != null)
					{
						dfdSkill = dfd.fusionSkillBonus;
					}
				}
				if (dfdResult == null && FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill1] is CombatSkill)
				{
					dfdResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill1])
						.tryActivate(dfd, atk, !atkTurn);
					if (dfdResult != null)
					{
						dfdSkill = dfd.fusionSkill1;
					}
				}
				if (dfdResult == null && FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill2] is CombatSkill)
				{
					dfdResult = ((CombatSkill)FusionSkillExecutioner.SKILL_LIST[(int)dfd.fusionSkill2])
						.tryActivate(dfd, atk, !atkTurn);
					if (dfdResult != null)
					{
						dfdSkill = dfd.fusionSkill2;
					}
				}
			}

			//Main attack
			Attack attack = new Attack(this, forecast, atkTurn);
			events.AddLast(attack);

			AfterEffect afterEffect = new AfterEffect(attack);
			events.AddLast(afterEffect);
			afterEffect.execute(atk, dfd, atkTurn);

			//Extra attacks
			int numExtraAttacks = atkTurn ? (atkResult != null ? atkResult[CombatSkill.EXTRACOUNT] : 0)
				: (dfdResult != null ? dfdResult[CombatSkill.EXTRACOUNT] : 0);

			for (int q = 0; q < numExtraAttacks; q++)
            {
				ActivationStep activation = new ActivationStep(events.Last.Value);
				events.AddLast(activation);
				activation.execute(events, forecast, atk, dfd, !atkTurn, atkTurn,
					atkTurn);
			}
		}
	}
	public class InitialStep : BattleEvent
    {
		public InitialStep(int[] forecast, Weapon atkWep, Weapon dfdWep)
		{
			atkInitialHP = forecast[ATKHP];
			dfdInitialHP = forecast[DFDHP];
			atkFinalHP = atkInitialHP;
			dfdFinalHP = dfdInitialHP;
			atkFinalWepDurability = atkWep == null || atkWep.uses == -1 ? int.MaxValue : atkWep.usesLeft;
			dfdFinalWepDurability = dfdWep == null || dfdWep.uses == -1 ? int.MaxValue : dfdWep.usesLeft;
		}
	}

	public BattleEvent getNextEvent()
    {
		if (currentEvent == null)
        {
			currentEvent = attacks.First;
        }
		else
        {
			currentEvent = currentEvent.Next;
        }
		return currentEvent.Value;
    }
	public bool onFinalState()
    {
		return currentEvent == null || currentEvent.Value == finalState;
    }

	private static void handleSupports(Unit atk, Unit dfd,
	List<Unit> atkAllies, List<Unit> dfdAllies, int[] forecast)
	{
		Unit[] atkPartners = StaticData.findLivingSupportPartners(atk);
		if (atkAllies.Contains(atkPartners[0]))
		{
			int[] buffs = getSupportBuffs(atk, atkPartners[0], atk.supportId1);
			applySupportBuffs(buffs, forecast, true);
		}
		if (atkAllies.Contains(atkPartners[1]))
		{
			int[] buffs = getSupportBuffs(atk, atkPartners[1], atk.supportId2);
			applySupportBuffs(buffs, forecast, true);
		}

		if (atk.isFusion())
		{
			int[] buffs = getFusionBuffs(atk);
			applySupportBuffs(buffs, forecast, true);
		}

		Unit[] dfdPartners = StaticData.findLivingSupportPartners(dfd);
		if (dfdAllies.Contains(dfdPartners[0]))
		{
			int[] buffs = getSupportBuffs(dfd, dfdPartners[0], dfd.supportId1);
			applySupportBuffs(buffs, forecast, false);
		}
		if (dfdAllies.Contains(dfdPartners[1]))
		{
			int[] buffs = getSupportBuffs(dfd, dfdPartners[1], dfd.supportId2);
			applySupportBuffs(buffs, forecast, false);
		}
		if (dfd.isFusion())
		{
			int[] buffs = getFusionBuffs(dfd);
			applySupportBuffs(buffs, forecast, false);
		}
	}

	/*
	 * [0] = ATK
	 * [1] = DEF
	 * [2] = HIT
	 * [3] = AVO
	 * [4] = CRT
	 * [5] = SEC
	 */
	private static int[] getSupportBuffs(Unit partner1, Unit partner2, int supportIdx)
	{
		int multiplier = Mathf.Min(3, (int)SupportLog.supportLog[supportIdx].level);
		float[] buffsHalf1 = null;
		float[] buffsHalf2 = null;
		if (partner1.affinity == Unit.Affinity.FIRE)
		{
			buffsHalf1 = new float[] { 0.5f, 0, 2.5f, 2.5f, 2.5f, 0 };
		}
		else if (partner1.affinity == Unit.Affinity.WATER)
		{
			buffsHalf1 = new float[] { 0.5f, 0.5f, 0, 0, 2.5f, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.EARTH)
		{
			buffsHalf1 = new float[] { 0, 0, 0, 5f, 2.5f, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.WIND)
		{
			buffsHalf1 = new float[] { 0.5f, 0, 2.5f, 0, 2.5f, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.LIGHTNING)
		{
			buffsHalf1 = new float[] { 0, 0.5f, 0, 2.5f, 2.5f, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.ICE)
		{
			buffsHalf1 = new float[] { 0, 0.5f, 2.5f, 2.5f, 0, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.ANIMA)
		{
			buffsHalf1 = new float[] { 0.5f, 0.5f, 0, 2.5f, 0, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.LIGHT)
		{
			buffsHalf1 = new float[] { 0.5f, 0.5f, 2.5f, 0, 2.5f, 0 };
		}
		else if (partner1.affinity == Unit.Affinity.DARK)
		{
			buffsHalf1 = new float[] { 0, 0, 2.5f, 2.5f, 2.5f, 2.5f };
		}
		else if (partner1.affinity == Unit.Affinity.HEAVEN)
		{
			buffsHalf1 = new float[] { 0.5f, 0.5f, 5f, 0, 0, 0 };
		}

		if (partner2.affinity == Unit.Affinity.FIRE)
		{
			buffsHalf2 = new float[] { 0.5f, 0, 2.5f, 2.5f, 2.5f, 0 };
		}
		else if (partner2.affinity == Unit.Affinity.WATER)
		{
			buffsHalf2 = new float[] { 0.5f, 0.5f, 0, 0, 2.5f, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.EARTH)
		{
			buffsHalf2 = new float[] { 0, 0, 0, 5f, 2.5f, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.WIND)
		{
			buffsHalf2 = new float[] { 0.5f, 0, 2.5f, 0, 2.5f, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.LIGHTNING)
		{
			buffsHalf2 = new float[] { 0, 0.5f, 0, 2.5f, 2.5f, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.ICE)
		{
			buffsHalf2 = new float[] { 0, 0.5f, 2.5f, 2.5f, 0, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.ANIMA)
		{
			buffsHalf2 = new float[] { 0.5f, 0.5f, 0, 2.5f, 0, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.LIGHT)
		{
			buffsHalf2 = new float[] { 0.5f, 0.5f, 2.5f, 0, 2.5f, 0 };
		}
		else if (partner2.affinity == Unit.Affinity.DARK)
		{
			buffsHalf2 = new float[] { 0, 0, 2.5f, 2.5f, 2.5f, 2.5f };
		}
		else if (partner2.affinity == Unit.Affinity.HEAVEN)
		{
			buffsHalf2 = new float[] { 0.5f, 0.5f, 5f, 0, 0, 0 };
		}

		float[] buffsTotal = new float[buffsHalf1.Length];
		for (int q = 0; q < buffsTotal.Length; q++)
		{
			buffsTotal[q] = (buffsHalf1[q] + buffsHalf2[q]) * multiplier;
		}

		int[] ret = new int[buffsTotal.Length];
		for (int q = 0; q < ret.Length; q++)
		{
			ret[q] = Mathf.FloorToInt(buffsTotal[q]);
		}

		return ret;
	}

	private static int[] getFusionBuffs(Unit fusion)
	{
		if (fusion.affinity == Unit.Affinity.FIRE)
		{
			return new int[] { 1, 0, 2, 2, 2, 0 };
		}
		else if (fusion.affinity == Unit.Affinity.WATER)
		{
			return new int[] { 1, 1, 0, 0, 2, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.EARTH)
		{
			return new int[] { 0, 0, 0, 5, 2, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.WIND)
		{
			return new int[] { 1, 0, 2, 0, 2, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.LIGHTNING)
		{
			return new int[] { 0, 1, 0, 2, 2, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.ICE)
		{
			return new int[] { 0, 1, 2, 2, 0, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.ANIMA)
		{
			return new int[] { 1, 1, 0, 2, 0, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.LIGHT)
		{
			return new int[] { 1, 1, 2, 0, 2, 0 };
		}
		else if (fusion.affinity == Unit.Affinity.DARK)
		{
			return new int[] { 0, 0, 2, 2, 2, 2 };
		}
		else if (fusion.affinity == Unit.Affinity.HEAVEN)
		{
			return new int[] { 1, 1, 5, 0, 0, 0 };
		}
		return null;
	}

	private static void applySupportBuffs(int[] buffs, int[] forecast, bool atk)
	{
		if (atk)
		{
			forecast[ATKMT] += buffs[0]; //ATK
			forecast[ATKDEF] -= buffs[1]; //DEF
			forecast[ATKHIT] += buffs[2]; //HIT
			forecast[DFDHIT] -= buffs[3]; //AVO
			forecast[ATKCRIT] += buffs[4]; //CRIT
			forecast[DFDCRIT] -= buffs[5]; //SEC
		}
		else
		{
			forecast[DFDMT] += buffs[0]; //ATK
			forecast[DFDDEF] -= buffs[1]; //DEF
			forecast[DFDHIT] += buffs[2]; //HIT
			forecast[ATKHIT] -= buffs[3]; //AVO
			forecast[DFDCRIT] += buffs[4]; //CRIT
			forecast[ATKCRIT] -= buffs[5]; //SEC
		}
	}

	private static int randomNumber0To99()
    {
		if (RNGSTORE == null)
        {
			RNGSTORE = new List<int>();
        }
		while (RNGSTORE.Count < currentRandom0To99 + 1)
        {
			RNGSTORE.Add(Random.Range(0, 100));
        }
		int ret = RNGSTORE[currentRandom0To99];
		currentRandom0To99++;
		return ret;
    }
	private static int trueHit()
	{
		return (randomNumber0To99() + randomNumber0To99()) / 2;
	}
}
