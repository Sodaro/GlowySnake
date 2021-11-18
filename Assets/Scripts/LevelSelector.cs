using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] TMP_Text selectedLevelDisplayer;
    [SerializeField] SnakeGrid grid;
    [SerializeField] Button playButton;
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button editorButton;
    int selectedIndex = 0;
    string[] files;
    private void Awake()
    {
        files = Directory.GetFiles(Application.streamingAssetsPath + "/Levels/", "*.txt", SearchOption.TopDirectoryOnly);
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
    }
    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(IncrementIndex);
        previousButton.onClick.RemoveListener(DecrementIndex);
        playButton.onClick.RemoveListener(StartGame);
        editorButton.onClick.RemoveListener(LoadLevelEditor);
    }

    void LoadLevelEditor()
    {
        SceneManager.LoadScene(1);
    }

    void StartGame()
    {
        EventHandler.Instance.RaiseOnGameStarted();
        gameObject.SetActive(false);
    }

    void GenerateLevel()
    {
        grid.GenerateLevelFromFile(files[selectedIndex]);
    }


    void IncrementIndex()
    {
        selectedIndex++;
        selectedIndex %= files.Length;
        //Debug.Log($"selectedIndex: {selectedIndex}");
        UpdateDisplayedLevelName(files[selectedIndex]);
        GenerateLevel();
    }

    void DecrementIndex()
    {
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
