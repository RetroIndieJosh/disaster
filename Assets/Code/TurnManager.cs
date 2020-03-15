using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance = null;

    [SerializeField] private Stone m_stonePrefabBlack = null;
    [SerializeField] private Stone m_stonePrefabWhite = null;

    private Player m_activePlayer = null;
    private Board m_board = null;

    public Card ActiveCard = null;

    private List<Player> m_playerList = new List<Player>();

    public void AddStone(int a_x, int a_y, PlayerColor a_color) {
        var parent = m_board.GetTile(a_x, a_y);
        var stone = (a_color == PlayerColor.Black) ? m_stonePrefabBlack : m_stonePrefabWhite;
        Instantiate(stone, parent.transform);
    }

    public void AddPlayer(Player a_player) {
        if (m_playerList.Contains(a_player)) {
            Debug.LogWarning($"Tried to add player {a_player} multiple times.");
            return;
        }

        m_playerList.Add(a_player);
    }

    public void ClickTile(int a_x, int a_y) {
        ActiveCard.Activate(a_x, a_y, m_activePlayer.Color);
    }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
