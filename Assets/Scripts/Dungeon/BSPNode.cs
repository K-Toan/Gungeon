using UnityEngine;

public class BSPNode
{
    public RectInt Area;
    public BSPNode Left;
    public BSPNode Right;
    public RectInt Room;

    public BSPNode(RectInt area)
    {
        Area = area;
    }
}