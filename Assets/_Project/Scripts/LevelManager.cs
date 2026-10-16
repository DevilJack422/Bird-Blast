using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Slingshot slingshot;
    [SerializeField] private Bird[] birdPrefabs;
    [SerializeField] private Transform waitPoint;
    [SerializeField] private float spacing = 0.8f;
    [SerializeField] private float nextBirdDelay = 1f;
    [SerializeField] private CinemachineCamera followCam;

    private readonly Queue<Bird> queue = new();

    private int pigsAlive;

    void Start()
    {
        foreach (var prefab in birdPrefabs)
        {
            queue.Enqueue(Instantiate(prefab, waitPoint.position, Quaternion.identity));
        }

        slingshot.OnBirdLaunch += HandleLaunched;
        RepositionQueue();
        LoadNext();

        pigsAlive = FindObjectsByType<Pig>(FindObjectsSortMode.None).Length;
        Destructible.Destroyed += OnAnyDestroyed;
    }

    void OnDestroy()
    {
        slingshot.OnBirdLaunch -= HandleLaunched;
        Destructible.Destroyed -= OnAnyDestroyed;
    }

    private void OnAnyDestroyed(Destructible d)
    {
        if (d is Pig && --pigsAlive <= 0)
        {
            Debug.Log("Out of pigs, WIN!");
        }
    }

    private void LoadNext()
    {
        if (queue.Count == 0)
        {
            Debug.Log("Out of bird");
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
