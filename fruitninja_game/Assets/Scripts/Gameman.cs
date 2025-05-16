using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        if (blade != null) blade.enabled = true;
        if (spawner != null) spawner.enabled = true;

        score = 0;
        if (scoreText != null) scoreText.text = score.ToString();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits) {
            if (fruit != null) Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();
        foreach (Bomb bomb in bombs) {
            if (bomb != null) Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        if (scoreText != null) scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void Explode()
    {
        if (blade != null) blade.enabled = false;
        if (spawner != null) spawner.enabled = false;

        if (fadeImage != null) {
            StartCoroutine(ExplodeSequence());
        } else {
            NewGame(); // Fallback if fade fails
        }
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }
}