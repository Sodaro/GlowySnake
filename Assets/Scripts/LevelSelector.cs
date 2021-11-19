using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] TMP_Text selectedLevelDisplayer;
    [SerializeField] SnakeGrid grid;
    [SerializeField] Button playButton;
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button editorButton;
    [SerializeField] Button exitGameButton;
    int selectedIndex = 0;
    string[] files;
    private void Awake()
    {
        files = Directory.GetFiles(Application.streamingAssetsPath + "/Levels/", "*.txt", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
            return;
        UpdateDisplayedLevelName(Path.GetFileName(files[selectedIndex]));
    }

    private void Start()
    {
        if (files.Length == 0)
            return;
        grid.GenerateLevelFromFile(files[selectedIndex]);
    }

    private void OnEnable()
    {
        nextButton.onClick.AddListener(IncrementIndex);
        previousButton.onClick.AddListener(DecrementIndex);
        playButton.onClick.AddListener(StartGame);
        editorButton.onClick.AddListener(LoadLevelEditor);
        exitGameButton.onClick.AddListener(ExitGame);
    }
    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(IncrementIndex);
        previousButton.onClick.RemoveListener(DecrementIndex);
        playButton.onClick.RemoveListener(StartGame);
        editorButton.onClick.RemoveListener(LoadLevelEditor);
        exitGameButton.onClick.RemoveListener(ExitGame);
    }

    void ExitGame()
    {
        SceneHandler.Instance.ExitGame();
    }

    void LoadLevelEditor()
    {
        SceneHandler.Instance.LoadScene(BuildScene.LevelEditor);
    }

    void StartGame()
    {
        if (files.Length == 0)
            return;
        if (!grid.IsValidLevel)
            return;
        EventHandler.Instance.RaiseOnGameStarted();
        gameObject.SetActive(false);
    }

    void GenerateLevel()
    {
        grid.GenerateLevelFromFile(files[selectedIndex]);
    }


    void IncrementIndex()
    {
        if (files.Length == 0)
            return;
        selectedIndex++;
        selectedIndex %= files.Length;
        //Debug.Log($"selectedIndex: {selectedIndex}");
        UpdateDisplayedLevelName(files[selectedIndex]);
        GenerateLevel();
    }

    void DecrementIndex()
    {
        if (files.Length == 0)
            return;

        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = files.Length - 1;
        //Debug.Log($"selectedIndex: {selectedIndex}");
        UpdateDisplayedLevelName(files[selectedIndex]);
        GenerateLevel();
    }

    void UpdateDisplayedLevelName(string path)
    {
        if (files.Length > selectedIndex)
        {
            selectedLevelDisplayer.text = Path.GetFileNameWithoutExtension(path);
        }
    }
}
