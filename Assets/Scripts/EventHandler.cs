using UnityEngine;

public class EventHandler : MonoBehaviour
{

    private static EventHandler _instance;
    public static EventHandler Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    //create a

    public delegate void OnGameStarted();
    public static event OnGameStarted onGameStarted;
    public void RaiseOnGameStarted()
    {
        if (onGameStarted != null)
        {
            onGameStarted.Invoke();
        }
    }
}
