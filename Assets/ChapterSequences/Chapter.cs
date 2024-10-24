using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter : MonoBehaviour
{
    [SerializeField] private UnitModel model; //This is the UnitModel.cs class, not the 3D model
                                                //PLEASE DO NOT DELETE
    [SerializeField] private Tile tile;

    [SerializeField] private Background background;
    [SerializeField] private Material[] surroundings;

    public string[] tileMap;
    public string[] deployMap;
    public string[] lootMap;
    public string[] heightMap;
    public string[] decoMap;
    public int[] loot;

    public Dictionary<char, GameObject> decoDictionary;
    public Dictionary<char, Material> materialDictionary;

    public int turnsTaken;

    public static int nextTODOChapter = 3;
    public void handleInput(SequenceMember seqMem)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            seqMem.LEFT_MOUSE();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            seqMem.RIGHT_MOUSE();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            seqMem.UP();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            seqMem.DOWN();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            seqMem.LEFT();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            seqMem.RIGHT();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            seqMem.Z();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            seqMem.X();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            seqMem.A();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            seqMem.S();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            seqMem.ENTER();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            seqMem.ESCAPE();
        }
    }

    public Tile[,] createMap(Transform mapTransform)
    {
        Tile[,] ret = new Tile[tileMap[0].Length, tileMap.Length];

        int lootIdx = 0;
        for (int y = 0; y < tileMap.Length; y++)
        {
            for (int x = 0; x < tileMap[y].Length; x++)
            {
                Tile toPut = Instantiate(tile, new Vector3(x, 0, y), Quaternion.identity);
                if (lootMap[y][x] == 'e')
                {
                    toPut.ironLoot = loot[lootIdx];
                    toPut.steelLoot = loot[lootIdx + 1];
                    toPut.silverLoot = loot[lootIdx + 2];
                    toPut.itemLoot = loot[lootIdx + 3] == -1 ? null : Item.itemIndex[loot[lootIdx + 3]];
                    lootIdx += 4;
                }

                char h = heightMap[y][x];
                int height = 0;
                if (char.IsDigit(h))
                {
                    height = h - '0';
                }
                else if (char.IsUpper(h))
                {
                    height = (h - 'A') + 10;
                }
                else if (char.IsLower(h))
                {
                    height = (h - 'a') + 36;
                }

                char tileId = tileMap[y][x];
                if (tileId == '_')
                {
                    toPut.draw(x, y, height, Tile.FLOOR);
                }
                else if (tileId == 'R')
                {
                    toPut.draw(x, y, height, Tile.RUBBLE);
                }
                else if (tileId == '|')
                {
                    toPut.draw(x, y, height, Tile.PILLAR);
                }
                else if (tileId == 'r')
                {
                    toPut.draw(x, y, height, Tile.WARP_PAD);
                }
                else if (tileId == '~')
                {
                    toPut.draw(x, y, height, Tile.DEEP_WATER);
                }
                else if (tileId == 'T')
                {
                    toPut.draw(x, y, height, Tile.SEIZE_POINT);
                }
                else if (tileId == 'W')
                {
                    toPut.draw(x, y, height, Tile.WALL);
                }
                else if (tileId == 'e')
                {
                    toPut.draw(x, y, height, Tile.CHEST);
                }
                else if (tileId == '+')
                {
                    toPut.draw(x, y, height, Tile.HEAL_TILE);
                }
                else
                {
                    Debug.Log($"Found unidentified tile: '{tileId}'");
                }

                toPut.GetComponent<MeshRenderer>().material = materialDictionary[tileId];

                if (decoDictionary.ContainsKey(decoMap[y][x]))
                {
                    toPut.decorate(Instantiate(decoDictionary[decoMap[y][x]]));
                }

                toPut.transform.SetParent(mapTransform);

                ret[x, y] = toPut;
            }
        }

        for (int q = 0; q < 5; q++)
        {
            Background part = Instantiate(background, mapTransform);
            part.draw(ret.GetLength(0), ret.GetLength(1), q, surroundings[q % surroundings.Length]);
        }


        return ret;
    }

    public void setUnits(Tile[,] map, Unit[] units, Unit.UnitTeam team, Quaternion rotation)
    {
        char spot = team == Unit.UnitTeam.PLAYER ? '*' : team == Unit.UnitTeam.ENEMY ? 'x'
            : team == Unit.UnitTeam.ALLY ? 'o' : '-';
        int idx = 0;
        for (int y = 0; y < deployMap.Length; y++)
        {
            for (int x = 0; x < deployMap[y].Length; x++)
            {
                if (idx == units.Length)
                {
                    return;
                }
                if (deployMap[y][x] == spot)
                {
                    if (units[idx] == null)
                    {
                        idx++;
                        continue;
                    }
                    Transform pos = map[x, y].getStage();
                    UnitModel myUnit = Instantiate(model,
                        new Vector3(pos.position.x, pos.position.y, pos.position.z), Quaternion.identity);
                    myUnit.setUnit(units[idx]);
                    map[x, y].setOccupant(myUnit);
                    myUnit.setStandingRotation(rotation);
                    myUnit.equip();
                    idx++;
                }
            }
        }
    }

    public List<Tile> getPlayerDeploymentTiles(Tile[,] map)
    {
        List<Tile> ret = new List<Tile>();
        for (int y = 0; y < deployMap.Length; y++)
        {
            for (int x = 0; x < deployMap[y].Length; x++)
            {
                if (deployMap[y][x] == '*')
                {
                    ret.Add(map[x, y]);
                }
            }
        }
        return ret;
    }

    public void finalize(List<Unit> playerList)
    {
        StaticData.dealWithGemstones();
        StaticData.refreshUnits();
        StaticData.registerRemainingSupports(playerList, turnsTaken);
        StaticData.scene++;
    }

    public UnitModel getUnitModelPrefab()
    {
        return model;
    }
    public static void goToChapter(int chapter)
    {
        StaticData.scene = chapter;
        if (Mathf.Abs(StaticData.scene) == Chapter.nextTODOChapter)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (StaticData.scene < 0)
        {
            //TODO go to base
        }
        else
        {
            SceneManager.LoadScene("Chapter" + StaticData.scene);
        }
    }
}
