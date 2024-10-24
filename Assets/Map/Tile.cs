using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
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
    private GameObject deco;

    public static float HALF_LENGTH = 0.5f;
    public static float TILE_HEIGHT_MULTIPLIER = 0.25f;
    public static float MAX_DECORATION_HEIGHT = 100;

    private Vector3 cursorPosition;

    public static TileType PLAIN = new TileType("PLAIN", 1, 1, 0, 0);
    public static TileType FLOOR = new TileType("FLOOR", 1, 5, 0, 0);
    public static TileType RUBBLE = new TileType("RUBBLE", 2, 1, 0, 0);
    public static TileType PILLAR = new TileType("PILLAR", 2, 6, 20, 0);
    public static TileType WARP_PAD = new TileType("WARP PAD", 2, 3, 0, 0);
    public static TileType DEEP_WATER = new TileType("DEEP WATER", int.MaxValue, 1, 0, 0);
    public static TileType WALL = new TileType("WALL", int.MaxValue, 1, 0, 0);
    public static TileType CHEST = new TileType("CHEST", 1, 4, 0, 0);
    public static TileType SEIZE_POINT = new TileType("SEIZE POINT", 1, 4, 20, 0);
    public static TileType HEAL_TILE = new TileType("HEAL TILE", 1, 1, 20, 10);
    public static TileType TREE = new TileType("TREE", 2, 1, 20, 0);
    public static TileType PEAK = new TileType("PEAK", 7, 1, 40, 0);
    public static TileType MOUNTAIN = new TileType("MOUNTAIN", 4, 1, 30, 0);
    public static TileType CAVE = new TileType("CAVE", 1, 1, 10, 0);
    public static TileType HOUSE = new TileType("HOUSE", 1, 1, 10, 0);
    public static TileType CLIFF = new TileType("CLIFF", int.MaxValue, 1, 0, 0);

    public void draw(int x, int y, int height, TileType type)
    {
        this.x = x;
        this.y = y;
        this.height = height;
        this.type = type;
        gemstones = new List<Gemstone>();
        name = $"Tile {x},{y}";

        float meshHeight = (height + 0.0f) * TILE_HEIGHT_MULTIPLIER;
        setUtilityPositions(meshHeight);

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

        List<Vector2> uvs = new List<Vector2>();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

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

            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));

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

            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));

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

            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));

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

            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 0));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<BoxCollider>().center = new Vector3(0, meshHeight / 2, 0);
        GetComponent<BoxCollider>().size = new Vector3(HALF_LENGTH * 2, meshHeight + 0.1f, HALF_LENGTH * 2);
    }
    public TileType getType()
    {
        return type;
    }
    public void setOccupant(UnitModel occupant)
    {
        this.occupant = occupant;
        if (occupant != null)
        {
            occupant.setTile(this);
        }
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
    public void updateGemstones()
    {
        if (gemstones.Count > 0)
        {
            Unit u = gemstones[0].unit;
            StaticData.findDeepChild(transform, "Gemstone").gameObject.SetActive(true);
            int paletteIdx = Mathf.Min(1, u.palette.Count - 1);
            if (paletteIdx > 0)
            {
                Color gemColor = u.palette[paletteIdx];
                StaticData.findDeepChild(transform, "Gemstone").GetComponent<MeshRenderer>()
                    .material.color = gemColor;
            }
        }
        else
        {
            StaticData.findDeepChild(transform, "Gemstone").gameObject.SetActive(false);
        }
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
        else if (steelLoot > 0)
        {
            StaticData.steel += steelLoot;
            string ret = "You got " + steelLoot + " Steel!";
            steelLoot = 0;
            return ret;
        }
        else if (silverLoot > 0)
        {
            StaticData.silver += silverLoot;
            string ret = "You got " + silverLoot + " Silver!";
            silverLoot = 0;
            return ret;
        }
        else if (itemLoot is Weapon && retriever.heldWeapon == null)
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
        this.deco = deco;
        deco.transform.position = getStage().position;
        deco.transform.SetParent(transform);
        RaycastHit hit;
        Physics.Raycast(getStage().position + new Vector3(0, MAX_DECORATION_HEIGHT, 0), Vector3.down, out hit, int.MaxValue);
        setUtilityPositions(hit.point.y);
    }

    public GameObject getDeco()
    {
        return deco;
    }

    public Transform getStage()
    {
        return StaticData.findDeepChild(transform, "Stage");
    }
    public Transform getHighlight()
    {
        return StaticData.findDeepChild(transform, "Highlight");
    }
    public Vector3 getCursorPosition()
    {
        return cursorPosition;
    }
    private void setUtilityPositions(float stageHeight)
    {
        getStage().position = new Vector3(getStage().position.x, stageHeight, getStage().position.z);
        StaticData.findDeepChild(transform, "Gemstone").transform.position = getStage().position;
        getHighlight().position = getStage().position + new Vector3(0, 0.05f, 0);
        cursorPosition = getHighlight().position + new Vector3(0, 0.01f, 0);
    }
    public void highlightMove()
    {
        getHighlight().gameObject.SetActive(true);
        getHighlight().GetComponent<MeshRenderer>().material = moveHighlight;
    }
    public void highlightAttack()
    {
        getHighlight().gameObject.SetActive(true);
        getHighlight().GetComponent<MeshRenderer>().material = attackHighlight;
    }
    public void highlightInteract()
    {
        getHighlight().gameObject.SetActive(true);
        getHighlight().GetComponent<MeshRenderer>().material = interactHighlight;
    }
    public void unhighlight()
    {
        getHighlight().gameObject.SetActive(false);
    }
    public class TileType
    {
        public string tileName;
        public int onFootCost;
        public int inAirCost;
        public int avoidBonus;
        public int healing;

        public TileType(string tileName, int onFootCost, int inAirCost, int avoidBonus, int healing)
        {
            this.tileName = tileName;
            this.onFootCost = onFootCost;
            this.inAirCost = inAirCost;
            this.avoidBonus = avoidBonus;
            this.healing = healing;
        }
    }
}
