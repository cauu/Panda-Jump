using System;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;


    public enum State { StartMenu, Result, Play, Pause };

    public State curState;

    public Canvas uiStartMenu;

    // UI Components
    public RectTransform startButtonRect;

    [HideInInspector] public int playerHp;

    void Awake () {
        instance = this;

        Application.targetFrameRate = 60;

        curState = State.StartMenu;

        playerHp = 3;

        uiStartMenu.enabled = true;
    }

    void Update () {
        if (isButtonTapped(startButtonRect)) {
            EnterGame();
        }

        if (curState == State.StartMenu) {
            uiStartMenu.enabled = true;
        } else {
            uiStartMenu.enabled = false;
        }
    }

    public bool isButtonTapped (RectTransform btnPos) {
        Vector3 mousePos = Input.mousePosition;

        print(mousePos);
        print(btnPos);
        print(BasicFunctions.ScreenPointInRectTransform(mousePos, btnPos));

        return BasicFunctions.ScreenPointInRectTransform(mousePos, btnPos);
    }

    public void ChangeState (State newState) {
        curState = newState;
    }

    public void EnterGame () {
        ChangeState (State.Play);
    }

    public void PlayerHit () {
        playerHp--;
        if (playerHp <= 0) {
            ChangeState (State.Result);
        }
    }

    public void PlayerHeal () {
        playerHp++;
    }
}