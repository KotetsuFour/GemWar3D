using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitModel : MonoBehaviour
{
    private GameObject model;
    private Animator anim;

    private Unit unit;
    private Tile tile;
    private Quaternion standingRotation;

    private Vector3[] path;
    private int currentDest;
    [SerializeField] private float mapMoveSpeed;
    [SerializeField] private float satisfactoryDistance;

    [SerializeField] private float battleMoveSpeed;

    [SerializeField] private LayerMask tileLayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (model != null)
        {
            model.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 100, 0), Vector3.down, out hit, int.MaxValue, tileLayer))
        {
            transform.position = new Vector3(
                transform.position.x,
                hit.collider.GetComponent<Tile>().height * Tile.TILE_HEIGHT_MULTIPLIER,
                transform.position.z
                );
        }

        if (path != null)
        {
            if ((transform.position - path[currentDest]).magnitude <= satisfactoryDistance)
            {
                if (currentDest == path.Length - 1)
                {
                    playIdle();
                    path = null;
                }
                else
                {
                    currentDest++;
                }
            }
            else
            {
                playMove();
                float currentX = path[currentDest].x;
                float currentZ = path[currentDest].z;
                Vector3 dest = new Vector3(currentX, transform.position.y, currentZ);
                transform.rotation = Quaternion.LookRotation(dest - transform.position);
                Vector3 direction = transform.forward * mapMoveSpeed * Time.deltaTime;
                transform.position += direction;
            }
        }
    }

    public void setUnit(Unit unit)
    {
        this.unit = unit;
        model = Instantiate(AssetDictionary.getModel(unit.unitClass.id), transform);
        unit.model = this;
        anim = model.GetComponent<Animator>();
        setPalette(unit.palette);
        setCircleColor();
        name = unit.unitName;
    }
    public void setCircleColor()
    {
        Material circleMaterial = StaticData.findDeepChild(transform, "TeamCircle").GetComponent<MeshRenderer>().material;
        if (unit.team == Unit.UnitTeam.PLAYER)
        {
            circleMaterial.color = Color.blue;
        }
        else if (unit.team == Unit.UnitTeam.ENEMY)
        {
            circleMaterial.color = Color.red;
        }
        else if (unit.team == Unit.UnitTeam.ALLY)
        {
            circleMaterial.color = Color.green;
        }
        else if (unit.team == Unit.UnitTeam.OTHER)
        {
            circleMaterial.color = Color.yellow;
        }
    }
    public void setCircleColorExhaust()
    {
        Material circleMaterial = StaticData.findDeepChild(transform, "TeamCircle").GetComponent<MeshRenderer>().material;
        circleMaterial.color = Color.grey;
    }
    private void setPalette(List<Color> palette)
    {
        Material[] materials = StaticData.findDeepChild(model.transform, "model")
            .GetComponent<SkinnedMeshRenderer>().materials;
        for (int q = 0; q < palette.Count; q++)
        {
            Material mat = StaticData.getMaterialByName(materials, "Palette" + ((q % materials.Length) + 1));
            mat.color = palette[q];
        }
    }

    public Unit getUnit()
    {
        return unit;
    }
    public void setTile(Tile tile)
    {
        this.tile = tile;
    }
    public Tile getTile()
    {
        return tile;
    }
    public void setStandingRotation(Quaternion rot)
    {
        standingRotation = rot;
        transform.rotation = rot;
    }

    public Animator getAnimator()
    {
        return anim;
    }
    public void playIdle()
    {
        if (unit.getEquippedWeapon() == null)
        {
            anim.Play("Idle");
        }
        else
        {
            string targetAnimation = Weapon.weaponTypeName(unit.getEquippedWeapon().weaponType) + " Idle";
            anim.Play(targetAnimation);
        }
    }
    public void playMove()
    {
        anim.Play("Running");
    }

    //Returns the length of the animation
    public float playAttack(int dist)
    {
        string attAnim = "Fist Attack";
        Weapon wep = unit.getEquippedWeapon();
        if (wep != null)
        {
            if (moveAtDistance(dist) || wep is Whip || wep is Bow || wep is SpecialWeapon)
            {
                attAnim = Weapon.weaponTypeName(unit.getEquippedWeapon().weaponType) + " Attack";
            }
            else
            {
                attAnim = Weapon.weaponTypeName(unit.getEquippedWeapon().weaponType) + " Throw";
            }
        }
        anim.Play(attAnim);
        return 2;
    }
    public float playDodge()
    {
        string dodge = "Dodge";
        anim.Play(dodge);
        return 1;
    }
    public float playGotHit(int damage)
    {
        string gothit = damage > 0 ? "Got Hit" : "Parry";
        anim.Play(gothit);
        return 1;
    }
    public void equip()
    {
        Transform hand = StaticData.findDeepChild(model.transform, "Hand");
        for (int q = 0; q < hand.childCount; q++)
        {
            Destroy(hand.GetChild(q).gameObject);
        }
        hand.DetachChildren();

        Weapon wep = unit.getEquippedWeapon();
        if (wep != null)
        {
            GameObject wepModel = Instantiate(AssetDictionary.getWeapon(wep.itemName));
            wepModel.transform.SetParent(hand);
            wepModel.transform.localPosition = hand.localPosition;
            Vector3 euler = hand.eulerAngles;
            wepModel.transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
        }
        playIdle();
    }

    public float getBattleMoveSpeed()
    {
        return battleMoveSpeed;
    }
    public bool moveAtDistance(int dist)
    {
        Weapon wep = getUnit().getEquippedWeapon();
        if (wep == null)
        {
            return true;
        }
        if (wep is Whip
            || wep is Bow
            || wep is SpecialWeapon)
        {
            return false;
        }
        return dist <= 1;
    }
    public void setPath(Vector3[] path)
    {
        this.path = path;
        this.currentDest = 0;
    }

    public void cancelMovement()
    {
        path = null;
        transform.position = tile.getStage().position;
        transform.rotation = standingRotation;
        playIdle();
    }
    public bool reachedDestination()
    {
        return path == null;
    }
}
