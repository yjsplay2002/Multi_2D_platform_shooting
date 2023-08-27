using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace UI
{
    public class Scoreboard : MonoBehaviourPunCallbacks
    {
        [SerializeField] Transform container;
        [SerializeField] GameObject scoreboardItemPrefab;

        Dictionary<Photon.Realtime.Player, ScoreboardItem> scoreboardItems = new Dictionary<Photon.Realtime.Player, ScoreboardItem>();

        public override void OnJoinedRoom()
        {
            foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                AddScoreboardItem(player);
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            AddScoreboardItem(newPlayer);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RemoveScoreboardItem(otherPlayer);
        }

        void AddScoreboardItem(Photon.Realtime.Player player)
        {
            ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
            item.Initialize(player);
            scoreboardItems[player] = item;
        }

        void RemoveScoreboardItem(Photon.Realtime.Player player)
        {
            Destroy(scoreboardItems[player].gameObject);
            scoreboardItems.Remove(player);
        }

    }
}