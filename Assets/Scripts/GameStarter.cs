using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    private bool hasClicked = false;
    public PlayableDirector director;
    public GameObject clickText;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = true;
    }

    public void OnStartClicked()
    {
        if (hasClicked)
            return;

        hasClicked = true;
        clickText.SetActive(false);
        director.Play();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
