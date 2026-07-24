using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicMaker : MonoBehaviour
{
    public static MusicMaker instance { get; private set; }

    private bool isPlaying;
    [HideInInspector] public bool isCreating;

    AudioSource src;

    public AudioClip snare;
    public AudioClip piano;
    public AudioClip hey;

    public float currentTime;
    public BeatColumn[] columns;
    private List<SpriteRenderer> markers = new();

    public float bpm = 120f;
    private int lastBeat = -1;

    private float BeatDuration => 60f / bpm;
    private float LoopDuration => BeatDuration * columns.Length;


    private void Awake()
    {
        instance = this;
        src = GetComponent<AudioSource>();

        gameObject.SetActive(false);
    }

    private void Start()
    {
        foreach (var column in columns)
        {
            markers.Add(column.GetComponent<SpriteRenderer>());
        }
    }

    private void Update()
    {
        if (!isPlaying) return;

        currentTime += Time.deltaTime;
        float beatTime = currentTime % LoopDuration;

        int currentBeat = Mathf.FloorToInt(beatTime / BeatDuration);

        if (currentBeat != lastBeat)
        {
            columns[currentBeat].Beat.Play();
            lastBeat = currentBeat;

            for (int i = 0; i < markers.Count; i++)
            {
                markers[i].color = (i == currentBeat)
                    ? Color.green
                    : Color.white;
            }
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && isCreating)
        {
            Stop();
            CameraController.instance.GoDirection(Direction.Down);
            GameManager.DisplayInstruction("KILL ALL DISTRACTIONS!!!", 2,
               () => CameraController.instance.GoDirection(Direction.Left, () => EnemySpawner.instance.StartWave(7, FinishedGame.instance.StartEnd)));
        }
    }

    public void Play()
    {
        currentTime = 0;
        isPlaying = true;
    }

    public void Stop()
    {
        currentTime = 0;
        isPlaying = false;
    }

    public void PlayBeat(BeatType beatType)
    {
        if (beatType.HasFlag(BeatType.Snare))
        {
            src.PlayOneShot(snare);
        }
        if (beatType.HasFlag(BeatType.Piano))
        {
            src.PlayOneShot(piano);
        }
        if (beatType.HasFlag(BeatType.Hey))
        {
            src.PlayOneShot(hey);
        }
    }
}
