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

    [SerializeField] private GameObject lord;
    [SerializeField] private GameObject servant;
    [SerializeField] private GameObject soldier;
    [SerializeField] private GameObject architect;
    [SerializeField] private GameObject diplomat;
    [SerializeField] private GameObject guard;
    [SerializeField] private GameObject priestess;
    [SerializeField] private GameObject pilot;
    [SerializeField] private GameObject elite_quartz;
    [SerializeField] private GameObject topaz_fusion;

    public static GameObject[] modelPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        modelPrefabs = new GameObject[] { lord, servant, soldier, architect, diplomat, guard, priestess, pilot,
                elite_quartz, topaz_fusion };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUnit(Unit unit)
    {
        this.unit = unit;
        model = Instantiate(modelPrefabs[unit.unitClass.id % modelPrefabs.Length], transform);
        anim = model.GetComponent<Animator>();
    }
}
