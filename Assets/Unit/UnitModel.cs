using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class UnitModel : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject model;
    private Animator anim;

    private Unit unit;
    private Tile tile;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUnit(Unit unit)
    {
        this.unit = unit;
        model = Instantiate(AssetDictionary.getModel(unit.unitClass.id), transform);
        anim = model.GetComponent<Animator>();
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

    public Animator getAnimator()
    {
        return anim;
    }

    public NavMeshAgent getAgent()
    {
        return agent;
    }
}
