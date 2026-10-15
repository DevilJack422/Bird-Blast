using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Slingshot : MonoBehaviour
{
    public event Action<Bird> OnBirdLaunch;
    public Transform Anchor => anchor;
    
    [Header("-----Slingshot Setting-----")]
    [SerializeField] private Transform anchor;
    [SerializeField] private LineRenderer bandFont, bandBack, trajectory;
    [SerializeField] private float maxDrag = 2f;
    [SerializeField] private float launchPower = 8f;
    [SerializeField] private int trajectoryPoints = 25;
    [SerializeField] private float trajectorySteps = 0.08f;
    [SerializeField] private AudioClip slingshotStretch;
    [SerializeField] private AudioClip slingshotSnap;

    private Camera cam;
    private bool dragging;
    private Bird bird;

    void Awake()
    {
        cam = Camera.main;
        trajectory.positionCount = 0;
        bandFont.positionCount = bandBack.positionCount = 2;
        UpdateBands(anchor.position);
    }

    void Update()
    {
        if (bird == null || bird.IsLaunched || Pointer.current == null) return;

        Vector2 pointer = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());

        if (Pointer.current.press.wasPressedThisFrame)
        {
            var hit = Physics2D.OverlapPoint(pointer);
            dragging = hit != null && hit.GetComponent<Bird>() == bird;
            if (dragging) AudioManager.Instance.PlaySFX(SfxId.SlingStretch);
        }

        if (dragging)
        {
            Vector2 offset = Vector2.ClampMagnitude(pointer - (Vector2)anchor.position, maxDrag);
            Vector2 pos = (Vector2)anchor.position + offset;
            bird.transform.position = pos;
            UpdateBands(pos);
            DrawTrajectory(pos, GetLaunchVelocity(pos));
        }

        if (dragging && Pointer.current.press.wasReleasedThisFrame)
        {
            dragging = false;
            AudioManager.Instance.PlaySFX(SfxId.SlingRelease);

            Bird launched = bird;
            launched.Launch(GetLaunchVelocity(launched.transform.position));
            bird = null;
            trajectory.positionCount = 0;
            UpdateBands(anchor.position);

            OnBirdLaunch?.Invoke(launched);
        }
    }

    Vector2 GetLaunchVelocity(Vector2 birdPos) => ((Vector2)anchor.position - birdPos) * launchPower;

    private void UpdateBands(Vector3 birdPos)
    {
        bandFont.SetPosition(0, anchor.position + Vector3.left * 0.2f);
        bandFont.SetPosition(1, birdPos);
        bandBack.SetPosition(0, anchor.position + Vector3.right * 0.2f);
        bandBack.SetPosition(1, birdPos);
    }

    private void DrawTrajectory(Vector2 start, Vector2 v0)
    {
        trajectory.positionCount = trajectoryPoints;
        Vector2 g = bird.GravityVector;
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * trajectorySteps;
            trajectory.SetPosition(i, start + v0 * t + 0.5f * g * t * t);
        }
    }

    public void LoadBird(Bird b)
    {
        bird = b;
        bird.transform.position = anchor.position;
        UpdateBands(anchor.position);
    }
}
