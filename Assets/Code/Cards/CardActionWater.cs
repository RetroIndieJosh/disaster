using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionWater : CardActionDisaster
{
    public CardActionWater() : base(DisasterType.Water, 1) {
        Color = Color.blue;
        Info = "Disaster ~ Water";
        Initial = "W";
    }
}
