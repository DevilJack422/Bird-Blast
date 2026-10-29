using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("-----Camera-----")]
    [SerializeField] private CinemachineCamera introCam;
    [SerializeField] private float introHold = 2f;
    [SerializeField] private float pigViewHold = 3f; // Camera stay still at pig after bird hit
    [SerializeField] private float camReturnTime = 1.3f;

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
    private bool camBusy;

    public GameState State => state;
    public int BirdsLeft => birdsLeft;
    public LevelData Data => levelData;

    void Awake()
    {
        birdsLeft = birdPrefabs.Length;
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
       StartCoroutine(IntroRoutine());
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
        int stars = CalcStarts(point);

        SaveSystem.SaveResult(levelData.levelIndex, point, stars);
        OnLevelEnded?.Invoke(true, point, stars);
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
        followCam.Priority = 20;

        bird.OnFirstHit += HandleFirstHit;
        bird.OnFinished += HandleFinished;
    }

    private void HandleFinished(Bird bird)
    {
        bird.OnFinished -= HandleFinished;
        bird.OnFirstHit -= HandleFirstHit;

        if (!camBusy) ReturnToSling();
        StartCoroutine(NextAfterDelay());
    }

    private void HandleFirstHit(Bird bird)
    {
        bird.OnFinished -= HandleFirstHit;
        if (!camBusy) StartCoroutine(PigViewRoutine());
    }

    private void FocusPig()
    {
        var c = PigCenter();
        if (c.HasValue)
        {
            introCam.transform.position = new Vector3(c.Value.x, c.Value.y, introCam.transform.position.z);
        }
        introCam.Priority = 30;
    }

    private void ReturnToSling()
    {
        introCam.Priority = 0;
        followCam.Priority = 0;
    }

    private IEnumerator PigViewRoutine()
    {
        camBusy = true;
        FocusPig();
        yield return new WaitForSeconds(pigViewHold);
        ReturnToSling();
        yield return new WaitForSeconds(camReturnTime);
        camBusy = false;
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(nextBirdDelay);
        while (camBusy) yield return null;
        LoadNext();
    }

    private IEnumerator IntroRoutine()
    {
        if (introCam != null)
        {
            var pigs = FindObjectsByType<Pig>(FindObjectsSortMode.None);
            if (pigs.Length > 0)
            {
                Vector3 sum = Vector3.zero;
                foreach (var p in pigs) sum += p.transform.position;
                Vector3 c = sum / pigs.Length;
                introCam.transform.position = new Vector3(c.x, c.y, introCam.transform.position.z);
            }

            introCam.Priority = 30;
            float t = 0f;
            while (t < introHold)
            {
                t += Time.deltaTime;
                if (t > 0.05f && Pointer.current != null && Pointer.current.press.wasPressedThisFrame) break;
                yield return null;
            }

            introCam.Priority = 0;
            yield return null;
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            while (brain != null && brain.IsBlending) yield return null;
        }

        LoadNext();
    }

    private Vector2? PigCenter()
    {
        var pigs = FindObjectsByType<Pig>(FindObjectsSortMode.None);
        if (pigs.Length == 0) return null;

        Vector2 sum = Vector2.zero;
        foreach (var p in pigs) sum += (Vector2)p.transform.position;
        return sum / pigs.Length;
    }
}
