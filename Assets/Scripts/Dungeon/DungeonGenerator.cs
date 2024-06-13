using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject connectorPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1.0f; // Kích thước của mỗi ô trong grid, tương ứng với pixel per unit

    private void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, y * cellSize, 0);
                PlaceRoomOrConnector(cellPosition);
            }
        }
    }

    void PlaceRoomOrConnector(Vector3 position)
    {
        // Tùy thuộc vào logic của bạn để xác định vị trí đặt room hoặc connector
        bool placeRoom = Random.value > 0.5f;

        if (placeRoom)
        {
            Instantiate(roomPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            Instantiate(connectorPrefab, position, Quaternion.identity, transform);
        }
    }
    // public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    // {
    //     Queue<BoundsInt> roomsQueue = new();
    //     List<BoundsInt> roomsList = new();
    //     roomsQueue.Enqueue(spaceToSplit);
    //     while (roomsQueue.Count > 0)
    //     {
    //         var room = roomsQueue.Dequeue();
    //         if (room.size.x >= minWidth && room.size.y >= minHeight)
    //         {
    //             if (Random.value < 0.5f)
    //             {
    //                 if (room.size.x >= minHeight * 2)
    //                 {
    //                     SplitVertically(minWidth, minHeight, roomsQueue, room);
    //                 }
    //                 else if (room.size.y >= minHeight * 2)
    //                 {
    //                     SplitHorizontally(minWidth, minHeight, roomsQueue, room);
    //                 }
    //                 else if (room.size.x >= minWidth && room.size.y >= minHeight)
    //                 {
    //                     roomsList.Add(room);
    //                 }
    //             }
    //             else
    //             {
    //                 if (room.size.y >= minHeight * 2)
    //                 {
    //                     SplitHorizontally(minWidth, minHeight, roomsQueue, room);
    //                 }
    //                 else if (room.size.x >= minHeight * 2)
    //                 {
    //                     SplitVertically(minWidth, minHeight, roomsQueue, room);
    //                 }
    //                 else if (room.size.x >= minWidth && room.size.y >= minHeight)
    //                 {
    //                     roomsList.Add(room);
    //                 }
    //             }
    //         }
    //     }
    //     return roomsList;
    // }

    // private static void SplitVertically(int minWidth, int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    // {
    //     var xSplit = Random.Range(1, room.size.x);
    //     BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
    //     BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
    //                                     new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
    //     roomsQueue.Enqueue(room1);
    //     roomsQueue.Enqueue(room2);
    // }

    // private static void SplitHorizontally(int minWidth, int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    // {
    //     var ySplit = Random.Range(1, room.size.x);
    //     BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
    //     BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
    //                                     new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
    //     roomsQueue.Enqueue(room1);
    //     roomsQueue.Enqueue(room2);
    // }
}
