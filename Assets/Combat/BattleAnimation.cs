using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleAnimation : MonoBehaviour
{
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

    private GridMap gridmap;
    public void constructor(Battle battle, GridMap gridmap)
    {
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
            isPlayerAttack = false;

            playerUnit = battle.atk;
            enemyUnit = battle.dfd;
            playerWep = battle.atkWep;
            enemyWep = battle.dfdWep;
            playerTile = battle.atkTile;
            enemyTile = battle.dfdTile;
        }
        else
        {
            isPlayerAttack = true;

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
        StaticData.findDeepChild(transform, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
            .text = playerWep.itemName;
        //TODO setimage
        StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
            .sprite = null;
        StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
            .text = "" + enemyUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "EnemyName").GetComponent<TextMeshProUGUI>()
            .text = enemyUnit.getUnit().unitName;
        StaticData.findDeepChild(transform, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
            .text =enemyWep.itemName;
        //TODO set image
        StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
            .sprite = null;

        StaticData.findDeepChild(transform, "PlayerPlatform").GetComponent<MeshRenderer>()
            .material = playerTile.GetComponent<MeshRenderer>().material;
        StaticData.findDeepChild(transform, "EnemyPlatform").GetComponent<MeshRenderer>()
            .material = enemyTile.GetComponent<MeshRenderer>().material;

        Transform playerStart = StaticData.findDeepChild(transform, "PlayerStart");
        Transform enemyStart = StaticData.findDeepChild(transform, "EnemyStart");

        playerUnit.transform.SetLocalPositionAndRotation(playerStart.position, playerStart.rotation);
        enemyUnit.transform.SetLocalPositionAndRotation(enemyStart.position, enemyStart.rotation);
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
        }
        foreach (Unit u in gridmap.enemy)
        {
            u.model.gameObject.SetActive(true);
        }
        foreach (Unit u in gridmap.ally)
        {
            u.model.gameObject.SetActive(true);
        }
        foreach (Unit u in gridmap.other)
        {
            u.model.gameObject.SetActive(true);
        }
        gridmap.getCursor().gameObject.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
