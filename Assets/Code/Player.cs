using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Card
{
    None = 0x00,
    Move = 0x01, // move up to two
    Life = 0x02, // add one
    Step = 0x04, // advance one
    Spread = 0x08, // advance two
    Fire = 0x0F,
    Water = 0x10,
    Plague = 0x11,
    Wild = 0x12,
    Reserved1 = 0x14,
    Reserved2 = 0x18,
    Reserved3 = 0x1F
}

public class Deck
{
    [SerializeField]
    private bool m_debug = false;

    [SerializeField]
    private List<Card> m_cardList = new List<Card>();

    public Card this[int i] {
        get {
            if (m_cardList == null) {
                Debug.LogError("Null card list");
                return Card.None;
            }

            if (i < 0 || i >= CardCount) {
                Debug.LogError($"[Deck] Card index {i} invalid");
                return Card.None;
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
        var str = "";
        foreach (var card in m_cardList)
            str += $"{card.ToString()}, ";
        return str.Substring(0, str.Length - 2);
    }

    public int CountCardsOfType(Card a_card) {
        var count = 0;
        foreach (var card in m_cardList)
            if (card == a_card)
                ++count;
        return count;
    }

    public void Add(Card a_card) {
        m_cardList.Add(a_card);
    }

    public void Clear() {
        m_cardList.Clear();
    }

    public Card Draw(bool a_keep = false) {
        var ret = m_cardList[0];
        if (a_keep == false)
            m_cardList.RemoveAt(0);
        return ret;
    }

    public void Shuffle() {
        Utility.Shuffle(m_cardList);
    }
}

public class Player : MonoBehaviour
{
    private Deck m_deck = new Deck();
    private Deck m_hand = new Deck();

    private void DrawCards(int a_count) {
        for (var i = 0; i < a_count; ++i)
            m_hand.Add(m_deck.Draw());
    }

    private void Start() {
        for (var i = 0; i < 4; ++i) {
            m_deck.Add(Card.Move);
            m_deck.Add(Card.Life);
            m_deck.Add(Card.Step);
        }
        for (var i = 0; i < 2; ++i) {
            m_deck.Add(Card.Fire);
            m_deck.Add(Card.Water);
            m_deck.Add(Card.Plague);
            m_deck.Add(Card.Spread);
        }
        m_deck.Add(Card.Wild);

        m_deck.Shuffle();
        DrawCards(5);

        Debug.Log($"Hand: {m_hand.ToString()}");
    }
}
