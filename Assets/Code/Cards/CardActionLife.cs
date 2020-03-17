using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionLife: CardAction
{
    public CardActionLife() : this(null) { }
    public CardActionLife(Player a_owner) : base(a_owner) {
        Color = Color.green;
    }

    public override void Activate() {
        Board.instance.ToggleTiles((t) => {
            return t.HasAdjacentOrthogonalStone(Owner) && t.IsClear;
        });
    }

    public override bool Execute(BoardTile a_tile) {
        Life(a_tile);
        return true;
    }
}

