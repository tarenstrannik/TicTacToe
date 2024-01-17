using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	private bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.cross;
	TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	int[,] _marks;
	int _turnsCount=0;
    bool _isGameOver = false;

    private IPlayerController _player2Controller;
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
        onPlayerWin.AddListener(GameOver);
    }

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
        _player2Controller = new AIController(_aiLevel,1) as IPlayerController;

        StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
        _marks = new int[3, 3]
        {
			{ 0,0,0 },
			{ 0,0,0 },
			{ 0,0,0 },
		};

        onGameStarted.Invoke();
	}
    private void GameOver(int value)
    {
        _isGameOver = true;
    }

	public void PlayerSelects(int coordX, int coordY){

		if (_isPlayerTurn)
		{
            if (_marks[coordX, coordY] != 0)
                return;

			SetVisual(coordX, coordY, playerState);
			
            _marks[coordX, coordY] = 2;
            _player2Controller.UpdateFieldConfiguration(coordX, coordY,0);
            _isPlayerTurn = false;
			CheckVictory();
			if (!_isGameOver) AIturn();
        }
	}

	public void AiSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, aiState);
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}

	private void AIturn()
	{
        
        var nextMove = _player2Controller.GetNextMove();
        AiSelects(nextMove.x, nextMove.y);
        _marks[nextMove.x, nextMove.y] = 1;
        CheckVictory();
        if (!_isGameOver) _isPlayerTurn = true;
    }
	private void CheckVictory()
	{
		_turnsCount++;
        
        if (_turnsCount>4)
		{
			//checking rows and columns
			var winnerCountRow = 1;
            var winnerCountCol = 1;
			bool hasWinnerRow = true;
            bool hasWinnerCol = true;

            var hasWinnerDiagonal1 = true;
            var hasWinnerDiagonal2 = true;

            var winnerCountDiagonal1 = 1;
            var winnerCountDiagonal2 = 1;
            for (int i=0;i<3;i++)
			{
                hasWinnerRow = true;
                hasWinnerCol = true;

                winnerCountRow = 1;
                winnerCountCol = 1;
                for (int j=0;j<3;j++)
				{
                    //checking rows and columns
                    if (hasWinnerRow)
					{
                        winnerCountRow *= _marks[i, j];
                        hasWinnerRow = hasWinnerRow && (winnerCountRow != 0);
                    }
					if (hasWinnerCol)
					{
						winnerCountCol *= _marks[j, i];
						hasWinnerCol = hasWinnerCol && (winnerCountCol != 0);
					}

                    if (!hasWinnerRow && !hasWinnerCol) break;
                }
                
				if(hasWinnerRow)
				{
					if(winnerCountRow == 1)
					{
						onPlayerWin.Invoke(1);
                        break;
                    }
					else if(winnerCountRow == 8)
					{
                        onPlayerWin.Invoke(0);
                        break;
                    }
				}
				if(hasWinnerCol)
                {
                    if (winnerCountCol == 1)
                    {
                        onPlayerWin.Invoke(1);
                        break;
                    }
                    else if (winnerCountCol == 8)
                    {
                        onPlayerWin.Invoke(0);
                        break;
                    }
                }
                //checking diagonals
                if (hasWinnerDiagonal1)
                {
                    winnerCountDiagonal1 *= _marks[i, i];
                    hasWinnerDiagonal1 = hasWinnerDiagonal1 && (winnerCountDiagonal1 != 0);
                }
                if (hasWinnerDiagonal2)
                {
                    winnerCountDiagonal2 *= _marks[i, 2 - i];
                    hasWinnerDiagonal2 = hasWinnerDiagonal2 && (winnerCountDiagonal2 != 0);
                }
            }
            if (!_isGameOver)
            {
                if (hasWinnerDiagonal1)
                {
                    if (winnerCountDiagonal1 == 1)
                    {
                        onPlayerWin.Invoke(1);

                    }
                    else if (winnerCountDiagonal1 == 8)
                    {
                        onPlayerWin.Invoke(0);
                    }
                }
                if (hasWinnerDiagonal2)
                {
                    if (winnerCountDiagonal2 == 1)
                    {
                        onPlayerWin.Invoke(1);


                    }
                    else if (winnerCountDiagonal2 == 8)
                    {
                        onPlayerWin.Invoke(0);

                    }
                }
            }
        }
		if(!_isGameOver&& _turnsCount > 8)
		{
            onPlayerWin.Invoke(-1);

		}
    }
}

