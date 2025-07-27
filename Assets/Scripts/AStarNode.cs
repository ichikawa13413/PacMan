
using UnityEngine;

public class AStarNode 
{
    public Vector3 position;
    public int cCost; // 移動コスト
    public float hCost; // ゴールまでの推定コスト
    public float sCost => cCost + hCost; // 総コスト
    public AStarNode comeFrom;//どのcellから来たのか

    public AStarNode(Vector3 pos, int c, float h)
    {
        position = pos;
        cCost = c;
        hCost = h;
    } 
}
