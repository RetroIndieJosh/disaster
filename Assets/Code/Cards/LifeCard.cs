using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCard : Card
{
    public override void Activate(BoardTile a_tile, Player a_player) {
        a_tile.SetStone(a_player);
    }
}
