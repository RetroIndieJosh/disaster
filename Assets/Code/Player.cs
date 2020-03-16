using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerColor
{
    Black = 0x00,
    White = 0x01
}

public class Player : MonoBehaviour
{
    [SerializeField] private bool m_isHuman = true;
    [SerializeField] private PlayerColor m_color = PlayerColor.Black;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_deckTextMesh = null;
    [SerializeField] private TextMeshProUGUI m_scoreTextMesh = null;

    [Header("Deck")]
    [SerializeField] private int m_cardMoveCount = 4;
    [SerializeField] private int m_cardLifeCount = 4;
    [SerializeField] private int m_cardStepCount = 4;
    [SerializeField] private int m_cardWildCount = 1;
    [SerializeField, Tooltip("All disasters plus Spread")] private int m_cardDisasterCount = 4;

    public PlayerColor Color => m_color;
    public int Score {
        get => m_score;
        set {
            m_score = value;
            m_scoreTextMesh.text = $"{m_color} Score: {m_score}";
        }
    }

    private int m_score = 0;
    private Deck m_deck = new Deck();
    private Card[] m_handVisual = null;

    public bool CardsEnabled {
        set {
            if (m_isHuman == false)
                return;
            foreach (var card in m_handVisual)
                card.GetComponent<Button>().interactable = value;
        }
    }

    private int m_cardsPlayed = 0;

    public void PlayedCard() {
        ++m_cardsPlayed;
        Board.instance.UpdateScore();
        if (m_cardsPlayed >= Board.instance.PlayPerTurn)
            EndTurn();
        else
            CardsEnabled = true;
    }

    public void StartTurn() {
        CardsEnabled = true;
        DrawCards();
        if (m_isHuman == false)
            EndTurn();
    }

    private void DrawCards() {
        if (m_deck.CardCount == 0) {
            Board.instance.EndGame();
            return;
        }
        var count = (m_cardsPlayed == 0) ? Board.instance.HandSize : m_cardsPlayed;
        if( m_deck.CardCount < count)
            count = m_deck.CardCount;
        Debug.Log($"Draw {count} cards");
        for (var i = 0; i < count; ++i) {
            var cardType = m_deck.Draw();

            if (m_isHuman) {
                for (var k = 0; k < m_handVisual.Length; ++k) {
                    if (m_handVisual[k].CardType == CardType.None) {
                        m_handVisual[k].CardType = cardType;
                        Debug.Log($"Drew {cardType} in visual slot {k}");
                        break;
                    } else
                        Debug.Log($"Card {k} is {m_handVisual[k].CardType}");
                } 
            }
        }
        m_cardsPlayed = 0;

        m_deckTextMesh.text = (m_deck.CardCount == 0) 
            ? $"Last turn!" 
            : $"Deck: {m_deck.CardCount}";
    }

    private void EndTurn() {
        CardsEnabled = false;
        Board.instance.NextTurn();
    }

    private void Awake() {
        for (var i = 0; i < m_cardMoveCount; ++i)
            m_deck.Add(CardType.Move);
        for (var i = 0; i < m_cardLifeCount; ++i)
            m_deck.Add(CardType.Life);
        for (var i = 0; i < m_cardStepCount; ++i)
            m_deck.Add(CardType.Step);
        for (var i = 0; i < m_cardDisasterCount; ++i) {
            m_deck.Add(CardType.Fire);
            m_deck.Add(CardType.Water);
            m_deck.Add(CardType.Plague);
            m_deck.Add(CardType.Spread);
        }
        for (var i = 0; i < m_cardWildCount; ++i)
            m_deck.Add(CardType.Wild);

        m_deck.Shuffle();
        InitializeVisualHand();
    }

    private void InitializeVisualHand() {
        if (m_isHuman == false)
            return;

        m_handVisual = new Card[Board.instance.HandSize];
        var pos = Vector2Int.zero;
        var cardPrefab = Board.instance.CardPrefab;
        var cardWidth = Mathf.FloorToInt(cardPrefab.GetComponent<RectTransform>().rect.width);
        for (var i = 0; i < Board.instance.HandSize; ++i) {
            var card = Instantiate(Board.instance.CardPrefab, transform);
            card.Owner = this;
            var rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = pos;
            pos.x += cardWidth;
            m_handVisual[i] = card;

            card.GetComponent<Button>().interactable = false;
        }
    }
}
