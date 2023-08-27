using System;
using Photon.Pun;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        protected Vector2 _direction;
        private int _originID;
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _spriteRenderer.color = _photonView.IsMine ? Color.green : Color.red;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // if other layer is Player
            if (other.CompareTag("Player"))
            {
                // if hit target is enemy and this bullet is mine player's
                if (!other.GetComponent<PhotonView>().IsMine && _photonView.IsMine)
                {
                    other.GetComponent<Health>().OnHit(_damage, _originID);
                    _photonView.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
                }
            }
            
            // if other layer is Ground
            if (IsGroundLayer(other.gameObject.layer, _groundLayer))
            {
                 _photonView.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
            }
        }

        private bool IsGroundLayer(int layer, LayerMask groundLayers)
        {
            var isGroundLayer = (groundLayers.value & (1 << layer)) != 0;
            return isGroundLayer;    
        }

        public void SetDirection(Vector2 direction)
        {
            _photonView.RPC(nameof(SetDirectionRPC), RpcTarget.All, direction);
        }
        
        [PunRPC]
        public void SetDirectionRPC(Vector2 direction)
        {
            // rotate this transform to direction
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,rotationZ);
            _direction = direction;
            _rigidbody2D.velocity = _direction * _speed;
        }
        
        [PunRPC]
        public void DestroyRPC()
        {
            Destroy(gameObject);
        }


        /// <summary>
        /// Set who is the origin of this bullet
        /// </summary>
        /// <param name="photonViewID"></param>
        public void SetOrigin(int photonViewID)
        {
            _originID = photonViewID;
        }
    }
}