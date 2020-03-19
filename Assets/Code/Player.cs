using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerColor
{
    None = 0x00,
    Black = 0x01,
    White = 0x02
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
    [SerializeField] private int m_cardExtendCount = 4;
    [SerializeField] private int m_cardWildCount = 1;
    [SerializeField, Tooltip("All disasters plus Spread")] private int m_cardDisasterCount = 4;

    public PlayerColor Color => m_color;
    public Disaster ControlledDisaster { get; private set; } = null;
    public bool HasControlledDisaster => ControlledDisaster != null;
    public int Score {
        get => m_score;
        set {
            m_score = value;
            m_scoreTextMesh.text = $"{m_color} Alive: {m_score} / Dead: {m_deadCount}";
        }
    }

    private int m_cardsPlayed = -1;
    private int m_deadCount = 0;
    private int m_score = 0;
    private Deck<System.Type> m_deck = new Deck<System.Type>();
    private Card[] m_handVisual = null;

    public bool CardsEnabled {
        set {
            if (m_isHuman == false)
                return;
            foreach (var card in m_handVisual)
                card.UpdatePlayable(value);
        }
    }

    public void ClearDisaster() {
        ControlledDisaster.Head.Controller = null;
        ControlledDisaster = null;
    }

    public void CreateDisaster(DisasterType a_disasterType, BoardTile a_tile) {
        if (ControlledDisaster != null) {
            Debug.LogError("Tried to create disaster illegally");
            return;
        }
        // TODO set direction
        ControlledDisaster = new Disaster(a_disasterType, a_tile);
        a_tile.Controller = this;
        a_tile.Disaster = ControlledDisaster;
    }

    public void Kill() {
        ++m_deadCount;
    }

    public void PlayedCard() {
        ++m_cardsPlayed;
        UpdateHand();
        Board.instance.UpdateScore();
        if (m_cardsPlayed >= Board.instance.PlayPerTurn) {
            EndTurn();
            return;
        }
        StartCoroutine(Board.instance.WaitForBlinkToEnd());
    }

    private void UpdateHand() {
        foreach (var card in m_handVisual)
            card.UpdateInfo();
    }

    public void StartTurn() {
        DrawCards();
        if (m_isHuman)
            UpdateHand();
        else
            // TODO AI here
            EndTurn();
    }

    private void DrawCards() {
        if (m_deck.CardCount == 0) {
            Board.instance.EndGame();
            return;
        }
        var count = (m_cardsPlayed == -1) ? Board.instance.HandSize : m_cardsPlayed;
        if( m_deck.CardCount < count)
            count = m_deck.CardCount;
        Debug.Log($"Draw {count} cards");
        for (var i = 0; i < count; ++i) {
            var cardType = m_deck.Draw();

            if (m_isHuman) {
                for (var k = 0; k < m_handVisual.Length; ++k) {
                    if (m_handVisual[k].CardType == null) {
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

    public void EndTurn() {
        Board.instance.NextTurn();
    }

    private void Awake() {
        for (var i = 0; i < m_cardMoveCount; ++i)
            m_deck.Add(typeof(CardActionMove));
        for (var i = 0; i < m_cardLifeCount; ++i)
            m_deck.Add(typeof(CardActionLife));
        for (var i = 0; i < m_cardExtendCount; ++i)
            m_deck.Add(typeof(CardActionExtend));
        for (var i = 0; i < m_cardDisasterCount; ++i) {
            m_deck.Add(typeof(CardActionFire));
            m_deck.Add(typeof(CardActionWater));
            //m_deck.Add(CardType.Plague);
            m_deck.Add(typeof(CardActionSpread));
        }
        /*
        for (var i = 0; i < m_cardWildCount; ++i)
            m_deck.Add(CardType.Wild);
            */

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
