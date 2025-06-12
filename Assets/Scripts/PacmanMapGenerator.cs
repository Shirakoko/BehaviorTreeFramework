using UnityEngine;
using System.Collections.Generic;

public class PacmanMapGenerator : MonoBehaviour
{
    [Header("地图预制体")]
    [SerializeField] private GameObject wallPrefab;        // 墙壁预制体
    [SerializeField] private GameObject dotPrefab;         // 普通豆子预制体
    [SerializeField] private GameObject powerDotPrefab;    // 能量豆预制体
    [SerializeField] private GameObject emptyPrefab;       // 空白区域预制体

    [Header("地图设置")]
    [SerializeField] private int mapWidth = 28;            // 地图宽度
    [SerializeField] private int mapHeight = 31;           // 地图高度
    [SerializeField] private float cellSize = 1f;          // 每个格子的大小

    [Header("地图原点偏移")]
    private Vector2 _mapOriginOffset = new Vector2(-19.5f, -19.5f);
    public Vector2 mapOriginOffset => _mapOriginOffset;

    [Header("豆子生成设置")]
    [SerializeField] private int initialDotCount = 4; // 初始的豆子个数
    [SerializeField] private float powerDotChance = 0.5f;  // 生成能量豆的概率

    // 地图元素类型枚举
    public enum MapElement
    {
        Empty,      // 空白区域
        Wall,       // 墙壁
        Dot,        // 普通豆子
        PowerDot    // 能量豆
    }

    private MapElement[,] mapData;    // 存储地图数据
    private GameObject[,] mapObjects; // 存储地图上的游戏对象
    private HashSet<Vector2Int> emptyPositions = new HashSet<Vector2Int>();  // 存储所有空位置

    // 获取地图生成器的单例
    public static PacmanMapGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMap();
        GenerateMap();
    }

    private void InitializeMap()
    {
        _mapOriginOffset = new Vector2(-mapWidth / 2 + cellSize / 2, -mapHeight / 2 + cellSize / 2);
        mapData = new MapElement[mapWidth, mapHeight];
        mapObjects = new GameObject[mapWidth, mapHeight];
        
        // 初始化地图数据（这里使用一个简单的地图布局作为示例）
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
                {
                    // 设置边界墙
                    mapData[x, y] = MapElement.Wall;
                }
                else
                {
                    // 设置内部区域
                    mapData[x, y] = MapElement.Empty;
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // 设置一些豆的位置（示例）
        for(int i = 0; i < initialDotCount; i++) {
            SpawnRandomDot();
        }
    }

    private void GenerateMap()
    {
        // 清除现有的地图对象
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 生成新的地图
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2 position = new Vector2(x * cellSize, y * cellSize) + mapOriginOffset;
                GameObject prefabToInstantiate = GetPrefabForElement(mapData[x, y]);
                
                if (prefabToInstantiate != null)
                {
                    GameObject obj = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                    obj.SetActive(true);
                    obj.name = $"MapElement_{x}_{y}_{prefabToInstantiate.name}";
                    mapObjects[x, y] = obj;
                }
            }
        }
    }

    private GameObject GetPrefabForElement(MapElement element)
    {
        switch (element)
        {
            case MapElement.Wall:
                return wallPrefab;
            case MapElement.Dot:
                return dotPrefab;
            case MapElement.PowerDot:
                return powerDotPrefab;
            case MapElement.Empty:
                return null; // 空区域不生成预制体
            default:
                return null;
        }
    }

    // 在随机空位置生成新的豆子
    public void SpawnRandomDot()
    {
        
        if (emptyPositions.Count > 0)
        {
            // 随机选择一个空位置
            int randomIndex = Random.Range(0, emptyPositions.Count);
            List<Vector2Int> tempList = new List<Vector2Int>(emptyPositions);
            Vector2Int pos = tempList[randomIndex];
            
            // 根据概率决定生成普通豆子还是能量豆
            MapElement dotType = Random.value < powerDotChance ? MapElement.PowerDot : MapElement.Dot;
            
            // 生成新的豆子
            SetMapElementAt(pos.x, pos.y, dotType);
        }
    }

    // 设置指定位置的地图元素类型
    public void SetMapElementAt(int x, int y, MapElement element)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            mapData[x, y] = element;

            // 销毁原有的对象
            if (mapObjects[x, y] != null)
            {
                Destroy(mapObjects[x, y]);
            }

            // 生成新的对象
            Vector3 position = new Vector3(x * cellSize, y * cellSize, 0) + new Vector3(mapOriginOffset.x, mapOriginOffset.y, 0);
            GameObject prefabToInstantiate = GetPrefabForElement(element);
            if (prefabToInstantiate != null)
            {
                mapObjects[x, y] = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                mapObjects[x, y].SetActive(true);
                mapObjects[x, y].name = $"MapElement_{x}_{y}";
            }

            // 加入或移出emptyPositions
            Vector2Int pos = new Vector2Int(x, y);
            if (element == MapElement.Empty)
            {
                emptyPositions.Add(pos);
            }
            else if (emptyPositions.Contains(pos))
            {
                emptyPositions.Remove(pos);
            }
            
        }
    }
} 