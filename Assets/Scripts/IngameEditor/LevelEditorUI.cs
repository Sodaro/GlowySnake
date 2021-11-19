using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{

    public bool MouseHeldDownState = false;
    [SerializeField] private ToolboxItem selectedTool;
    [SerializeField] List<ToolboxItem> toolBoxItems = new List<ToolboxItem>();

    [SerializeField] EditorGrid editorGrid;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_InputField fileName;
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private TMP_Text gridSizeDisplay;
    [SerializeField] private TMP_Text errorTextDisplay;
    [SerializeField] private GameObject errorPanel;


    private static LevelEditorUI _instance;
    public static LevelEditorUI Instance => _instance;

    public ToolboxItem SelectedTool => selectedTool;

    void SetSelectedContent(ToolboxItem item)
    {
        selectedTool = item;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    //add event listeners
    private void OnEnable()
    {
        foreach (var item in toolBoxItems)
        {
            item.onClick.AddListener(delegate { SetSelectedContent(item); });
        }
        saveButton.onClick.AddListener(WriteLevelToFile);
        exitButton.onClick.AddListener(ExitLevelEditor);
        sizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    //remove event listeners
    private void OnDisable()
    {
        foreach (var item in toolBoxItems)
        {
            item.onClick.RemoveAllListeners();
        }
        saveButton.onClick.RemoveListener(WriteLevelToFile);
        exitButton.onClick.RemoveListener(ExitLevelEditor);
        sizeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    //update the grid size text and generate the new grid
    void OnSliderValueChanged(float value)
    {
        int valueAsInt = (int)value;
        gridSizeDisplay.text = $"{valueAsInt}x{valueAsInt}";
        editorGrid.GenerateGrid(valueAsInt);
    }

    //convert the editorgrid placed gridobjects string and write to file
    void WriteLevelToFile()
    {
        if (editorGrid == null)
            return;

        if (fileName.text == string.Empty)
        {
            errorPanel.SetActive(true);
            errorTextDisplay.text = "FILE NAME CANNOT BE EMPTY";
            return;
        }

        errorPanel.SetActive(false);
        errorTextDisplay.text = string.Empty;
        string path = Application.streamingAssetsPath + "/Levels/" + fileName.text + ".txt";
        FileInfo fileInfo = null;
        try
        {
            fileInfo = new FileInfo(path);
        }
        catch (ArgumentException)
        {
            errorPanel.SetActive(true);
            errorTextDisplay.text = "PATH IS NOT VALID";
        }
        catch (PathTooLongException)
        {
            errorPanel.SetActive(true);
            errorTextDisplay.text = "PATH IS TOO LONG";
        }
        catch (NotSupportedException)
        {
            errorPanel.SetActive(true);
            errorTextDisplay.text = "PATH IS NOT VALID";
        };

        if (fileInfo == null)
            return;

        string levelText = editorGrid.ConvertGridToString();
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine(levelText);
        }
    }
    void ExitLevelEditor()
    {
        SceneHandler.Instance.LoadScene(BuildScene.LevelSelector);
    }
}
