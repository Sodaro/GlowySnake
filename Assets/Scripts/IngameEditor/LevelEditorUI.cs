using UnityEngine;
using System.Collections.Generic;

public class LevelEditorUI : MonoBehaviour
{

    public bool MouseHeldDownState = false;
    [SerializeField] private ToolboxItem selectedTool;
    [SerializeField] List<ToolboxItem> toolBoxItems = new List<ToolboxItem>();

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
    private void OnEnable()
    {
        foreach (var item in toolBoxItems)
        {
            item.onClick.AddListener(delegate { SetSelectedContent(item); });
        }
    }

    private void OnDisable()
    {
        foreach (var item in toolBoxItems)
        {
            item.onClick.RemoveAllListeners();
        }
    }
}
