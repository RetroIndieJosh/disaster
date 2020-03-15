using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerColor
{
    Black = 0x00,
    White = 0x01
}

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerColor m_color = PlayerColor.Black;

    public PlayerColor Color => m_color;
    private Deck m_deck = new Deck();
    private Deck m_hand = new Deck();
    private Card[] m_handVisual = null;

    private void DrawCards() {
        var count = TurnManager.instance.HandSize - m_hand.CardCount;
        for (var i = 0; i < count; ++i) {
            var cardType = m_deck.Draw();
            m_hand.Add(cardType);

            for (var k = 0; k < m_handVisual.Length; ++k) {
                if (m_handVisual[k].CardType == CardType.None) {
                    m_handVisual[k].CardType = cardType;
                    break;
                }
            }
        }
    }

    private void Start() {
        for (var i = 0; i < 4; ++i) {
            m_deck.Add(CardType.Move);
            m_deck.Add(CardType.Life);
            m_deck.Add(CardType.Step);
        }
        for (var i = 0; i < 2; ++i) {
            m_deck.Add(CardType.Fire);
            m_deck.Add(CardType.Water);
            m_deck.Add(CardType.Plague);
            m_deck.Add(CardType.Spread);
        }
        m_deck.Add(CardType.Wild);

        m_deck.Shuffle();
        InitializeVisualHand();

        DrawCards();
        Debug.Log($"Hand: {m_hand.ToString()}");
    }

    private void InitializeVisualHand() {
        m_handVisual = new Card[TurnManager.instance.HandSize];
        var pos = Vector2Int.zero;
        var cardPrefab = TurnManager.instance.CardPrefab;
        var cardWidth = Mathf.FloorToInt(cardPrefab.GetComponent<RectTransform>().rect.width);
        for (var i = 0; i < TurnManager.instance.HandSize; ++i) {
            var card = Instantiate(TurnManager.instance.CardPrefab, transform);
            var rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = pos;
            pos.x += cardWidth;
            m_handVisual[i] = card;
        }
    }
}
