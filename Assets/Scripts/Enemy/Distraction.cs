using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class Distraction : MonoBehaviour
{
    private SpriteRenderer sr;

    private Vector2 spawnPos;
    private Vector2 targetPos;

    private float finalScale = 2.5f;

    public int health = 10;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    public void Initialize(Sprite sprite, float duration, Vector2 spawnPosition)
    {
        sr.sprite = sprite;

        spawnPos = spawnPosition;
        transform.position = spawnPosition;
        targetPos = spawnPosition + Vector2.down * 3f;

        StartCoroutine(StartWalking(duration));
    }

    IEnumerator StartWalking(float duration)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * finalScale, t);
            transform.localPosition = Vector2.Lerp(spawnPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector2.one * finalScale;
        transform.localPosition = targetPos;

        GameManager.DisplayInstruction("YOU FUCKED UP!! \n NO JAME GAMS FOR YOU!!", 3, () => SceneManager.LoadScene("MainScene"));
    }

    public void Die()
    {
        StopAllCoroutines();
        EnemySpawner.instance.ReportDeadEnemy();

        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashRed());

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public IEnumerator FlashRed()
    {
        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }
}
