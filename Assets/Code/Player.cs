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
    public bool IsHuman {
        private get;
        set;
    } = false;
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
            var cardCount = 0;
            foreach (var card in m_handVisual) {
                card.UpdatePlayable(value);
                if (card.CardType != null)
                    ++cardCount;
            }
            if (value && cardCount == Board.instance.HandSize) {
                Board.instance.CheckGameOver();
                if (HasPlayableCard == false)
                    EndTurn();
            }
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

    public bool HasPlayableCard {
        get {
            foreach (var card in m_handVisual)
                if (card.IsPlayable)
                    return true;
            return false;
        }
    }

    public void PlayedCard() {
        ++m_cardsPlayed;
        UpdateHand();
        StartCoroutine(Board.instance.WaitForBlinkToEnd(CheckEndTurn));
    }

    private void CheckEndTurn() {
        if (Board.instance.IsGameOver)
            return;
        if (m_cardsPlayed >= Board.instance.PlayPerTurn) {
            Debug.Log("Played enough cards");
            EndTurn();
            return;
        }
        // not end of turn, update score (and check for game over due to no playable cards)
        // TODO separate game over check for score vs no playable
        Board.instance.UpdateScore();
        if (HasPlayableCard == false) {
            Debug.Log("No playable cards");
            EndTurn();
            return;
        }
    }

    private void UpdateHand(bool a_enable=true) {
        foreach (var card in m_handVisual) {
            card.UpdateInfo();
            card.UpdatePlayable(a_enable);
        }
    }

    public void StartTurn() {
        DrawCards(true);
        m_cardsPlayed = 0;
        Board.instance.CheckGameOver();
        if (IsHuman == false)
            StartCoroutine(RunAi());
    }

    private IEnumerator RunAi() {
        Board.instance.InfoText = "AI is thinking...";
        yield return new WaitForSeconds(Board.instance.BlinkTimeTotal * 1.5f);
        for (var i = 0; i < Board.instance.PlayPerTurn; i++) {
            var cardList = new List<Card>();
            cardList.AddRange(m_handVisual);
            cardList.RemoveAll(Card => Card.IsPlayable == false);
            if (cardList.Count == 0)
                yield break;
            var k = Random.Range(0, cardList.Count);
            var card = cardList[k];
            card.IsCardActive = true;
            Debug.Log($"AI: Activate {card}");
            Board.instance.InfoText = "AI is thinking...";
            yield return new WaitForSeconds(Board.instance.BlinkTimeTotal);

            // select start tile
            if (Board.instance.ActiveTileCount == 0)
                continue;
            var m = Random.Range(0, Board.instance.ActiveTileCount);
            var tile = Board.instance.PlayableTiles[m];
            card.Play(tile);
            Debug.Log($"AI: Play {card} on {tile}");
            Board.instance.InfoText = "AI is thinking...";
            yield return new WaitForSeconds(Board.instance.BlinkTimeTotal * 1.5f);

            // select second tile
            if (Board.instance.ActiveTileCount == 0)
                continue;
            var o = Random.Range(0, Board.instance.ActiveTileCount);
            var tile2 = Board.instance.PlayableTiles[o];
            card.Play(tile2);
            Debug.Log($"AI: Play #2 {card} on {tile2}");
            Board.instance.InfoText = "AI is thinking...";
            yield return new WaitForSeconds(Board.instance.BlinkTimeTotal * 1.5f);

            if (HasPlayableCard == false)
                yield break;
        }
    }

    public void DrawCards(bool a_enable) {
        if (m_deck.CardCount == 0) {
            Board.instance.EndGame();
            return;
        }
        var count = (m_cardsPlayed == -1) ? Board.instance.HandSize : m_cardsPlayed;
        if( m_deck.CardCount < count)
            count = m_deck.CardCount;
        //Debug.Log($"Draw {count} cards");
        for (var i = 0; i < count; ++i) {
            var cardType = m_deck.Draw();

            for (var k = 0; k < m_handVisual.Length; ++k) {
                if (m_handVisual[k].CardType == null) {
                    m_handVisual[k].CardType = cardType;
                    //Debug.Log($"Drew {cardType} in visual slot {k}");
                    break;
                } //else
                  //Debug.Log($"Card {k} is {m_handVisual[k].CardType}");
            }
        }

        m_deckTextMesh.text = (m_deck.CardCount == 0) 
            ? $"Last turn!" 
            : $"Deck: {m_deck.CardCount}";

        UpdateHand(a_enable);
    }

    public void EndTurn() {
        UpdateHand(false);
        Debug.Log("End turn");
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
