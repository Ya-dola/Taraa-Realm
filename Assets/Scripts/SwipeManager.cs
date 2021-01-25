using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;

public class SwipeManager : MonoBehaviour
{
    public void SwipeHandler(string id)
    {
        // To Only allow Player Movement if the Player is not recovering
        if (GameManager.singleton.playerRecovered)
        {
            switch (id)
            {
                case DirectionId.ID_UP:
                    GameManager.singleton.MovePlayerUp();
                    // Debug.Log("Up");
                    break;

                case DirectionId.ID_DOWN:
                    GameManager.singleton.MovePlayerDown();
                    // Debug.Log("Down");
                    break;

                case DirectionId.ID_RIGHT:
                    GameManager.singleton.MovePlayerRight();
                    // Debug.Log("Right");
                    break;

                case DirectionId.ID_LEFT:
                    GameManager.singleton.MovePlayerLeft();
                    // Debug.Log("Left");
                    break;
            }
        }
    }
}
