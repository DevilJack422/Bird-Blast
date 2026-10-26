using UnityEngine;

public class BlueBird : Bird
{
    [SerializeField] private Bird miniPrefab;
    [SerializeField] private float splitAngle = 15f;

    private Bird[] children;

    protected override bool HasAbility => true;

    protected override void UseAbility()
    {
        Vector2 v = rb.linearVelocity;
        children = new Bird[2];
        float[] angle = { +splitAngle, -splitAngle };

        for (int i = 0; i < 2; i++)
        {
            Bird mini = Instantiate(miniPrefab, transform.position, Quaternion.identity);
            mini.Launch(Quaternion.Euler(0, 0, angle[i]) * v);
            children[i] = mini;
        }
    }

    protected override bool CanFinish()
    {
        if (children == null) return true;
        foreach (var c in children)
        {
            if (c != null) return false;
        }
        return true;
    }
}
