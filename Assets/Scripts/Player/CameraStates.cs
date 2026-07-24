using System.Collections.Generic;
using UnityEngine;


public abstract class CameraState
{
    public float CameraSize;
    public Vector2 CameraPos;
    public HandInt HandState;

    public Dictionary<Direction, CameraTransition> Connections = new();
}

public class DeskState : CameraState
{
    public DeskState()
    {
        CameraSize = 5;
        CameraPos = Vector2.zero;
        HandState = HandInt.point;
    }
}

public class PCState : CameraState
{
    public PCState()
    {
        CameraSize = 1.8f;
        CameraPos = new(0.15f, 0.12f);
    }
}

public class ShootState : CameraState
{
    public ShootState()
    {
        CameraSize = 4;
        CameraPos = new(-10.6f, 0);
        HandState = HandInt.shoot;
    }
}

public enum Direction
{
    Up, Down, Left, Right,
}