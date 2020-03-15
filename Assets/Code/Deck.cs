using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private bool m_debug = false;
    private List<CardType> m_cardList = new List<CardType>();

    public CardType this[int i] {
        get {
            if (m_cardList == null) {
                Debug.LogError("Null card list");
                return CardType.None;
            }

            if (i < 0 || i >= CardCount) {
                Debug.LogError($"[Deck] Card index {i} invalid");
                return CardType.None;
            }
            return m_cardList[i];
        }
        private set {
            if (i < 0 || i >= CardCount) {
                Debug.LogError($"[Deck] Card index {i} invalid");
                return;
            }

            m_cardList[i] = value;
        }
    }

    public int CardCount => m_cardList.Count;

    public override string ToString() {
        if (CardCount == 0)
            return "[empty deck]";
        var str = "";
        foreach (var card in m_cardList)
            str += $"{card.ToString()}, ";
        return str.Substring(0, str.Length - 2);
    }

    public int CountCardsOfType(CardType a_card) {
        var count = 0;
        foreach (var card in m_cardList)
            if (card == a_card)
                ++count;
        return count;
    }

    public void Add(CardType a_card) {
        m_cardList.Add(a_card);
    }

    public void Clear() {
        m_cardList.Clear();
    }

    public CardType Draw(bool a_keep = false) {
        var ret = m_cardList[0];
        if (a_keep == false)
            m_cardList.RemoveAt(0);
        return ret;
    }

    public void Shuffle() {
        Utility.Shuffle(m_cardList);
    }
}

