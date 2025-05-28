using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{

    void Enter();
    void Update(Vector2 mousePos);
    void Exit();
}
public class StateMachine
{
    public IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update(Vector2 mousePos)
    {
        currentState?.Update(mousePos);
    }
}
