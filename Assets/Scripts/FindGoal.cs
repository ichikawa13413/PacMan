using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using VContainer;

//---考え方(A*を参考)---
//現在の位置を取得
//上下左右の移動可能なタイル（Wallがないタイル）を取得
//現在位置に隣接するタイル（移動可能なタイル）のコストを計算する、結果を記録する
//一番コストが低かったタイルを選択、
public class FindGoal 
{
    private MapManager _mapManager;

    [Inject]
    public FindGoal(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    //上下左右のチェック用
    Vector3Int[] directionsAround = new Vector3Int[]
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    /// <summary>
    /// 現在地からゴールまでの推定コストを計算し、ゴールまでの最短経路の計算に使用。
    /// </summary>
    /// <param name="position">対象の現在位置</param>
    /// <param name="goalpos">設定されたゴール</param>
    /// <param name="cCost">設定移動コスト（基本的には１）</param>
    /// <returns>ゴール発見後、最短経路のリストを返す</returns>

    public List<AStarNode> AStar(Vector3 position, Vector3 goalpos, int cCost)
    {
        //取得したセルの座標を記録するリスト
        List<AStarNode> openNode = new();

        //調査済みのリスト
        HashSet<AStarNode> closeNode = new();

        AStarNode goalNode = new AStarNode(goalpos, cCost, 0);

        AStarNode startNode = new(position, 0,GetDistance(position, goalpos));

        openNode = new();
        closeNode = new();

        openNode.Add(startNode);

        while (openNode.Count > 0)
        {
            //openListをソートして一番Ｓコストが低いやつを選択
            openNode.Sort((s1, s2) => s1.sCost.CompareTo(s2.sCost));

            //currentNodeは調査中ノード
            AStarNode currentNode = openNode[0];

            openNode.Remove(currentNode);
            closeNode.Add(currentNode);

            //ゴール判定
            if (currentNode.position == goalNode.position)
            {
                return RetracePath(startNode, currentNode);
            }

            Vector3Int currentCell = _mapManager.backGround.WorldToCell(currentNode.position);

            //currentNodeの上下左右に壁がない場合リストに追加
            HashSet<Vector3> checkedCell = CheckCell(currentCell);

            foreach (Vector3 pos in checkedCell)
            {
                if (closeNode.Any(node => node.position == pos))
                {
                    continue;
                }

                float hcost = GetDistance(pos, goalpos);
                AStarNode node = new(pos, cCost, hcost);
                node.comeFrom = currentNode;

                if (!openNode.Contains(node))
                {
                    openNode.Add(node);
                }
            }
        }
        return null;
    }

    //最短経路の作成
    private List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.comeFrom;
        }

        path.Reverse();
        return path;
    }

    private float GetDistance(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        return distance;
    }

    //currentNodeの上下左右に壁がない場合リストに追加
    private HashSet<Vector3> CheckCell(Vector3Int targetCell)
    {
        HashSet<Vector3> vectores = new();

        Vector3Int targetCenter = _mapManager.backGround.WorldToCell(targetCell);

        foreach (Vector3Int direction in directionsAround)
        {
            Vector3Int pos = direction + targetCenter;

            if (_mapManager.backGround.HasTile(pos) && !_mapManager.wall.HasTile(pos))
            {
                Vector3 current = _mapManager.backGround.GetCellCenterWorld(pos);
                vectores.Add(current);
            }
        }
        return vectores;
    }
}
