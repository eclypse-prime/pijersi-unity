using System.Linq;
using UnityEngine;

[SelectionBase]
public partial class Piece : MonoBehaviour
{
    private const int maxRotationOffset = 20;
    private const float maxPositionOffset = .2f;
    private const float moveDuration = 1f;

    [SerializeField] private AnimationCurve jumpCurve;
    public Transform Transform { get; private set; }
    public MeshRenderer mainRenderer;
    public MeshRenderer[] SignRenderer;
    public Vector3 EndPosition { get; private set; }

    public PieceType type;
    public PieceType prey;
    [HideInInspector] public int team;
    [HideInInspector] public Cell cell;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float startTime;
    private bool isJump;

    private void Awake()
    {
        Transform = transform;
    }

    protected virtual bool CanAttack(Piece targetPiece, Cell targetCell)
    {
        if (targetPiece.type != prey || targetPiece.team == team || this != cell.LastPiece) return false;

        foreach (Cell near in cell.Nears)
        {
            if (near == null) continue;

            if (near == targetCell || CanAttackFar(near)) return true;
        }

        return false;

        bool CanAttackFar(Cell near)
        {
            if (!near.IsEmpty)
            {
                bool isUnvalidNear = near.pieces[0].team == team ? near.IsFull : near.LastPiece.type != prey;
                if (isUnvalidNear) return false;
            }

            foreach (Cell farNear in near.Nears)
            {
                if (farNear == null || farNear == cell) continue;

                if (farNear == targetCell)
                {
                    bool canDirectAttack = cell.IsFull && near.IsEmpty;
                    if (canDirectAttack) return true;

                    bool canIndirectAttack = near.LastPiece?.team == team ? !near.IsFull : cell.IsFull && near.LastPiece?.type == prey;
                    if (canIndirectAttack) return true;

                    break;
                }

                if (CanAttackDeep(near, farNear)) return true;
            }

            return false;
        }

        bool CanAttackDeep(Cell near, Cell farNear)
        {
            if (!farNear.IsEmpty)
            {
                bool isUnvalidNear = near.IsFull || !near.IsEmpty && near.pieces[0].team != team;
                bool isUnvalidFar = farNear.pieces[0]?.team == team;
                if (isUnvalidNear || isUnvalidFar) return false;
            }

            foreach (Cell deepNear in farNear.Nears)
            {
                if (deepNear == null) continue;

                if (deepNear == targetCell)
                {
                    bool canIndirectAttack;
                    if (near.IsEmpty)
                        canIndirectAttack = cell.IsFull && cell.GetDirectFarNears().Contains(farNear) && (farNear.IsEmpty || farNear.LastPiece.type == prey);
                    else
                        canIndirectAttack = farNear.IsEmpty && near.GetDirectFarNears().Contains(deepNear);
                    if (canIndirectAttack) return true;

                    break;
                }
            }

            return false;
        }
    }
}
