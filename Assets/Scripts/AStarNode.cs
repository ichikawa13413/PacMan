
using UnityEngine;

public class AStarNode 
{
    public Vector3 position;
    public int cCost; // �ړ��R�X�g
    public float hCost; // �S�[���܂ł̐���R�X�g
    public float sCost => cCost + hCost; // ���R�X�g
    public AStarNode comeFrom;//�ǂ�cell���痈���̂�

    public AStarNode(Vector3 pos, int c, float h)
    {
        position = pos;
        cCost = c;
        hCost = h;
    } 
}
