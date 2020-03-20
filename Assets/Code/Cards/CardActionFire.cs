using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardActionFire : CardActionDisaster
{
    public CardActionFire() : base(DisasterType.Fire, Board.instance.FireSpeed) {
        Color = Color.red;
        Info = "Disaster ~ Fire\nCreate fire adjacent to your stone\nAdvance fire x2 or water x1\nTurn on advance\nFire moves x2\nFire stops itself";
        Initial = "F";
    }
}
