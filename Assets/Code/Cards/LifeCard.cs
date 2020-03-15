using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCard : Card
{
    public override void Activate(int a_x, int a_y, PlayerColor a_color) {
        TurnManager.instance.AddStone(a_x, a_y, a_color);
    }
}
