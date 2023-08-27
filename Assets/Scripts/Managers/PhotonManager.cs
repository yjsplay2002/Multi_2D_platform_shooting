using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public TMP_InputField _nickNameInput;
        public GameObject _readyToConnectPanel;
        public GameObject _respawnPanel;

        public static PhotonManager Instance { get; set; }

        void Awake()
        {
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;
            Instance = this;
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        public void Connect() => PhotonNetwork.ConnectUsingSettings();

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.LocalPlayer.NickName = _nickNameInput.text;
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
        }

        public override void OnJoinedRoom()
        {
            _readyToConnectPanel.SetActive(false);
            StartCoroutine(InitGameScene());
            SpawnPlayer();
        }

        IEnumerator InitGameScene()
        {
            yield return new WaitForSeconds(0.2f);
            // foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Bullet")) GO.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
        }

        public void SpawnPlayer()
        {
            PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-10f, 10f), 10, 0), Quaternion.identity);
            _respawnPanel.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _readyToConnectPanel.SetActive(true);
            _respawnPanel.SetActive(false);
        }

        public void OnMyPlayerDead()
        {
            _readyToConnectPanel.SetActive(false);
            _respawnPanel.SetActive(true);
        }
    }
}