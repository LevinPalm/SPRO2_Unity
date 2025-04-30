using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; } // Using the GameState enum
    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetState(GameState.StartMenu); // Using the enum value
        }
        else
        {
            Debug.LogWarning("Another GameManager instance found, destroying!");
        }
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        Debug.Log($"Game State Changed To: {newState}");
        OnStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.StartMenu:
                SceneManager.LoadScene("Start");
                break;
            case GameState.Playing:
                SceneManager.LoadScene("Main");
                break;
            case GameState.Interacting:
                SceneManager.LoadScene("Interaction");
                break;
            case GameState.Loading:
                break;
            case GameState.Paused:
                break;
        }
    }

    public void StartGame()
    {
        Debug.Log("GameManager.StartGame() called.");
        SetState(GameState.Playing); // Using the enum value
    }

    public void Interacting()
    {
        SetState(GameState.Interacting); // Using the enum value
    }

    public void ReturnToGame()
    {
        SetState(GameState.Playing); // Using the enum value
    }
}
