using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SparringBattleAnimation : AbstractBattleAnimation
{
    private CrystalBase cbase;
    private Vector3 speed;

    public override void constructor(Battle battle, string musicName, GridMap gridmap)
    {
        throw new System.NotImplementedException();
    }
    private AudioSource music;
    public void constructor(Battle battle, string musicName, CrystalBase cbase)
    {
        this.battle = battle;
        this.cbase = cbase;
        cbase.gameObject.SetActive(false);

        //Set Variables
        isPlayerAttack = true;
        playerUnit = battle.atk;
        enemyUnit = battle.dfd;
        playerWep = battle.atkWep;
        enemyWep = battle.dfdWep;
        playerTile = battle.atkTile;
        enemyTile = battle.dfdTile;

        playerUnit.playIdle();
        enemyUnit.playIdle();

        int[] forecast = battle.forecast;

        //Initialize Battle
        if (isPlayerAttack)
        {
            StaticData.findDeepChild(transform, "PlayerATK").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]);
            StaticData.findDeepChild(transform, "EnemyATK").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDMT] - forecast[Battle.ATKDEF]);
            StaticData.findDeepChild(transform, "PlayerHIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKHIT]);
            StaticData.findDeepChild(transform, "EnemyHIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDHIT]);
            StaticData.findDeepChild(transform, "PlayerCRIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKCRIT]);
            StaticData.findDeepChild(transform, "EnemyCRIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDCRIT]);
        }
        else
        {
            StaticData.findDeepChild(transform, "PlayerATK").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDMT] - forecast[Battle.ATKDEF]);
            StaticData.findDeepChild(transform, "EnemyATK").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]);
            StaticData.findDeepChild(transform, "PlayerHIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDHIT]);
            StaticData.findDeepChild(transform, "EnemyHIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKHIT]);
            StaticData.findDeepChild(transform, "PlayerCRIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.DFDCRIT]);
            StaticData.findDeepChild(transform, "EnemyCRIT").GetComponent<TextMeshProUGUI>()
                .text = "" + Mathf.Max(0, forecast[Battle.ATKCRIT]);
        }
        StaticData.findDeepChild(transform, "PlayerHP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "PlayerName").GetComponent<TextMeshProUGUI>()
            .text = playerUnit.getUnit().unitName;
        if (playerWep != null)
        {
            StaticData.findDeepChild(transform, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = playerWep.itemName;
            StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(playerWep.weaponType));
        }
        else
        {
            StaticData.findDeepChild(transform, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = null;
            StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                .color = Color.black;
        }
        StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
            .text = "" + enemyUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "EnemyName").GetComponent<TextMeshProUGUI>()
            .text = enemyUnit.getUnit().unitName;
        if (enemyWep != null)
        {
            StaticData.findDeepChild(transform, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = enemyWep.itemName;
            StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(enemyWep.weaponType));
        }
        else
        {
            StaticData.findDeepChild(transform, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = null;
            StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                .color = Color.black;
        }

        StaticData.findDeepChild(transform, "PlayerPlatform").GetComponent<MeshRenderer>()
            .material = playerTile.GetComponent<MeshRenderer>().material;
        StaticData.findDeepChild(transform, "EnemyPlatform").GetComponent<MeshRenderer>()
            .material = enemyTile.GetComponent<MeshRenderer>().material;

        Transform playerStart = StaticData.findDeepChild(transform, "PlayerStart");
        Transform enemyStart = StaticData.findDeepChild(transform, "EnemyStart");

        playerUnit.transform.SetLocalPositionAndRotation(playerStart.position, playerStart.rotation);
        enemyUnit.transform.SetLocalPositionAndRotation(enemyStart.position, enemyStart.rotation);

        if (musicName != null)
        {
            music = cbase.getAudioSource(AssetDictionary.getAudio(musicName));
            music.loop = true;
            music.Play();
        }

        getNextEvent();
    }

    public override void backToGridMap()
    {
        Destroy(playerUnit.gameObject);
        Destroy(enemyUnit.gameObject);

        cbase.gameObject.SetActive(true);
        cbase.endBattleAnimation();
    }

    public void defeat(UnitModel unit, bool skipping)
    {
        if (!skipping)
        {
            timer = unit.playDefeat();
        }
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
            if (currentActor.moveAtDistance(battle.distance)
                && (currentActor.transform.position - target.transform.position).magnitude > 1)
            {
                setCamera(currentActor);
                currentActor.transform.position += speed * Time.deltaTime;
                currentActor.playMove();
            }
            else if (((Battle.Attack)currentEvent).crit)
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
            setCamera(currentActor);
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
            setCamera(currentActor);
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
            setCamera(currentActor);
            //TODO eventually, we'll use projectiles
            if (timer <= 0)
            {
                if (((Battle.Attack)currentEvent).hit)
                {
                    setCamera(target);
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
                    setCamera(target);
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
                        setCamera(target);
                        if (isPlayerAttack)
                        {
                            defeat(playerUnit, false);
                        }
                        else
                        {
                            defeat(enemyUnit, false);
                        }

                        phase = Phase.POOF;
                    }
                    else if (currentEvent.dfdFinalHP <= 0)
                    {
                        setCamera(target);
                        if (isPlayerAttack)
                        {
                            defeat(enemyUnit, false);
                        }
                        else
                        {
                            defeat(playerUnit, false);
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
            setCamera(playerUnit);
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
            timer = 2;
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
                StaticData.findDeepChild(transform, "PlayerSkill").gameObject.SetActive(true);
                StaticData.findDeepChild(transform, "PlayerSkillName").GetComponent<TextMeshProUGUI>()
                    .text = playerSkillExec.skillName;
                StaticData.findDeepChild(transform, "PlayerSkillIcon").GetComponent<Image>()
                    .sprite = null; //TODO set icon (AssetDictionary.getImage(playerSkillExec.skillName))
                //TODO play sound (AssetDictionary.getAudio(playerSkillExec.skillName))
            }
            if (enemySkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner enemySkillExec = FusionSkillExecutioner.SKILL_LIST[(int)enemySkill];
                StaticData.findDeepChild(transform, "EnemySkill").gameObject.SetActive(true);
                StaticData.findDeepChild(transform, "EnemySkillName").GetComponent<TextMeshProUGUI>()
                    .text = enemySkillExec.skillName;
                StaticData.findDeepChild(transform, "EnemySkillIcon").GetComponent<Image>()
                    .sprite = null; //TODO set icon (AssetDictionary.getImage(enemySkillExec.skillName))
                //TODO play sound (AssetDictionary.getAudio(enemySkillExec.skillName))
            }
            phase = Phase.ACTIVATE_BATTLE_SKILL;
        }
        else if (currentEvent is Battle.Attack)
        {
            currentActor = ((isPlayerAttack && currentEvent.isATKAttacking) || !(isPlayerAttack || currentEvent.isATKAttacking))
                ? playerUnit : enemyUnit;
            target = currentActor == playerUnit ? enemyUnit : playerUnit;

            speed = (target.transform.position - currentActor.transform.position).normalized
                * currentActor.getBattleMoveSpeed();

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
                StaticData.findDeepChild(transform, "PlayerSkill").gameObject.SetActive(true);
                StaticData.findDeepChild(transform, "PlayerSkillName").GetComponent<TextMeshProUGUI>()
                    .text = playerSkillExec.skillName;
                StaticData.findDeepChild(transform, "PlayerSkillIcon").GetComponent<Image>()
                    .sprite = null; //TODO set icon (AssetDictionary.getImage(playerSkillExec.skillName))
                //TODO play sound (AssetDictionary.getAudio(playerSkillExec.skillName))
            }
            else
            {
                StaticData.findDeepChild(transform, "PlayerSkill").gameObject.SetActive(false);
            }
            if (enemySkill != Unit.FusionSkill.LOCKED)
            {
                timer = 1;
                FusionSkillExecutioner enemySkillExec = FusionSkillExecutioner.SKILL_LIST[(int)enemySkill];
                StaticData.findDeepChild(transform, "EnemySkill").gameObject.SetActive(true);
                StaticData.findDeepChild(transform, "EnemySkillName").GetComponent<TextMeshProUGUI>()
                    .text = enemySkillExec.skillName;
                StaticData.findDeepChild(transform, "EnemySkillIcon").GetComponent<Image>()
                    .sprite = null; //TODO set icon (AssetDictionary.getImage(enemySkillExec.skillName))
                //TODO play sound (AssetDictionary.getAudio(enemySkillExec.skillName))
            }
            else
            {
                StaticData.findDeepChild(transform, "EnemySkill").gameObject.SetActive(false);
            }

            phase = Phase.ACTIVATE_AFTER_SKILL;
        }
    }

    private void setCamera(UnitModel camTarget)
    {
        Transform cam = StaticData.findDeepChild(transform, "Camera");

        if (camTarget == playerUnit)
        {
            cam.position = new Vector3(camTarget.transform.position.x - 1.3f,
                camTarget.transform.position.y + 1.5f,
                camTarget.transform.position.z + 1.5f);
        }
        else
        {
            cam.position = new Vector3(camTarget.transform.position.x + 1.3f,
                camTarget.transform.position.y + 1.5f,
                camTarget.transform.position.z + 1.5f);
        }

        Vector3 lookAt = new Vector3(camTarget.transform.position.x, camTarget.transform.position.y + 1, camTarget.transform.position.z);
        cam.rotation = Quaternion.LookRotation(lookAt - cam.position);
    }
}
