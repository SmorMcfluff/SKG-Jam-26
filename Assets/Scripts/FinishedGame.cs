using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class FinishedGame : MonoBehaviour
{
    public static FinishedGame instance;
    public PlayableDirector timeline;

    public GameObject endGameCanvas;
    public PlayableDirector endGameTimeline;

    private void Awake()
    {
        instance = this;
    }

    public void StartEnd()
    {
        Sequence seq = DOTween.Sequence();

        seq
            .Append(Camera.main.transform.DOMoveX(0, 0.3f))
            .Join(Camera.main.DOOrthoSize(5, 0.3f))
            .Append(Camera.main.transform.DOMoveY(11, 0.2f))
            .AppendInterval(2)
            .Append(Camera.main.transform.DOMoveY(22, 1.3f))
            .AppendCallback(timeline.Play)
            .JoinCallback(() => MusicMaker.instance.Play());
    }

    public void EndGame()
    {
        MusicMaker.instance.Stop();
        endGameCanvas.SetActive(true);
        endGameTimeline.Play();
    }
}
