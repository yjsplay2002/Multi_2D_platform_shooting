using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class NickName : MonoBehaviour
    {
        private PhotonView _photonView;
        [SerializeField] private TMP_Text _nickNameText;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _nickNameText.text = _photonView.IsMine ? PhotonNetwork.NickName : _photonView.Owner.NickName;
            _nickNameText.color = _photonView.IsMine ? Color.green : Color.red;
        }
    }
}