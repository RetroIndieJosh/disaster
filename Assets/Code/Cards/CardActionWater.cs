using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardActionWater : CardActionDisaster
{
    public CardActionWater() : base(DisasterType.Water, Board.instance.WaterSpeed) {
        Color = Color.blue;
        Info = "Disaster ~ Water\nCreate water adjacent to your stone\nAdvance water x2 or fire x1\nTurn on advance\nWater kills fire\nWater can pass through itself";
        Initial = "W";
    }
}
