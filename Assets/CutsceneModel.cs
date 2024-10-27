using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class CutsceneModel : MonoBehaviour
{
    private GameObject model;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void setUnit(Unit unit)
    {
        model = Instantiate(AssetDictionary.getModel(unit.unitClass.id), transform);
        anim = model.GetComponent<Animator>();

        setPalette(unit.palette);
    }
    */
    public void setClass(int classId, float[] palette)
    {
        model = Instantiate(AssetDictionary.getModel(classId), transform);

        List<Color> colors = new List<Color>();
        for (int q = 0; q * 3 < palette.Length; q++)
        {
            float r = (q * 3);
            float g = (q * 3) + 1;
            float b = (q * 3) + 2;
            colors.Add(new Color(r, g, b));
        }
        setPalette(colors);
    }
    public void equip(string itemName)
    {
        Transform hand = StaticData.findDeepChild(transform, "Hand");
        Transform leftHand = StaticData.findDeepChild(transform, "LeftHand");
        for (int q = 0; q < hand.childCount; q++)
        {
            Destroy(hand.GetChild(q).gameObject);
        }
        hand.DetachChildren();
        for (int q = 0; q < leftHand.childCount; q++)
        {
            Destroy(leftHand.GetChild(q).gameObject);
        }
        leftHand.DetachChildren();

        for (int q = 0; q < Item.itemIndex.Length; q++)
        {
            if (Item.itemIndex[q].itemName == itemName)
            {
                Item item = Item.itemIndex[q];
                GameObject wepModel = Instantiate(AssetDictionary.getWeapon(itemName));
                Transform parent = item is Bow ? leftHand : hand;
                wepModel.transform.SetParent(parent);
                wepModel.transform.localPosition = parent.localPosition;
                Vector3 euler = parent.eulerAngles;
                wepModel.transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                return;
            }
        }
    }

    private void setPalette(List<Color> palette)
    {
        for (int q = 0; q < palette.Count; q++)
        {
            Material mat = StaticData.getMaterialByName(model.GetComponent<SkinnedMeshRenderer>().materials, "Palette" + q);
            mat.color = palette[q];
        }
    }
    public Animator getAnimator()
    {
        return StaticData.findDeepChild(transform, "animHolder").GetComponent<Animator>();
    }

    public NavMeshAgent getAgent()
    {
        return GetComponent<NavMeshAgent>();
    }

    public void setDestination(Vector3 dest)
    {
        getAgent().SetDestination(dest);
    }
}
