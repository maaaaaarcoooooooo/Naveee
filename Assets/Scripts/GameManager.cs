using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool GameStarted { get; private set; }
    public bool IsGameOver { get; private set; }

    int score;
    Text scoreText;
    GameObject startPanel;
    GameObject gameOverPanel;
    Text finalScoreText;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        Time.timeScale = 0;
        BuildUI();
        scoreText.text = "PUNTOS: 0";
    }

    void Update()
    {
        if (GameStarted && !IsGameOver && Player.instance == null)
            GameOver();
    }

    public void StartGame()
    {
        if (GameStarted) return;
        GameStarted = true;
        score = 0;
        scoreText.text = "PUNTOS: 0";
        startPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void AddScore(int points)
    {
        if (!GameStarted || IsGameOver) return;
        score += points;
        scoreText.text = "PUNTOS: " + score;
    }

    void GameOver()
    {
        IsGameOver = true;
        finalScoreText.text = "PUNTOS: " + score;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void BuildUI()
    {
        Canvas canvas = CreateCanvas();
        EnsureEventSystem();

        scoreText = CreateScoreText(canvas);
        startPanel = CreatePanel(canvas, "StartPanel", "NAVE", "MUEVE LA NAVE CON WASD O FLECHAS", "JUGAR", StartGame, out _);
        gameOverPanel = CreatePanel(canvas, "GameOverPanel", "GAME OVER", "", "REINICIAR", RestartGame, out finalScoreText);
        gameOverPanel.SetActive(false);
    }

    Canvas CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    Text CreateScoreText(Canvas canvas)
    {
        RectTransform rt = CreateUIElement("ScoreText", canvas.transform);
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -20);
        rt.sizeDelta = new Vector2(500, 60);
        Text text = rt.gameObject.AddComponent<Text>();
        text.font = GetBuiltinFont();
        text.fontSize = 36;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.color = Color.white;
        return text;
    }

    GameObject CreatePanel(Canvas canvas, string panelName, string title, string message, string buttonLabel, UnityAction onClick, out Text messageText)
    {
        RectTransform panelRT = CreateUIElement(panelName, canvas.transform);
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        panelRT.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

        CreateCenteredText(panelRT, "Title", title, new Vector2(0, 220), 80, new Vector2(900, 110));
        messageText = CreateCenteredText(panelRT, "Message", message, new Vector2(0, 60), 44, new Vector2(1100, 80));
        CreateButton(panelRT, buttonLabel, new Vector2(0, -140), new Vector2(320, 90), onClick);

        return panelRT.gameObject;
    }

    Text CreateCenteredText(RectTransform parent, string name, string content, Vector2 pos, int fontSize, Vector2 size)
    {
        RectTransform rt = CreateUIElement(name, parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text text = rt.gameObject.AddComponent<Text>();
        text.font = GetBuiltinFont();
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.color = Color.white;
        text.text = content;
        return text;
    }

    RectTransform CreateButton(RectTransform parent, string label, Vector2 pos, Vector2 size, UnityAction onClick)
    {
        RectTransform btnRT = CreateUIElement("Button", parent);
        btnRT.anchorMin = btnRT.anchorMax = new Vector2(0.5f, 0.5f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = pos;
        btnRT.sizeDelta = size;
        Image image = btnRT.gameObject.AddComponent<Image>();
        image.color = new Color(0.1f, 0.6f, 1f, 1f);

        Button button = btnRT.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.3f, 0.7f, 1f, 1f);
        colors.pressedColor = new Color(0.05f, 0.35f, 0.65f, 1f);
        colors.selectedColor = new Color(0.3f, 0.7f, 1f, 1f);
        button.colors = colors;
        button.onClick.AddListener(onClick);

        CreateCenteredText(btnRT, "Text", label, Vector2.zero, 40, new Vector2(300, 80));

        return btnRT;
    }

    RectTransform CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    Font GetBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return font;
    }
}