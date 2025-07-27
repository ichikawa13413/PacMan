using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using VContainer;

//---�l����(A*���Q�l)---
//���݂̈ʒu���擾
//�㉺���E�̈ړ��\�ȃ^�C���iWall���Ȃ��^�C���j���擾
//���݈ʒu�ɗאڂ���^�C���i�ړ��\�ȃ^�C���j�̃R�X�g���v�Z����A���ʂ��L�^����
//��ԃR�X�g���Ⴉ�����^�C����I���A
public class FindGoal 
{
    private MapManager _mapManager;

    [Inject]
    public FindGoal(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    //�㉺���E�̃`�F�b�N�p
    Vector3Int[] directionsAround = new Vector3Int[]
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    /// <summary>
    /// ���ݒn����S�[���܂ł̐���R�X�g���v�Z���A�S�[���܂ł̍ŒZ�o�H�̌v�Z�Ɏg�p�B
    /// </summary>
    /// <param name="position">�Ώۂ̌��݈ʒu</param>
    /// <param name="goalpos">�ݒ肳�ꂽ�S�[��</param>
    /// <param name="cCost">�ݒ�ړ��R�X�g�i��{�I�ɂ͂P�j</param>
    /// <returns>�S�[��������A�ŒZ�o�H�̃��X�g��Ԃ�</returns>

    public List<AStarNode> AStar(Vector3 position, Vector3 goalpos, int cCost)
    {
        //�擾�����Z���̍��W���L�^���郊�X�g
        List<AStarNode> openNode = new();

        //�����ς݂̃��X�g
        HashSet<AStarNode> closeNode = new();

        AStarNode goalNode = new AStarNode(goalpos, cCost, 0);

        AStarNode startNode = new(position, 0,GetDistance(position, goalpos));

        openNode = new();
        closeNode = new();

        openNode.Add(startNode);

        while (openNode.Count > 0)
        {
            //openList���\�[�g���Ĉ�Ԃr�R�X�g���Ⴂ���I��
            openNode.Sort((s1, s2) => s1.sCost.CompareTo(s2.sCost));

            //currentNode�͒������m�[�h
            AStarNode currentNode = openNode[0];

            openNode.Remove(currentNode);
            closeNode.Add(currentNode);

            //�S�[������
            if (currentNode.position == goalNode.position)
            {
                return RetracePath(startNode, currentNode);
            }

            Vector3Int currentCell = _mapManager.backGround.WorldToCell(currentNode.position);

            //currentNode�̏㉺���E�ɕǂ��Ȃ��ꍇ���X�g�ɒǉ�
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

    //�ŒZ�o�H�̍쐬
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

    //currentNode�̏㉺���E�ɕǂ��Ȃ��ꍇ���X�g�ɒǉ�
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
