using DG.Tweening;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public bool isTransitioning;

    public CameraState currentState;

    private CameraState deskState;
    private CameraState pcState;
    private CameraState shootState;

    private Camera cam;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;

        SetUpCameraStates();

        currentState = deskState;

        GoDirection(Direction.Up);
    }

    private void SetUpCameraStates()
    {
        deskState = new DeskState();
        pcState = new PCState();
        shootState = new ShootState();

        HandController hand = HandController.instance;

        deskState.Connections.Add(
            Direction.Up,
            CreateSimpleTransition(pcState,
            () =>
            {
                PCGameState nextState = GameManager.instance.state switch
                {
                    null => GameManager.instance.shapeGameState,
                    ShapeHole => GameManager.instance.drawGameState,
                    CreateArt => GameManager.instance.musicGameState,
                    _ => throw new NotImplementedException()
                };

                GameManager.instance.ChangeState(nextState);

                hand.ChangeHandState(nextState.gameHandState);

            }));

        deskState.Connections.Add(
            Direction.Left,
            CreateArcTransition(shootState, -14.6f, () => hand.ChangeHandState(shootState.HandState)));

        pcState.Connections.Add(
            Direction.Down,
            CreateSimpleTransition(deskState, () => hand.ChangeHandState(deskState.HandState)));

        shootState.Connections.Add(
            Direction.Right,
            CreateArcTransition(deskState, -14.6f, () => hand.ChangeHandState(deskState.HandState)));
    }

    private void Update()
    {
        if (!isTransitioning)
        {
            ReadInputs();
        }
    }

    public void ReadInputs()
    {
    }

    public void GoDirection(Direction direction, Action onComplete = null)
    {
        if (!currentState.Connections.TryGetValue(direction, out CameraTransition transition)) return;

        isTransitioning = true;

        transition.CreateSequence(cam)
            .OnComplete(() =>
            {
                currentState = transition.TargetState;
                isTransitioning = false;
                onComplete?.Invoke();
            });
    }

    private CameraTransition CreateSimpleTransition(CameraState target, Action onComplete = null)
    {
        return new CameraTransition
        {
            TargetState = target,
            OnComplete = onComplete,
            CreateSequence = cam =>
            {
                Vector3 pos = new(
                    target.CameraPos.x,
                    target.CameraPos.y,
                    cam.transform.position.z);

                return DOTween.Sequence()
                    .Append(cam.DOOrthoSize(target.CameraSize, 0.4f)).SetEase(Ease.InOutCubic)
                    .Join(cam.transform.DOMove(pos, 0.4f)).SetEase(Ease.InOutCubic)
                    .AppendCallback(() => onComplete?.Invoke());
            }
        };
    }

    private CameraTransition CreateArcTransition(CameraState target, float angle, Action onComplete = null)
    {
        return new CameraTransition
        {
            TargetState = target,
            CreateSequence = cam =>
            {
                Vector3 targetPos = new(
                    target.CameraPos.x,
                    target.CameraPos.y,
                    cam.transform.position.z);

                Vector3 start = cam.transform.position;

                Vector3 midPoint = new(
                    (start.x + targetPos.x) * 0.5f,
                    (start.y + targetPos.y) * 0.5f,
                    start.z);

                Sequence seq = DOTween.Sequence();

                seq.Join(cam.DOOrthoSize(target.CameraSize, 0.6f));

                seq.Join(
                    cam.transform.DOPath(
                        new[] { midPoint, targetPos },
                        0.6f,
                        PathType.CatmullRom));

                seq.Join(
                    cam.transform
                        .DOLocalRotate(
                            new Vector3(0, angle, 0),
                            0.3f)
                        .SetLoops(2, LoopType.Yoyo))

                .AppendCallback(() => onComplete?.Invoke());

                return seq;
            }
        };
    }
}