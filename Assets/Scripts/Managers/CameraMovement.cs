using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(10f, 30f)]
    [SerializeField] private float distance;

    private new Transform transform;
    private Vector3 center;
    private Dictionary<PositionType, Vector3> positions;

    private PositionType currentPosition;

    public PositionType position
    {
        get => currentPosition;
        set => SetPosition(value);
    }

    public enum PositionType
    {
        Black,
        White
    }

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    public void SetCenter(Vector3 position)
    {
        center = position;
        positions = new Dictionary<PositionType, Vector3>
        {
            { PositionType.White, position + new Vector3(0f, 1f, -1f).normalized * distance },
            { PositionType.Black, position + new Vector3(0f, 1f, 1f).normalized * distance }
        };

        SetPosition(PositionType.White);
    }

    private void SetPosition(PositionType type)
    {
        currentPosition = type;
        transform.position = positions[type];
        transform.LookAt(center);
    }
}
