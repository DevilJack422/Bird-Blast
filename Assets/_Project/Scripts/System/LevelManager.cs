using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public enum GameState { Playing, Won, Lose }

public class LevelManager : MonoBehaviour
{
    public event Action<int> OnBirdLeftChanged;
    public event Action<bool, int, int> OnLevelEnded; // win?, point, starts number

    [Header("-----Level-----")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Bird[] birdPrefabs;
    [SerializeField] private Transform waitPoint;
    [SerializeField] private float spacing = 0.8f;
    [SerializeField] private float nextBirdDelay = 1f;


    [Header("-----Offset-----")]
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private CinemachineCamera followCam;

    [Header("-----Physic Delay----")]
    [SerializeField] private float winSettleDelay = 2f;
    [SerializeField] private float loseSettleDelay = 3f;
 
    private readonly Queue<Bird> queue = new();

    private GameState state = GameState.Playing;
    private int pigsAlive;
    private int birdsLeft;

    public GameState State => state;
    public int BirdsLeft => birdsLeft;
    public LevelData Data => levelData;

    void Awake()
    {
        birdsLeft = birdPrefabs.Length;

        if (levelData.levelIndex == 1)
        {
            MusicManager.Instance.PlayMusic("Gameplay");
        }
    }

    void Start()
    {
        foreach (var prefab in birdPrefabs)
        {
            queue.Enqueue(Instantiate(prefab, waitPoint.position, Quaternion.identity));
        }

       pigsAlive = FindObjectsByType<Pig>(FindObjectsSortMode.None).Length;

       slingshot.OnBirdLaunch += HandleLaunched;
       Destructible.Destroyed += OnAnyDestroyed;

       RepositionQueue();
       LoadNext();
    }

    void OnDestroy()
    {
        slingshot.OnBirdLaunch -= HandleLaunched;
        Destructible.Destroyed -= OnAnyDestroyed;
    }

    // ---------------Win/Lose----------------
    private void OnAnyDestroyed(Destructible d)
    {
        if (state != GameState.Playing) return;
        if (d is Pig && --pigsAlive <= 0)
        {
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator LoseCheckRoutine()
    {
        yield return new WaitForSeconds(loseSettleDelay);
        if (state != GameState.Playing) yield break;

        state = GameState.Lose;
        slingshot.enabled = false;
        OnLevelEnded?.Invoke(false, ScoreManager.Instance.Point, 0);
    }

    private IEnumerator WinRoutine()
    {
        state = GameState.Won;
        slingshot.enabled = false;
        yield return new WaitForSeconds(winSettleDelay);

        ScoreManager.Instance.Add(birdsLeft * levelData.birdBonus);
        int point = ScoreManager.Instance.Point;
        int starts = CalcStarts(point);

        OnLevelEnded?.Invoke(true, point, starts);
    }

    private int CalcStarts(int point)
    {
        if (point >= levelData.threeStart) return 3;
        if (point >= levelData.twoStart) return 2;
        return 1;
    }

    // ------------ Wait on Bird---------------
    private void LoadNext()
    {
        if (state != GameState.Playing) return;

        if (queue.Count == 0)
        {
            StartCoroutine(LoseCheckRoutine());
            return;
        }

        Bird next = queue.Dequeue();
        RepositionQueue();
        StartCoroutine(HopToSling(next));
    }

    private IEnumerator HopToSling(Bird bird)
    {
        Vector3 from = bird.transform.position;
        Vector3 to = slingshot.Anchor.position;
        const float duration = 0.4f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float k = t / duration;
            Vector3 p = Vector3.Lerp(from, to, k);
            p.y += Mathf.Sin(k * Mathf.PI) * 0.8f;
            bird.transform.position = p;
            yield return null;
        }
        slingshot.LoadBird(bird);
    }

    private void RepositionQueue()
    {
        int i = 0;
        foreach (var b in queue)
        {
            b.transform.position = waitPoint.position + Vector3.left * spacing * i++;
        }
    }

    private void HandleLaunched(Bird bird)
    {
        birdsLeft--;
        OnBirdLeftChanged?.Invoke(birdsLeft);

        followCam.Follow = bird.transform;
        followCam.Priority = 20; // Increase IdleCam (10) camera turn to follow bird
        bird.OnFinished += HandleFinished;
    }

    private void HandleFinished(Bird bird)
    {
        bird.OnFinished -= HandleFinished;
        followCam.Priority = 0; // Under IdleCam so camera turn back
        followCam.Follow = null;
        StartCoroutine(NextAfterDelay());
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(nextBirdDelay);
        LoadNext();
    }
}
