using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionFire : CardActionDisaster
{
    public CardActionFire() : base(DisasterType.Fire, 2) {
        Color = Color.red;
        Info = "Disaster ~ Fire";
        Initial = "F";
    }
}
