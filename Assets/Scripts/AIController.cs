using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : IPlayerController
{
    private class Cell
    {
        public int _xCoord;
        public int _yCoord;
        public int? _playerIndex;
        public Cell(int xCoord, int yCoord, int? playerIndex)
        {
            _xCoord = xCoord;
            _yCoord = yCoord;
            _playerIndex = playerIndex;
        }
    }

    private List<Cell> _freeCells;
    private List<Cell> _playerCells;
    private Cell[,] _cells;

    private List<Cell> _cellsForRandom = new List<Cell>();
    public AIController(int aiLevel,int aiPlayerIndex)
    {
        this._aiLevel = aiLevel;
        this._aiPlayerIndex = aiPlayerIndex;

        _cells = new Cell[3, 3];
        _freeCells = new List<Cell>();
        _playerCells = new List<Cell>();
        for (int i = 0;i<3;i++)
        {
            for(int j=0;j<3;j++)
            {
                _cells[i, j] = new Cell(i, j, null);
                _freeCells.Add(_cells[i, j]);
            }
        }
    }
    private int _aiLevel;

    private int _aiPlayerIndex;

    private bool _hasFirstPat = false;
    private const float _aiRandomLevel=0.5f;

    public void UpdateFieldConfiguration(int coordX, int coordY,int playerIndex)
    {
        _cells[coordX, coordY]._playerIndex = playerIndex;
        if(playerIndex!=_aiPlayerIndex && _playerCells.Count<3)
        {
            _playerCells.Add(_cells[coordX, coordY]);
        }
        _freeCells.Remove(_cells[coordX, coordY]);
    }
    public Vector2Int GetNextMove()
    {
        Cell nextCell=null;
        //preventing player from wining instantly
        if (!_hasFirstPat)
            nextCell = GetBestFreeCell();
        else
        {
            if(_aiLevel==0)
            {
                //then on easy level simple random
                nextCell = GetRandomFreeCell();
            }
            else if(_aiLevel == 1)
            {
                //and on hard AI can make random or smart moves with configured probability
                float randomVer = Random.Range(0f, 1f);
                if(randomVer < _aiRandomLevel)
                {
                    nextCell = GetRandomFreeCell();
                }
                else
                {
                    nextCell = GetBestFreeCell();
                }
            }
            else
            {
                nextCell = GetBestFreeCell();
            }
        }


        Vector2Int nextMove = Vector2Int.zero;
        if (nextCell != null)
        {
            nextMove = new Vector2Int(nextCell._xCoord, nextCell._yCoord);
            UpdateFieldConfiguration(nextCell._xCoord, nextCell._yCoord, _aiPlayerIndex);
            
        }
            return nextMove;
    }
    private Cell GetRandomFreeCell()
    {
        var index=Random.Range(0, _freeCells.Count);
        var cell = _freeCells[index];

        return cell;
        
    }
    public void SetAIParameters(int aiLevel, int aiPlayerIndex)
    {
        this._aiLevel = aiLevel;
        this._aiPlayerIndex = aiPlayerIndex;
    }


    private Cell GetBestFreeCell()
    {

        bool isNotRowBroken = true;
        bool isNotColBroken = true;
        bool isNotDiag1Broken = true;
        bool isNotDiag2Broken = true;

        Cell emptyCellInRow = null;
        Cell emptyCellInCol = null;
        Cell emptyCellInDiag1 = null;
        Cell emptyCellInDiag2 = null;


        //if nobody doesn't took center - taking it
        if (_cells[1, 1]._playerIndex == null)
        {
            return _cells[1, 1];
        }
        //check if ai has two-in-a-line

        isNotDiag1Broken = true;
        isNotDiag2Broken = true;

        emptyCellInDiag1 = null;
        emptyCellInDiag2 = null;

        for (var i = 0; i < 3; i++)
        {
            isNotRowBroken = true;
            isNotColBroken = true;


            emptyCellInRow = null;
            emptyCellInCol = null;


            for (var j = 0; j < 3; j++)
            {

                if (isNotRowBroken)
                    isNotRowBroken = isNotRowBroken
                    && !(_cells[i, j]._playerIndex != null && _cells[i, j]._playerIndex != _aiPlayerIndex)
                    && !(_cells[i, j]._playerIndex == null && emptyCellInRow != null);

                if (isNotColBroken)
                    isNotColBroken = isNotColBroken
                    && !(_cells[j, i]._playerIndex != null && _cells[j, i]._playerIndex != _aiPlayerIndex)
                    && !(_cells[j, i]._playerIndex == null && emptyCellInCol != null);

                if (!isNotRowBroken && !isNotColBroken)
                {
                    break;
                }

                if (isNotRowBroken && _cells[i, j]._playerIndex == null)
                    emptyCellInRow = _cells[i, j];
                if (isNotColBroken && _cells[j, i]._playerIndex == null)
                    emptyCellInCol = _cells[j, i];
            }

            if (isNotRowBroken)
            {
               
                return emptyCellInRow;
            }
            else if (isNotColBroken)
            {
                
                return emptyCellInCol;
            }
            else
            {
                //checking diagonals
                if (isNotDiag1Broken)
                {
                    isNotDiag1Broken = isNotDiag1Broken
                    && !(_cells[i, i]._playerIndex != null && _cells[i, i]._playerIndex != _aiPlayerIndex)
                    && !(_cells[i, i]._playerIndex == null && emptyCellInDiag1 != null);
                }
                if (isNotDiag2Broken)
                {
                    isNotDiag2Broken = isNotDiag2Broken
                        && !(_cells[i, 2 - i]._playerIndex != null && _cells[i, 2 - i]._playerIndex != _aiPlayerIndex)
                        && !(_cells[i, 2 - i]._playerIndex == null && emptyCellInDiag2 != null);
                }
                if (!isNotDiag1Broken && !isNotDiag2Broken)
                    continue;
                if (isNotDiag1Broken && _cells[i, i]._playerIndex == null)
                    emptyCellInDiag1 = _cells[i, i];
                if (isNotDiag2Broken && _cells[i, 2 - i]._playerIndex == null)
                    emptyCellInDiag2 = _cells[i, 2 - i];

            }

        }
        if (isNotDiag1Broken)
        {
            return emptyCellInDiag1;
        }
        else if (isNotDiag2Broken)
        {
            return emptyCellInDiag2;
        }


        //if not check if player has two-in-line
        isNotDiag1Broken = true;
        isNotDiag2Broken = true;

        emptyCellInDiag1 = null;
        emptyCellInDiag2 = null;

        for (var i=0;i<3;i++)
        {
            isNotRowBroken = true;
            isNotColBroken = true;
            

            emptyCellInRow = null;
            emptyCellInCol = null;
            

            for (var j = 0; j < 3; j++)
            {
                
                if (isNotRowBroken)
                    isNotRowBroken = isNotRowBroken 
                    && (_cells[i, j]._playerIndex != _aiPlayerIndex)
                    && !(_cells[i, j]._playerIndex == null && emptyCellInRow !=null);

                if (isNotColBroken)
                    isNotColBroken = isNotColBroken 
                    && (_cells[j, i]._playerIndex != _aiPlayerIndex) 
                    && !(_cells[j, i]._playerIndex == null && emptyCellInCol != null);

                if (!isNotRowBroken && !isNotColBroken)
                {
                    break;
                }

                if (isNotRowBroken && _cells[i, j]._playerIndex == null)
                    emptyCellInRow = _cells[i, j];
                if(isNotColBroken && _cells[j, i]._playerIndex == null)
                    emptyCellInCol = _cells[j, i];
            }

            if (isNotRowBroken)
            {
                if (!_hasFirstPat)
                    _hasFirstPat = true;
                
                return emptyCellInRow;
            }
            else if (isNotColBroken)
            {
                
                if (!_hasFirstPat)
                    _hasFirstPat = true;
                return emptyCellInCol;
            }
            else 
            {
                //checking diagonals
                if (isNotDiag1Broken)
                {
                    isNotDiag1Broken = isNotDiag1Broken
                    && (_cells[i, i]._playerIndex != _aiPlayerIndex)
                    && !(_cells[i, i]._playerIndex == null && emptyCellInDiag1 != null);
                }
                if (isNotDiag2Broken)
                {
                    isNotDiag2Broken = isNotDiag2Broken
                        && (_cells[i, 2 - i]._playerIndex != _aiPlayerIndex)
                        && !(_cells[i, 2 - i]._playerIndex == null && emptyCellInDiag2 != null);
                }
                if (!isNotDiag1Broken && !isNotDiag2Broken)
                    continue;
                if (isNotDiag1Broken && _cells[i, i]._playerIndex == null)
                    emptyCellInDiag1 = _cells[i, i];
                if (isNotDiag2Broken && _cells[i, 2-i]._playerIndex == null)
                    emptyCellInDiag2 = _cells[i, 2-i];

            }

        }
        if(isNotDiag1Broken)
        {
            if (!_hasFirstPat)
                _hasFirstPat = true;
            return emptyCellInDiag1;
        }
        else if(isNotDiag2Broken)
        {
            if (!_hasFirstPat)
                _hasFirstPat = true;
            return emptyCellInDiag2;
        }
        //else checking game patterns
        //if player in center - take corners
        if (_cells[1,1]._playerIndex!=null && _cells[1, 1]._playerIndex != _aiPlayerIndex)
        {
            for(var i = 0;i<3;i=i+2)
            {
                for (var j = 0; j < 3; j=j+2)
                {

                    if (_cells[i, j]._playerIndex == null && !_cellsForRandom.Contains(_cells[i, j]))
                        _cellsForRandom.Add(_cells[i, j]);
                }
            }
            if (_cellsForRandom.Count > 0)
            {
                var rand = Random.Range(0, _cellsForRandom.Count);
                var cell = _cellsForRandom[rand];
                _cellsForRandom.Clear();
                return cell;
            }
        }
        //another protection from possible player schemes
        if (_playerCells.Count==2)
        {

            int fixX;
            int fixY;
            if ((_playerCells[0]._xCoord %2 ==0) && (_playerCells[1]._yCoord % 2==0))
            {
                fixX = _playerCells[0]._xCoord;
                fixY= _playerCells[1]._yCoord;
                //if player took diagonal cells - don't take corners
                if((_playerCells[1]._xCoord % 2 == 0) && (_playerCells[0]._yCoord % 2 == 0))
                {
                    if (_cells[fixX, 1]._playerIndex == null && !_cellsForRandom.Contains(_cells[fixX, 1]))
                        _cellsForRandom.Add(_cells[fixX, 1]);
                    else if (_cells[1, fixY]._playerIndex == null && !_cellsForRandom.Contains(_cells[1, fixY]))
                        _cellsForRandom.Add(_cells[1, fixY]);
                }
                else
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (_cells[fixX, i]._playerIndex == null && !_cellsForRandom.Contains(_cells[fixX, i]))
                            _cellsForRandom.Add(_cells[fixX, i]);
                        else if (_cells[i, fixY]._playerIndex == null && !_cellsForRandom.Contains(_cells[i, fixY]))
                            _cellsForRandom.Add(_cells[i, fixY]);

                    }
                }
                
            }
            if ((_playerCells[1]._xCoord % 2 == 0) && (_playerCells[0]._yCoord % 2 == 0))
            {
                fixX = _playerCells[1]._xCoord;
                fixY = _playerCells[0]._yCoord;
                if ((_playerCells[0]._xCoord % 2 == 0) && (_playerCells[1]._yCoord % 2 == 0))
                {
                    if (_cells[fixX, 1]._playerIndex == null && !_cellsForRandom.Contains(_cells[fixX, 1]))
                        _cellsForRandom.Add(_cells[fixX, 1]);
                    else if (_cells[1, fixY]._playerIndex == null && !_cellsForRandom.Contains(_cells[1, fixY]))
                        _cellsForRandom.Add(_cells[1, fixY]);
                }
                else
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (_cells[fixX, i]._playerIndex == null && !_cellsForRandom.Contains(_cells[fixX, i]))
                            _cellsForRandom.Add(_cells[fixX, i]);
                        else if (_cells[i, fixY]._playerIndex == null && !_cellsForRandom.Contains(_cells[i, fixY]))
                            _cellsForRandom.Add(_cells[i, fixY]);
                    }
                }
            }
            if(_cellsForRandom.Count>0)
            {
                var rand = Random.Range(0, _cellsForRandom.Count);
                var cell = _cellsForRandom[rand];
                _cellsForRandom.Clear();
                return cell;
            }
   
        }


        //else random move

        var index = Random.Range(0, _freeCells.Count);

        return _freeCells[index];;
    }

}
