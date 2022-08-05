using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pijersi : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;
    
    private enum State
    {
        Wait,
        Ready
    }

    private State state;
    private new Camera camera;

    public static Pijersi Instance;

    #region base
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state   = State.Ready;
        camera  = Camera.main;
    }

    private void Update()
    {
        OnStateUpdate();
    }
    #endregion

    #region State Machine
    private void OnStateEnter()
    {
        switch (state)
        {
            case State.Ready:
                OnEnterReady();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }
    private void OnStateExit()
    {
        switch (state)
        {
            case State.Ready:
                OnExitReady();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }
    private void OnStateUpdate()
    {
        switch (state)
        {
            case State.Ready:
                OnUpdateReady();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }

    private void ChangeState(State newState)
    {
        OnStateExit();
        state = newState;
        OnStateEnter();
    }
    #endregion

    #region Ready
    private void OnEnterReady() { }
    private void OnExitReady() { }
    private void OnUpdateReady()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out hit, 50f, cellLayer);

        animation.UpdateHighlight(hit.transform);
    }
    #endregion

/*
    #region 
    private void OnEnter() { }
    private void OnExit() { }
    private void OnUpdate() { }
    #endregion
*/
}
