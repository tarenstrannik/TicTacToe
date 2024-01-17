using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController 
{
    void UpdateFieldConfiguration(int coordX,int coordY, int playerIndex);
    Vector2Int GetNextMove();
    void SetAIParameters(int aiLevel,int aiPlayerIndex);

}
