using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private static EventHandler _instance;
    public static EventHandler Instance => _instance;

    private void Awake()
    {
        if ( _instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public delegate void OnGameStarted();
    public static event OnGameStarted onGameStarted;
    public void RaiseOnGameStarted()
    {
        if (onGameStarted != null)
        {
            onGameStarted.Invoke();
        }
    }


    public delegate void OnGamePaused();
    public static event OnGamePaused onGamePaused;
    public void RaiseOnGamePaused()
    {
        if (onGamePaused != null)
        {
            onGamePaused.Invoke();
        }
    }
}
