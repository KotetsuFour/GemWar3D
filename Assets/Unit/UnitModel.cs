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
                Vector3 direction = new Vector3(currentX - transform.position.x, 0, currentZ - transform.position.z);
                transform.rotation = Quaternion.LookRotation(direction);
                direction = direction.normalized * mapMoveSpeed * Time.deltaTime;
                transform.Translate(direction.x, 0, direction.z);
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
    }
    private void setPalette(List<Color> palette)
    {
        for (int q = 0; q < palette.Count; q++)
        {
            Material mat = StaticData.getMaterialByName(model.GetComponent<SkinnedMeshRenderer>().materials, "Palette" + q);
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
        this.standingRotation = rot;
        transform.rotation = rot;
    }

    public Animator getAnimator()
    {
        return anim;
    }
    public void playIdle()
    {
        anim.Play("Idle");
    }
    public void playMove()
    {
        anim.Play("Running");
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
    }
    public bool reachedDestination()
    {
        return path == null;
    }
}
