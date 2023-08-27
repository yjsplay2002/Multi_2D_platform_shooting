using System;
using System.Collections;
using Managers;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Player
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private SpriteRenderer _hpSpriteRenderer;
        [SerializeField] private GameObject _hitEffect;

        private float _currentHealth;
        private Animator _animator;
        private PhotonView _photonView;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Dead = Animator.StringToHash("dead");
        private int _deathCount = 0;
        private int _killCount = 0;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _photonView = GetComponent<PhotonView>();
            Init();
        }

        public void Init()
        {
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            _hpSpriteRenderer.size = new Vector2(Mathf.Clamp(_currentHealth / _maxHealth, 0, 1), 1f);
        }

        public void OnHit(float damage, int originID)
        {
            _photonView.RPC(nameof(OnHitRPC), RpcTarget.AllBuffered, damage, originID);
        }

        [PunRPC]
        private void OnHitRPC(float damage, int originID)
        {
            if (_currentHealth <= 0) return;

            
            _currentHealth -= damage;
            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, transform.position, Quaternion.identity);
            }
        
            if (_currentHealth <= 0)
            {
                Die();

                print($"originID: {originID}, localPlayerID: {PhotonNetwork.LocalPlayer.ActorNumber}");
                
                if (originID == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    AddKillCount();
                }
            }
            else
            {
                _animator.SetTrigger(Hurt);
            }
        }

        private void AddDeathCount()
        {
            Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
            var deathCountExist = ht.TryGetValue("deaths", out var deaths);
            if (deathCountExist)
            {
                _deathCount = (int) deaths;
                ht["deaths"] = _deathCount + 1;
            }
            else
            {
                ht.Add("deaths", _deathCount + 1);    
            }
                    
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
        }

        private void AddKillCount()
        {
            Hashtable killsHt = PhotonNetwork.LocalPlayer.CustomProperties;
            var killCountExist = killsHt.TryGetValue("kills", out var kills);
            if (killCountExist)
            {
                _killCount = (int) kills;
                killsHt["kills"] = _killCount + 1;
            }
            else
            {
                killsHt.Add("kills", _killCount + 1);
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(killsHt);
        }

        public void Die()
        {
            // if this player is mine, increase death count
            if (_photonView.IsMine)
            {
                AddDeathCount();
            }
            StartCoroutine(DieCo());
        }

        IEnumerator DieCo()
        {
            _animator.SetBool(Dead, true);
            yield return new WaitForSeconds(2f);
            _photonView.RPC(nameof(DestroyMeRPC), RpcTarget.AllBuffered);
        }
        
        [PunRPC]
        private void DestroyMeRPC()
        {
            if (_photonView.IsMine)
            {
                PhotonManager.Instance.OnMyPlayerDead();
            }
            Destroy(gameObject);
        }
    }
}