using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private UnitModel model;

    public string[] tileMap;
    public string[] deployMap;
    public string[] lootMap;
    public string[] heightMap;
    public string[] decoMap;
    public int[] loot;

    public Dictionary<char, GameObject> decoDictionary;

    public int turnsTaken;
    
    public Tile[,] createMap()
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

                if (tileMap[y][x] == '_')
                {
                    toPut.draw(x, y, height, Tile.FLOOR);
                }
                else if (tileMap[y][x] == 'R')
                {
                    toPut.draw(x, y, height, Tile.RUBBLE);
                }
                else if (tileMap[y][x] == '|')
                {
                    toPut.draw(x, y, height, Tile.PILLAR);
                }
                else if (tileMap[y][x] == 'r')
                {
                    toPut.draw(x, y, height, Tile.WARP_PAD);
                }

                if (decoDictionary.ContainsKey(decoMap[y][x]))
                {
                    toPut.decorate(Instantiate(decoDictionary[decoMap[y][x]]));
                }
                ret[x, y] = toPut;
            }
        }


        return ret;
    }

    public void setUnits(Tile[,] map, Unit[] units, Unit.UnitTeam team)
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
                    Transform pos = map[x, y].getStage();
                    UnitModel myUnit = Instantiate(model, new Vector3(pos.position.x, pos.position.y, pos.position.z), Quaternion.identity);
                    myUnit.setUnit(units[idx]);
                    //TODO set rotation
                    idx++;
                }
            }
        }
    }

    public void finalize(List<Unit> playerList)
    {
        StaticData.dealWithGemstones();
        StaticData.refreshUnits();
        StaticData.registerRemainingSupports(playerList, turnsTaken);
        StaticData.scene++;
    }
}
