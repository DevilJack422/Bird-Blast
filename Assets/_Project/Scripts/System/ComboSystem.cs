using TMPro;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance;

    [SerializeField] private float timeLimit = 1.5f; // time limit for destuction to be combo
    [SerializeField] private int showFrom = 3; // from which destoy is text appear

    private TMP_Text comboText;
    private int count;
    private float timer;

    void Awake()
    {
        Instance = this;
        comboText = GetComponent<TMP_Text>();
        comboText.text = "";
    }

    void Update()
    {
        if (count == 0) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            count = 0;
            comboText.text = "";
        }
    }


    public float Register()
    {
        count++;
        timer = timeLimit;

        float mult = GetMultiplier();
        if (count >= showFrom)
        {
            comboText.text = $"COMBO x{count}";
        }
        comboText.color = mult >= 2f ? Color.red : Color.yellow;

        return mult;
    }

    private float GetMultiplier()
    {
        if (count >= 10) return 3f;
        if (count >= 6) return 2f;
        if (count >= 3) return 1.5f;
        return 1f;
    }

    [ContextMenu("Test Combo")]
    private void Test() => Register();
}
