using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionFire : CardActionDisaster
{
    public CardActionFire() : base(DisasterType.Fire, Board.instance.FireSpeed) {
        Color = Color.red;
        Info = "Disaster ~ Fire";
        Initial = "F";
    }
}
