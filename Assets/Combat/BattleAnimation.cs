using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleAnimation : MonoBehaviour
{
    [SerializeField] private ParticleAnimation poofEffect;

    private UnitModel playerUnit;
    private UnitModel enemyUnit;
    private Weapon playerWep;
    private Weapon enemyWep;
    private Tile playerTile;
    private Tile enemyTile;
    
    private Vector3 playerReturnPos;
    private Vector3 enemyReturnPos;
    private Quaternion playerReturnRot;
    private Quaternion enemyReturnRot;

    private bool isPlayerAttack;

    private UnitModel currentActor;
    private UnitModel target;
    private Vector3 speed;

    private Battle battle;
    private GridMap gridmap;

    private Phase phase;
    private Battle.BattleEvent currentEvent;
    private float timer;

    private bool[] levelup;

    private AudioSource music;
    public void constructor(Battle battle, GridMap gridmap)
    {
        this.battle = battle;
        this.gridmap = gridmap;
        gridmap.gameObject.SetActive(false);
        foreach (Unit u in gridmap.player)
        {
            u.model.gameObject.SetActive(false);
        }
        foreach (Unit u in gridmap.enemy)
        {
            u.model.gameObject.SetActive(false);
        }
        foreach (Unit u in gridmap.ally)
        {
            u.model.gameObject.SetActive(false);
        }
        foreach (Unit u in gridmap.other)
        {
            u.model.gameObject.SetActive(false);
        }
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
        playerReturnPos = playerUnit.transform.position;
        enemyReturnPos = enemyUnit.transform.position;
        playerReturnRot = playerUnit.transform.rotation;
        enemyReturnRot = enemyUnit.transform.rotation;

        playerUnit.gameObject.SetActive(true);
        enemyUnit.gameObject.SetActive(true);
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
            //TODO setimage
            StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = null;
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
            //TODO set image
            StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = null;
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

        music = gridmap.getAudioSource(AssetDictionary.getAudio("battle-music"));
        music.Play();

        getNextEvent();
    }

    private void backToGridMap()
    {
        gridmap.gameObject.SetActive(true);
        if (playerUnit != null)
        {
            playerUnit.transform.SetPositionAndRotation(playerReturnPos, playerReturnRot);
        }
        if (enemyUnit != null)
        {
            enemyUnit.transform.SetPositionAndRotation(enemyReturnPos, enemyReturnRot);
        }

        foreach (Unit u in gridmap.player)
        {
            u.model.gameObject.SetActive(true);
            u.model.playIdle();
        }
        foreach (Unit u in gridmap.enemy)
        {
            u.model.gameObject.SetActive(true);
            u.model.playIdle();
        }
        foreach (Unit u in gridmap.ally)
        {
            u.model.gameObject.SetActive(true);
            u.model.playIdle();
        }
        foreach (Unit u in gridmap.other)
        {
            u.model.gameObject.SetActive(true);
            u.model.playIdle();
        }
        gridmap.getCursor().gameObject.SetActive(true);

        Destroy(music.gameObject);

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
                    timer = target.playGotHit();
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
                    timer = target.playGotHit();
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
                            poof(playerUnit);
                        }
                        else
                        {
                            poof(enemyUnit);
                        }

                        phase = Phase.POOF;
                    }
                    else if (currentEvent.dfdFinalHP <= 0)
                    {
                        setCamera(target);
                        if (isPlayerAttack)
                        {
                            poof(enemyUnit);
                        }
                        else
                        {
                            poof(playerUnit);
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
                if (playerUnit != null && playerUnit.getUnit().team == Unit.UnitTeam.PLAYER)
                {
                    setEXPDisplay(false);
                    timer = 1;
                    phase = Phase.EXP;
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
                    backToGridMap();
                }
                else
                {
                    //TODO
                    phase = Phase.LEVELUP;
                }
            }
        }
        else if (phase == Phase.LEVELUP)
        {
            //TODO
            phase = Phase.LEVELSTATS;
        }
        else if (phase == Phase.LEVELSTATS)
        {
            if (timer <= 0)
            {
                backToGridMap();
            }
        }
    }

    private void getNextEvent()
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

    private void poof(UnitModel unit)
    {
        gridmap.player.Remove(unit.getUnit());
        gridmap.enemy.Remove(unit.getUnit());
        gridmap.ally.Remove(unit.getUnit());
        gridmap.other.Remove(unit.getUnit());
        gridmap.playOneTimeSound(AssetDictionary.getAudio("poof"));
        Instantiate(poofEffect, unit.transform.position, Quaternion.identity);
        Destroy(unit.gameObject);
    }

    private void setEXPDisplay(bool leveledUp)
    {
        StaticData.findDeepChild(transform, "EXPMeter").gameObject.SetActive(true);
        if (leveledUp)
        {
            StaticData.findDeepChild(transform, "LevelBack").GetComponent<Image>()
                .color = Color.green;
            StaticData.findDeepChild(transform, "Level").GetComponent<TextMeshProUGUI>()
                .color = Color.black;
        }
        StaticData.findDeepChild(transform, "Level").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().level;
        StaticData.findDeepChild(transform, "EXPBack").GetComponent<Image>()
            .color = new Color(0, playerUnit.getUnit().experience / 100f, 0);
        StaticData.findDeepChild(transform, "EXP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().experience;
    }

    private int calculateEXP()
    {
        if (enemyUnit == null)
        {
            return 30;
        }
        if (isPlayerAttack)
        {
            if (battle.dfdTookDamage())
            {
                return 10;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (battle.atkTookDamage())
            {
                return 10;
            }
            else
            {
                return 1;
            }
        }
    }

    private void updateHPs()
    {
        playerUnit.getUnit().currentHP = Mathf.Max(0, isPlayerAttack ? currentEvent.atkFinalHP : currentEvent.dfdFinalHP);
        enemyUnit.getUnit().currentHP = Mathf.Max(0, isPlayerAttack ? currentEvent.dfdFinalHP : currentEvent.atkFinalHP);
        StaticData.findDeepChild(transform, "PlayerHP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
            .text = "" + enemyUnit.getUnit().currentHP;
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

    public enum Phase
    {
        BEGIN, ACTIVATE_BATTLE_SKILL, MOVE, ACTIVATE_CRITICAL, ATTACKANIM, RANGEANIM,
        PROJECTILE, HITANIM, ACTIVATE_AFTER_SKILL, AFTERHP, POOF, END, EXP, EXPCHANGE, LEVELUP, LEVELSTATS
    }
}