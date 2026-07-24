using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PCGameState state;

    public PCGameState shapeGameState;
    public PCGameState drawGameState;
    public PCGameState musicGameState;

    public TextMeshProUGUI instruction;

    public bool pcGameInProgress;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        shapeGameState = new ShapeHole();
        drawGameState = new CreateArt();
        musicGameState = new CreateMusic();

        Application.targetFrameRate = 60;
    }

    public void ChangeState(PCGameState newState)
    {
        state = newState;

        if (newState is ShapeHole)
        {
            ShapeGameManager.instance.StartGame();
        }

        if (newState is CreateArt)
        {
            CharacterDrawer.instance.gameObject.SetActive(true);
        }

        if (newState is CreateMusic)
        {
            MusicMaker.instance.gameObject.SetActive(true);
            MusicMaker.instance.Play();
            MusicMaker.instance.isCreating = true;
        }
    }

    public static void SetCursor(bool visible)
    {
        Cursor.visible = visible;
    }

    public static void DisplayInstruction(string text, float duration, Action onComplete = null)
    {
        instance.StartCoroutine(instance.ShowInstruction(text, duration, onComplete));
    }

    private IEnumerator ShowInstruction(string text, float duration, Action onComplete = null)
    {
        instance.instruction.text = text;

        yield return new WaitForSeconds(duration);
        instance.instruction.text = "";
        onComplete?.Invoke();
    }
}

public class PCGameState
{
    public HandInt gameHandState;
}

public class ShapeHole : PCGameState
{
    public ShapeHole()
    {
        gameHandState = HandInt.shape;
    }
}

public class CreateArt : PCGameState
{
    public CreateArt()
    {
        gameHandState = HandInt.point;
    }
}

public class CreateMusic : PCGameState
{
    public CreateMusic()
    {
        gameHandState = HandInt.point;
    }
}
