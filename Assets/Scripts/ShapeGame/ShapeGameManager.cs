using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShapeGameManager : MonoBehaviour
{

    public static ShapeGameManager instance;

    public Sprite[] shapeSprites;

    public Sprite[] spriteTextures;

    public ShapeReceiver shapeHolePrefab;
    private ShapeReceiver currentReceiver;

    public ShapeObject shapePrefab;

    public Bounds bounds;

    public float objectSpacing = 1.5f;
    private readonly List<Vector2> usedPositions = new();

    public int scorePerObject = 1000;
    public int scorePerLevel = 10000;

    public float comboTime;
    private float comboTimer;

    public float bonusTime;
    private float totalTimer;

    public int[] spawnsPerLevel;
    private int currentLevel = 0;

    private int currentCombo = 1;

    private int swallowedThisLevel;

    public ShapeGameHUD gameHUD;

    private float minSpawnDistance;

    private LayerMask spawnMask;

    private Vector2 spawnSize;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        bounds = GetComponent<Collider2D>().bounds;

        spawnMask = LayerMask.GetMask("Shape", "Receiver");

        Bounds shapeBounds = shapePrefab.GetComponent<Collider2D>().bounds;
        spawnSize = shapeBounds.size;
        
        minSpawnDistance = Mathf.Max(shapeBounds.size.x, shapeBounds.size.y);
    }

    public void StartGame()
    {
        GameManager.DisplayInstruction("STEP 1 \n PROGRAM YOUR GAME!!!!", 2, StartGameForReal);
    }

    private void StartGameForReal()
    {
        currentLevel = 0;
        swallowedThisLevel = 0;
        currentCombo = 1;

        comboTimer = 0;
        totalTimer = bonusTime;

        gameHUD.gameObject.SetActive(true);
        gameHUD.UpdateMultiplier(currentCombo);

        GameManager.instance.pcGameInProgress = true;

        currentReceiver = Instantiate(shapeHolePrefab, bounds.center, Quaternion.identity, transform);

        currentReceiver.Initialize(GetShapeSprite(Shape.Square));

        StartLevel();
    }

    public void StartLevel()
    {
        ClearLevel();

        swallowedThisLevel = 0;

        totalTimer = bonusTime;
        SpawnObjects(spawnsPerLevel[currentLevel]);
    }

    public void Update()
    {
        if (!GameManager.instance.pcGameInProgress) return;
        if (comboTimer > 0)
        {
            comboTimer = Mathf.Max(0, comboTimer - Time.deltaTime);
        }
        else if (currentCombo > 1)
        {
            currentCombo = 1;
            gameHUD.UpdateMultiplier(currentCombo);
        }

        if (totalTimer > 0)
        {
            totalTimer = Mathf.Max(0, totalTimer - Time.deltaTime);
        }

        gameHUD.UpdateSlider(gameHUD.totalSlider, totalTimer / bonusTime);
        gameHUD.UpdateSlider(gameHUD.comboSlider, comboTimer / comboTime);
    }

    public void SpawnObjects(int amount)
    {
        usedPositions.Clear();
        for (int i = 0; i < amount; i++)
        {
            Shape shape = (Shape)Random.Range(
                0,
                Enum.GetNames(typeof(Shape)).Length);

            Vector2 shapePos = GetValidSpawnPosition();

            ShapeObject obj = Instantiate(shapePrefab, shapePos, Quaternion.identity, transform);

            obj.Initialize(shape, GetShapeSprite(shape), spriteTextures[Random.Range(0, spriteTextures.Length)]);
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        for (int i = 0; i < 500; i++)
        {
            Vector2 pos = new(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));

            if (!IsValidPosition(pos))
                continue;

            Collider2D[] hits = Physics2D.OverlapBoxAll(pos, spawnSize * 1.2f, 0f);

            bool blocked = false;

            foreach (Collider2D hit in hits)
            {
                if (hit.GetComponent<ShapeObject>() != null ||
                    hit.GetComponent<ShapeReceiver>() != null)
                {
                    blocked = true;
                    break;
                }
            }

            if (!blocked)
                return pos;
        }

        throw new Exception($"Failed to find valid spawn position for level {currentLevel}");
    }

    public bool IsValidPosition(Vector2 spawnPos)
    {
        Collider2D col = shapePrefab.GetComponent<Collider2D>();

        Bounds objectBounds = col.bounds;
        Vector2 size = objectBounds.size;

        return spawnPos.x - size.x / 2 > bounds.min.x &&
               spawnPos.x + size.x / 2 < bounds.max.x &&
               spawnPos.y - size.y / 2 > bounds.min.y &&
               spawnPos.y + size.y / 2 < bounds.max.y;
    }

    private Sprite GetShapeSprite(Shape shape)
    {
        return shapeSprites[(int)shape];
    }

    public void OnShapeSwallowed()
    {
        int scoreToAdd = scorePerObject * currentCombo;

        currentCombo++;
        swallowedThisLevel++;
        comboTimer = comboTime;

        gameHUD.UpdateMultiplier(currentCombo);

        ScoreManager.instance.AddPoints(scoreToAdd);

        if (swallowedThisLevel >= spawnsPerLevel[currentLevel])
        {
            OnLevelComplete();
        }
    }

    public void OnLevelComplete()
    {
        int scoreToAdd = scorePerLevel;

        float timeBonus = totalTimer / bonusTime;
        scoreToAdd += Mathf.FloorToInt(timeBonus * 10000);

        ScoreManager.instance.AddPoints(scoreToAdd);

        currentLevel++;

        if (currentLevel >= spawnsPerLevel.Length)
        {
            EndGame();
            return;
        }

        StartLevel();
    }

    private void EndGame()
    {
        GameManager.instance.pcGameInProgress = false;

        gameHUD.gameObject.SetActive(false);
        CameraController.instance.GoDirection(Direction.Down);
        GameManager.DisplayInstruction("KILL ALL DISTRACTIONS!!!", 2,
            () =>
            {
                EnemySpawner.instance.StartWave(3,
                    () => CameraController.instance.GoDirection(Direction.Right, () =>
                    {
                        CameraController.instance.GoDirection(Direction.Up);
                        GameManager.DisplayInstruction("DRAW A NICE CHARACTER \nFOR YOUR GAME!!", 2);
                    }
                    ));
                CameraController.instance.GoDirection(Direction.Left);
            }
        );
        ClearLevel();
        Destroy(currentReceiver.gameObject);
        Destroy(this);
    }

    private void ClearLevel()
    {
        foreach (ShapeObject shape in GetComponentsInChildren<ShapeObject>())
        {
            Destroy(shape.gameObject);
        }
    }
}
