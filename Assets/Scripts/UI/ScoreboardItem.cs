using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;

namespace UI
{
    public class ScoreboardItem : MonoBehaviourPunCallbacks
    {
        public TMP_Text _usernameText;
        public TMP_Text _killDeathText;

        Photon.Realtime.Player _player;

        public void Initialize(Photon.Realtime.Player player)
        {
            _player = player;

            _usernameText.text = player.NickName;
            UpdateStats();
        }

        void UpdateStats()
        {
            object kills; 
            object deaths;

            var hasKills = _player.CustomProperties.TryGetValue("kills", out kills);
            var hasDeaths = _player.CustomProperties.TryGetValue("deaths", out deaths);
            _killDeathText.text = $"Kills {(hasKills ? kills : 0)} / Deaths {(hasDeaths ? deaths : 0)}";
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if(Equals(targetPlayer, _player))
            {
                if(changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
                {
                    UpdateStats();
                }
            }
        }
    }
}