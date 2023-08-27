using System;
using Bullets;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Weapons;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _jumpPower = 300f;
        [SerializeField] private LayerMask _deadZoneLayer;
        [SerializeField] private Weapon[] _weapons;
        [SerializeField] private Transform _handTransform;
        [SerializeField] private Transform _weaponAnchor;
        
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private PhotonView _photonView;
        private Health _health;
        
        bool isGround;
        Vector3 curPos;
        private static readonly int VelocityX = Animator.StringToHash("velocityX");
        private static readonly int VelocityY = Animator.StringToHash("velocityY");
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Fire = Animator.StringToHash("fire");
        private Weapon _currentWeapon;
        private int _currentWeaponIndex = 0;
        
        public float WeaponCooltime { get; private set; }
        public Weapon CurrentWeapon => _currentWeapon;

        void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _photonView = GetComponent<PhotonView>();
            _health = GetComponent<Health>();

            if (_photonView.IsMine)
            {
                var vCam = GameObject.Find("Virtual Cam").GetComponent<CinemachineVirtualCamera>();
                vCam.Follow = transform;
                vCam.LookAt = transform;
                _photonView.RPC(nameof(SetWeaponRPC), RpcTarget.AllBuffered, 0);
            }
        }


        /// <summary>
        /// For control player
        /// </summary>
        void Update()
        {
            if (_photonView.IsMine)
            {
                // Movement
                float horizontalAxis = Input.GetAxisRaw("Horizontal");
                _rigidbody2D.velocity = new Vector2(_speed * horizontalAxis, _rigidbody2D.velocity.y);

                _animator.SetFloat(VelocityX, Mathf.Abs(horizontalAxis));
                _animator.SetFloat(VelocityY, _rigidbody2D.velocity.y);
                
                _photonView.RPC("SetFlipRPC", RpcTarget.All, horizontalAxis);
                
                // Jump and ground check
                isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, _groundLayer);
                _animator.SetBool(Grounded, isGround);

                if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
                {
                    _photonView.RPC(nameof(JumpRPC), RpcTarget.All);
                }
                
                // Fire bullet
                if (Input.GetKeyDown(KeyCode.Space) && WeaponCooltime <= 0)
                {
                    _animator.SetTrigger(Fire);
                    
                    var bulletObj = PhotonNetwork.Instantiate(_currentWeapon._bulletPrefabName, transform.position, Quaternion.identity);
                    var bullet = bulletObj.GetComponent<Bullet>();
                    bullet.SetDirection(_spriteRenderer.flipX ? Vector2.left : Vector2.right);
                    bullet.SetOrigin(PhotonNetwork.LocalPlayer.ActorNumber);
                    bullet.SetDamage(_currentWeapon._damage);
                    
                    WeaponCooltime = _currentWeapon._fireInterval;
                }
                
                // Change weapon
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    _photonView.RPC(nameof(SetWeaponRPC), RpcTarget.AllBuffered, (_currentWeaponIndex + 1) % _weapons.Length);
                }
                
            }
            
            WeaponCooltime -= Time.deltaTime;
        }

        [PunRPC]
        private void SetWeaponRPC(int index)
        {
            _currentWeapon = _weapons[index];
            _currentWeaponIndex = index;
            WeaponCooltime = _currentWeapon._fireInterval;
            var weaponRenderer = _weaponAnchor.GetComponent<SpriteRenderer>();
            weaponRenderer.sprite = _currentWeapon._sprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsDeadZoneLayer(other.gameObject.layer, _deadZoneLayer))
            {
                _health.Die();
            }
        }
        
        private bool IsDeadZoneLayer(int layer, LayerMask deadZoneLayers)
        {
            var isDeadZoneLayer = (deadZoneLayers.value & (1 << layer)) != 0;
            return isDeadZoneLayer;    
        }

        [PunRPC]
        private void SetFlipRPC(float axis)
        {
            if (axis != 0)
            {
                _spriteRenderer.flipX = axis < 0;
                if (_spriteRenderer.flipX)
                {
                    var position = _handTransform.localPosition;
                    position = new Vector3(Mathf.Abs(position.x) * -1, position.y, position.z) ;
                    _handTransform.localPosition = position;
                    _handTransform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    var position = _handTransform.localPosition;
                    position = new Vector3(Mathf.Abs(position.x), position.y, position.z) ;
                    _handTransform.localPosition = position;
                    _handTransform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        [PunRPC]
        private void JumpRPC()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);
        }
    }
}