using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapBattleAnimation : AbstractBattleAnimation
{
    public override void constructor(Battle battle, string musicName, GridMap gridmap)
    {
        this.battle = battle;
        this.gridmap = gridmap;

        gridmap.getCursor().gameObject.SetActive(false);

        //Set Variables
        if (battle.dfd.getUnit().team == Unit.UnitTeam.ENEMY)
        {
            isPlayerAttack = true;

            playerUnit = battle.atk;
            enemyUnit = battle.dfd;
            playerWep = battle.atkWep;
            enemyWep = battle.dfdWep;
            playerTile = battle.atkTile;
            enemyTile = battle.dfdTile;
        }
        else
        {
            isPlayerAttack = false;

            playerUnit = battle.dfd;
            enemyUnit = battle.atk;
            playerWep = battle.dfdWep;
            enemyWep = battle.atkWep;
            playerTile = battle.dfdTile;
            enemyTile = battle.atkTile;
        }
        enemyEXPYield = enemyUnit.getUnit().rawEXPReward();

        playerUnit.playIdle();
        enemyUnit.playIdle();
        
        //Initialize Battle
        StaticData.findDeepChild(transform, "PlayerHP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "PlayerName").GetComponent<TextMeshProUGUI>()
            .text = playerUnit.getUnit().unitName;
        StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
            .text = "" + enemyUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "EnemyName").GetComponent<TextMeshProUGUI>()
            .text = enemyUnit.getUnit().unitName;

        getNextEvent();
    }

    public override void backToGridMap()
    {
        gridmap.gameObject.SetActive(true);
        if (playerUnit != null)
        {
            playerUnit.playIdle();
            if (playerWep != null && playerWep.weaponType == playerUnit.getUnit().weaponType)
            {
                playerUnit.getUnit().proficiency += isPlayerAttack ? battle.atkFinalWepEXP() : battle.dfdFinalWepEXP();
            }
        }
        if (enemyUnit != null)
        {
            enemyUnit.playIdle();
            if (enemyWep != null && enemyWep.weaponType == enemyUnit.getUnit().weaponType)
            {
                enemyUnit.getUnit().proficiency += isPlayerAttack ? battle.dfdFinalWepEXP() : battle.atkFinalWepEXP();
            }
        }

        gridmap.getCursor().gameObject.SetActive(true);

        gridmap.endBattleAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (phase == Phase.BEGIN)
        {
            if (timer <= 0)
            {
                getNextEvent();
            }
        }
        else if (phase == Phase.ACTIVATE_BATTLE_SKILL)
        {
            if (timer <= 0)
            {
                getNextEvent();
            }
        }
        else if (phase == Phase.MOVE)
        {
            if (((Battle.Attack)currentEvent).crit)
            {
                timer = 1;
                currentActor.playIdle();
                gridmap.playOneTimeSound(AssetDictionary.getAudio("crit-activate"));
                ParticleAnimation crit = AssetDictionary.getParticles("crit");
                Instantiate(crit, new Vector3(
                    currentActor.transform.position.x,
                    currentActor.transform.position.y + 1,
                    currentActor.transform.position.z
                    ), crit.transform.rotation);
                phase = Phase.ACTIVATE_CRITICAL;
            }
            else
            {
                timer = currentActor.playAttack(battle.distance);
                if (currentActor.moveAtDistance(battle.distance))
                {
                    phase = Phase.ATTACKANIM;
                }
                else
                {
                    phase = Phase.RANGEANIM;
                }
            }
        }
        else if (phase == Phase.ACTIVATE_CRITICAL)
        {
            if (timer <= 0)
            {
                timer = currentActor.playAttack(battle.distance);
                if (currentActor.moveAtDistance(battle.distance))
                {
                    phase = Phase.ATTACKANIM;
                }
                else
                {
                    phase = Phase.RANGEANIM;
                }
            }
        }
        else if (phase == Phase.ATTACKANIM)
        {
            if (timer <= 0)
            {
                if (((Battle.Attack)currentEvent).hit)
                {
                    timer = target.playGotHit(((Battle.Attack)currentEvent).damage);
                    currentActor.playIdle();
                    updateHPs();

                    if (((Battle.Attack)currentEvent).damage <= 0)
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("parry"));
                    }
                    else if (((Battle.Attack)currentEvent).crit)
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("crit-hit"));
                    }
                    else
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("damage"));
                    }
                }
                else
                {
                    gridmap.playOneTimeSound(AssetDictionary.getAudio("dodge"));
                    timer = target.playDodge();
                    currentActor.playIdle();
                }
                phase = Phase.HITANIM;
            }
        }
        else if (phase == Phase.RANGEANIM)
        {
            //TODO eventually, we'll use projectiles
            if (timer <= 0)
            {
                if (((Battle.Attack)currentEvent).hit)
                {
                    timer = target.playGotHit(((Battle.Attack)currentEvent).damage);
                    currentActor.playIdle();
                    updateHPs();

                    if (((Battle.Attack)currentEvent).damage <= 0)
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("parry"));
                    }
                    else if (((Battle.Attack)currentEvent).crit)
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("crit-hit"));
                    }
                    else
                    {
                        gridmap.playOneTimeSound(AssetDictionary.getAudio("damage"));
                    }
                }
                else
                {
                    gridmap.playOneTimeSound(AssetDictionary.getAudio("dodge"));
                    timer = target.playDodge();
                    currentActor.playIdle();
                }
                phase = Phase.HITANIM;
            }
        }
        else if (phase == Phase.HITANIM)
        {
            if (timer <= 0)
            {
                getNextEvent();
            }
        }
        else if (phase == Phase.ACTIVATE_AFTER_SKILL)
        {
            if (timer <= 0)
            {
                updateHPs();
                phase = Phase.AFTERHP;
            }
        }
        else if (phase == Phase.AFTERHP)
        {
            if (timer <= 0)
            {
                if (battle.onFinalState())
                {
                    timer = 2;
                    if (currentEvent.atkFinalHP <= 0)
                    {
                        if (isPlayerAttack)
                        {
                            poof(playerUnit, false);
                        }
                        else
                        {
                            poof(enemyUnit, false);
                        }

                        phase = Phase.POOF;
                    }
                    else if (currentEvent.dfdFinalHP <= 0)
                    {
                        if (isPlayerAttack)
                        {
                            poof(enemyUnit, false);
                        }
                        else
                        {
                            poof(playerUnit, false);
                        }

                        phase = Phase.POOF;
                    }
                    else
                    {
                        phase = Phase.END;
                    }
                }
                else
                {
                    getNextEvent();
                }
            }
        }
        else if (phase == Phase.POOF)
        {
            if (timer <= 0)
            {
                timer = 2;
                phase = Phase.END;
            }
        }
        else if (phase == Phase.END)
        {
            if (timer <= 0)
            {
                if (playerWep != null)
                {
                    playerWep.usesLeft = isPlayerAttack ? battle.atkFinalWepDurability() : battle.dfdFinalWepDurability();
                }
                if (enemyWep != null)
                {
                    enemyWep.usesLeft = !isPlayerAttack ? battle.atkFinalWepDurability() : battle.dfdFinalWepDurability();
                }
                if (playerUnit != null && playerUnit.getUnit().team == Unit.UnitTeam.PLAYER)
                {
                    setEXPDisplay(false);
                    timer = 1;
                    phase = Phase.EXP;
                }
                else
                {
                    backToGridMap();
                }
            }
        }
        else if (phase == Phase.EXP)
        {
            if (timer <= 0)
            {
                timer = 1;
                levelup = playerUnit.getUnit().addExperience(calculateEXP());
                setEXPDisplay(levelup != null);
                gridmap.playOneTimeSound(AssetDictionary.getAudio("exp"));
                phase = Phase.EXPCHANGE;
            }
        }
        else if (phase == Phase.EXPCHANGE)
        {
            if (timer <= 0)
            {
                if (levelup == null)
                {
                    if (playerWep != null && playerWep.uses > 0 && playerWep.usesLeft <= 0)
                    {
                        setBrokenDisplay();
                        playerUnit.getUnit().breakEquippedWeapon();
                        playerUnit.equip();
                        timer = 3;
                        phase = Phase.BROKENWEP;
                    }
                    else
                    {
                        backToGridMap();
                    }
                }
                else
                {
                    timer = 2;
                    levelUpFanfare();
                    phase = Phase.LEVELUP;
                }
            }
        }
        else if (phase == Phase.LEVELUP)
        {
            if (timer <= 0)
            {
                timer = 4;
                setLevelUpDisplay();
                phase = Phase.LEVELSTATS;
            }
        }
        else if (phase == Phase.LEVELSTATS)
        {
            //nothing
        }
        else if (phase == Phase.BROKENWEP)
        {
            if (timer <= 0)
            {
                backToGridMap();
            }
        }
    }

    public override void getNextEvent()
    {
        currentEvent = battle.getNextEvent();

        if (currentEvent is Battle.InitialStep)
        {
            phase = Phase.BEGIN;
        }
        else if (currentEvent is Battle.ActivationStep)
        {
            Battle.ActivationStep act = (Battle.ActivationStep)currentEvent;
            Unit.FusionSkill playerSkill = isPlayerAttack ? act.atkSkill : act.dfdSkill;
            Unit.FusionSkill enemySkill = isPlayerAttack ? act.dfdSkill : act.atkSkill;
            if (playerSkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner playerSkillExec = FusionSkillExecutioner.SKILL_LIST[(int)playerSkill];
                //TODO play sound (AssetDictionary.getAudio(playerSkillExec.skillName))
            }
            if (enemySkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner enemySkillExec = FusionSkillExecutioner.SKILL_LIST[(int)enemySkill];
                //TODO play sound (AssetDictionary.getAudio(enemySkillExec.skillName))
            }
            phase = Phase.ACTIVATE_BATTLE_SKILL;
        }
        else if (currentEvent is Battle.Attack)
        {
            currentActor = ((isPlayerAttack && currentEvent.isATKAttacking) || !(isPlayerAttack || currentEvent.isATKAttacking))
                ? playerUnit : enemyUnit;
            target = currentActor == playerUnit ? enemyUnit : playerUnit;

            phase = Phase.MOVE;
        }
        else if (currentEvent is Battle.AfterEffect)
        {
            Battle.AfterEffect act = (Battle.AfterEffect)currentEvent;

            Unit.FusionSkill playerSkill = isPlayerAttack ? act.atkSkill : act.dfdSkill;
            Unit.FusionSkill enemySkill = isPlayerAttack ? act.dfdSkill : act.atkSkill;
            if (playerSkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner playerSkillExec = FusionSkillExecutioner.SKILL_LIST[(int)playerSkill];
                //TODO play sound (AssetDictionary.getAudio(playerSkillExec.skillName))
            }
            if (enemySkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner enemySkillExec = FusionSkillExecutioner.SKILL_LIST[(int)enemySkill];
                //TODO play sound (AssetDictionary.getAudio(enemySkillExec.skillName))
            }

            phase = Phase.ACTIVATE_AFTER_SKILL;
        }
    }
}
