using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Button))]
public class Card : GameElement
{
    [SerializeField] private TextMeshProUGUI m_labelTextMesh = null;

    public System.Type CardType {
        get => m_type;
        set {
            if (value != null && value.IsSubclassOf(typeof(CardAction)) == false) {
                Debug.LogError($"Tried to set card type to illegal type {value}");
                return;
            }
            m_type = value;
            m_button.interactable = m_type != null;
            m_action = m_type == null ? null : (CardAction)System.Activator.CreateInstance(m_type);
            if( m_action != null)
                m_action.Owner = Owner;
            UpdateColor();
            UpdateInfo();
        }
    }

    public Player Owner {
        private get; set;
    }

    public UnityEvent m_onPlayed = new UnityEvent();

    Button m_button = null;
    private CardAction m_action = null;
    private System.Type m_type = null;
    private bool m_isCardActive = false;

    private void Activate() {
        if (m_action == null)
            return;
        m_action.Activate();
    }

    public bool IsCardActive {
        get => m_isCardActive;
        set {
            var rectTrans = GetComponent<RectTransform>();
            var height = rectTrans.rect.height / 2;
            var pos = rectTrans.anchoredPosition;
            m_isCardActive = value;
            Debug.Log($"Set card {name} " + (m_isCardActive ? "active" : "inactive"));
            pos.y = m_isCardActive ? Mathf.FloorToInt(height) / 2 : 0;
            if (Owner.Color == PlayerColor.White)
                pos.y = -pos.y;
            rectTrans.anchoredPosition = pos;

            if (m_isCardActive)
                Activate();
            else
                Board.instance.ResetTiles();
        }
    }

    public void Play(BoardTile a_tile) {
        var done = m_action.Execute(a_tile);
        if (done) {
            Board.instance.ActiveCard = null;
            CardType = null;
            Owner.PlayedCard();
        } else
            m_action.Activate();
    }

    private void SetDisasterDirection(Player a_player, BoardTile a_endTile) {
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Start() {
        UpdateInfo();

        m_button.onClick.AddListener(() => {
            Board.instance.ActiveCard = this;
        });
    }

    public void UpdatePlayable(bool m_playable) {
        if (m_action == null)
            return;
        var isPlayable = m_playable && m_action.IsPlayable;
        if (isPlayable) {
            Activate();
            if (Board.instance.ActiveTileCount == 0)
                isPlayable = false;
            Board.instance.ResetTiles();
            IsCardActive = false;
        }

        m_button.interactable = isPlayable;
    }

    private void UpdateColor() {
        gameObject.SetActive(m_action != null);
        if (m_action == null)
            return;

        m_button.interactable = m_action.IsPlayable;
        var color = (m_action == null) ? Color.black : m_action.Color;
        var colors = m_button.colors;
        colors.highlightedColor = color;
        m_labelTextMesh.color = color;
        colors.selectedColor = color;
        m_button.colors = colors;
    }

    public void UpdateInfo() {
        m_infoText = "";
        if (m_labelTextMesh != null)
            m_labelTextMesh.text = "X";
        if (m_action == null)
            return;
        //m_infoText = $"Owner: {Owner.name}\n";
        m_infoText += m_action.Info;
        if (m_action.IsPlayable == false)
            m_infoText += "\n(unplayable)";
        m_labelTextMesh.text = m_action.Initial;
    }
}
