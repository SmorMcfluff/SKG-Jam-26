using DG.Tweening;
using System;
using UnityEngine;

public class CameraTransition
{
    public CameraState TargetState;

    public Func<Camera, Sequence> CreateSequence;
    public Action OnComplete;
}
