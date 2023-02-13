using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(10f, 30f)]
    [SerializeField] private float distance;

    private new Transform transform;
    private Vector3 center;
    private Dictionary<positionType, Vector3> positions;

    private positionType currentPosition;

    public positionType position
    {
        get => currentPosition;
        set => SetPosition(value);
    }

    public enum positionType
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
        positions = new Dictionary<positionType, Vector3>
        {
            { positionType.White, position + new Vector3(0f, 1f, -1f).normalized * distance },
            { positionType.Black, position + new Vector3(0f, 1f, 1f).normalized * distance }
        };

        SetPosition(positionType.White);
    }

    private void SetPosition(positionType type)
    {
        currentPosition = type;
        transform.position = positions[type];
        transform.LookAt(center);
    }
}
