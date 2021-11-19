using SnakeUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] private Image image;
    private ContentType contentType;

    /// <summary>
    /// Sets position of GridObject using pixel coordinates
    /// </summary>
    public void SetPosition(int x, int y, int z = 0)
    {
        rectTransform.localPosition = new Vector3(x, y, z);
    }

    public ContentType ContentType => contentType;
    public void OnPointerDown(PointerEventData eventData)
    {
        LevelEditorUI.Instance.MouseHeldDownState = true;
        ToolboxItem selectedTool = LevelEditorUI.Instance.SelectedTool;
        image.sprite = selectedTool.sprite;
        contentType = selectedTool.contentType;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LevelEditorUI.Instance.MouseHeldDownState = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LevelEditorUI.Instance.MouseHeldDownState == false)
            return;

        ToolboxItem selectedTool = LevelEditorUI.Instance.SelectedTool;
        image.sprite = selectedTool.sprite;
        contentType = selectedTool.contentType;
    }
}
