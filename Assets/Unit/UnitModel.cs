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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        model = StaticData.findDeepChild(transform, "model").gameObject;
        anim = model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
