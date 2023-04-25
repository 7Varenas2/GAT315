using Assets.Scripts.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Events")]
    [SerializeField] EventRouter startGameEvent;
    [SerializeField] EventRouter stopGameEvent;


    [SerializeField] AudioSource titleMusic;
    [SerializeField] AudioSource gameMusic;

    //[SerializeField] GameObject playerPrefab;
    //[SerializeField] Transform playerStart;


    public enum State
    {
        TITLE,
        START_GAME,
        PLAY_GAME,
        GAME_OVER,
        GAME_WON
    }
    State state = State.TITLE;
    float stateTimer = 0;

    private void Start()
    {
        //winGameEvent.onEvent += SetGameWin;
        titleMusic.Play();
    }

    private void Update()
    {
        switch (state)
        {
            case State.TITLE:
                
                UIManager.Instance.ShowTitle(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case State.START_GAME:
                startGameEvent.Notify();
                UIManager.Instance.ShowTitle(false);
                state = State.PLAY_GAME;
                gameMusic.Play();
                break;
            case State.PLAY_GAME:
                break;
            case State.GAME_OVER:
                stateTimer -= Time.deltaTime;
                SetGameOver();
                if (stateTimer <= 0)
                {
                    UIManager.Instance.ShowGameOver(false);
                    state = State.TITLE;
                }
                break;
            case State.GAME_WON:
                stateTimer -= Time.deltaTime;
                SetGameWin();
                if (stateTimer <= 0)
                {
                    UIManager.Instance.ShowGameWin(false);
                    state = State.TITLE;
                }
                break;

            default:
                break;
        }
    }
    public void SetGameOver()
    {
        stopGameEvent.Notify();
        UIManager.Instance.ShowGameOver(true);
        gameMusic.Stop();
        state = State.GAME_OVER;
        stateTimer = 3;
    }

    public void SetGameWin()
    {
        stopGameEvent.Notify();
        UIManager.Instance.ShowGameWin(true);
        //gameMusic.Stop();
        //winnerMusic.Play();
        state = State.GAME_WON;
        stateTimer = 5;
        Debug.Log("Win!!!");
    }

    public void OnStartGame()
    {
        titleMusic.Stop();
        gameMusic.Play();
        state = State.START_GAME;
    }
}
