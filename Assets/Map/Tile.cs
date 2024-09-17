using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Material moveHighlight;
    [SerializeField] private Material attackHighlight;
    [SerializeField] private Material interactHighlight;

    public int x;
    public int y;
    public int height;
    private TileType type;

    private UnitModel occupant;

    private List<Gemstone> gemstones;
    public int ironLoot, steelLoot, silverLoot;
    public Item itemLoot;

    public static float HALF_LENGTH = 0.5f;
    public static float TILE_HEIGHT_MULTIPLIER = 0.25f;

    public static TileType PLAIN = new TileType("PLAIN", 1, 1, 0);
    public static TileType FLOOR = new TileType("FLOOR", 1, 5, 0);
    public static TileType RUBBLE = new TileType("RUBBLE", 2, 1, 0);
    public static TileType PILLAR = new TileType("PILLAR", 2, 6, 20);
    public static TileType WARP_PAD = new TileType("WARP PAD", 2, 3, 0);
    public static TileType DEEP_WATER = new TileType("DEEP WATER", int.MaxValue, 1, 0);
    public static TileType wALL = new TileType("WALL", int.MaxValue, 1, 0);
    public static TileType CHEST = new TileType("CHEST", 1, 4, 0);
    public static TileType SEIZE_POINT = new TileType("SEIZE POINT", 1, 4, 20);
    public static TileType HEAL_TILE = new TileType("HEAL TILE", 1, 1, 20);
    public static TileType TREE = new TileType("TREE", 2, 1, 20);
    public static TileType PEAK = new TileType("PEAK", 7, 1, 40);
    public static TileType MOUNTAIN = new TileType("MOUNTAIN", 4, 1, 30);
    public static TileType CAVE = new TileType("CAVE", 1, 1, 10);
    public static TileType HOUSE = new TileType("HOUSE", 1, 1, 10);
    public static TileType CLIFF = new TileType("CLIFF", int.MaxValue, 1, 0);

    /*
    private void OnEnable()
    {
        draw(0, 0, 1, PLAIN);
    }
    */
    public void draw(int x, int y, int height, TileType type)
    {
        this.x = x;
        this.y = y;
        this.height = height;
        this.type = type;

        float meshHeight = (height + 0.0f) * TILE_HEIGHT_MULTIPLIER;
        getStage().Translate(0, meshHeight, 0);
        getHighlight().Translate(0, meshHeight + 0.05f, 0);

        Mesh mesh = new Mesh
        {
            name = "TileMesh"
        };

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, -HALF_LENGTH));
        vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, HALF_LENGTH));
        vertices.Add(new Vector3(HALF_LENGTH, meshHeight, HALF_LENGTH));
        vertices.Add(new Vector3(HALF_LENGTH, meshHeight, -HALF_LENGTH));

        List<int> triangles = new List<int>(new int[] { 0, 1, 2, 0, 2, 3 });

        List<Vector3> normals = new List<Vector3>();
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);

        if (meshHeight > 0)
        {
            //Front
            vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, -HALF_LENGTH)); //4
            vertices.Add(new Vector3(HALF_LENGTH, meshHeight, -HALF_LENGTH)); //5
            vertices.Add(new Vector3(-HALF_LENGTH, 0, -HALF_LENGTH)); //6
            vertices.Add(new Vector3(HALF_LENGTH, 0, -HALF_LENGTH)); //7

            triangles.AddRange(new int[] { 4, 7, 6, 4, 5, 7 });

            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));

            //Left
            vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, -HALF_LENGTH)); //8
            vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, HALF_LENGTH)); //9
            vertices.Add(new Vector3(-HALF_LENGTH, 0, -HALF_LENGTH)); //10
            vertices.Add(new Vector3(-HALF_LENGTH, 0, HALF_LENGTH)); //11

            triangles.AddRange(new int[] { 8, 10, 9, 9, 10, 11 });

            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));

            //Back
            vertices.Add(new Vector3(-HALF_LENGTH, meshHeight, HALF_LENGTH)); //12
            vertices.Add(new Vector3(HALF_LENGTH, meshHeight, HALF_LENGTH)); //13
            vertices.Add(new Vector3(-HALF_LENGTH, 0, HALF_LENGTH)); //14
            vertices.Add(new Vector3(HALF_LENGTH, 0, HALF_LENGTH)); //15

            triangles.AddRange(new int[] { 12, 14, 13, 13, 14, 15 });

            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));

            //Right
            vertices.Add(new Vector3(HALF_LENGTH, meshHeight, HALF_LENGTH)); //16
            vertices.Add(new Vector3(HALF_LENGTH, meshHeight, -HALF_LENGTH)); //17
            vertices.Add(new Vector3(HALF_LENGTH, 0, HALF_LENGTH)); //18
            vertices.Add(new Vector3(HALF_LENGTH, 0, -HALF_LENGTH)); //19

            triangles.AddRange(new int[] { 16, 18, 17, 17, 18, 19 });

            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void setOccupant(UnitModel occupant)
    {
        this.occupant = occupant;
    }
    public UnitModel getOccupant()
    {
        return occupant;
    }
    public bool isVacant()
    {
        return occupant == null;
    }
    public int getMoveCost(Unit u)
    {
        if (u.isFlying())
        {
            return type.inAirCost;
        }
        return type.onFootCost;
    }
    public string getName()
    {
        return type.tileName;
    }
    public int getCost(bool flying)
    {
        if (flying)
        {
            return type.inAirCost;
        }
        return type.onFootCost;
    }
    public int getAvoidBonus()
    {
        return type.avoidBonus;
    }
    public List<Gemstone> getGemstones()
    {
        return gemstones;
    }
    public bool hasLoot()
    {
        return ironLoot > 0 || steelLoot > 0 || silverLoot > 0 || itemLoot != null;
    }

    public string takeLoot(Unit retriever)
    {
        //You can't have multiple types of loot
        if (ironLoot > 0)
        {
            StaticData.iron += ironLoot;
            string ret = "You got " + ironLoot + " Iron!";
            ironLoot = 0;
            return ret;
        }
        if (steelLoot > 0)
        {
            StaticData.steel += steelLoot;
            string ret = "You got " + steelLoot + " Steel!";
            steelLoot = 0;
            return ret;
        }
        if (silverLoot > 0)
        {
            StaticData.silver += silverLoot;
            string ret = "You got " + silverLoot + " Silver!";
            silverLoot = 0;
            return ret;
        }
        if (itemLoot is Weapon && retriever.heldWeapon == null)
        {
            retriever.heldWeapon = (Weapon)itemLoot.clone();
            string ret = "You got a(n) " + itemLoot.itemName + "!";
            itemLoot = null;
            return ret;
        }
        else if (!(itemLoot is Weapon) && retriever.heldItem == null)
        {
            retriever.heldItem = itemLoot.clone();
            string ret = "You got a(n) " + itemLoot.itemName + "!";
            itemLoot = null;
            return ret;
        }
        else
        {
            StaticData.addToConvoy(itemLoot.clone());
            string ret = "A(n) " + itemLoot.itemName + " was sent to the convoy!";
            itemLoot = null;
            return ret;
        }
    }

    public void decorate(GameObject deco)
    {
        deco.transform.position = getStage().position;
        deco.transform.SetParent(transform);
    }

    public Transform getStage()
    {
        return StaticData.findDeepChild(transform, "Stage");
    }
    public Transform getHighlight()
    {
        return StaticData.findDeepChild(transform, "Highlight");
    }
    public class TileType
    {
        public string tileName;
        public int onFootCost;
        public int inAirCost;
        public int avoidBonus;

        public TileType(string tileName, int onFootCost, int inAirCost, int avoidBonus)
        {
            this.tileName = tileName;
            this.onFootCost = onFootCost;
            this.inAirCost = inAirCost;
            this.avoidBonus = avoidBonus;
        }
    }
}
