using System;
using UnityEngine;

namespace Player
{
    public class CooltimeGuage : MonoBehaviour
    {
        private PlayerController _playerController;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_playerController.WeaponCooltime > 0)
            {
                _spriteRenderer.enabled = true;
                _spriteRenderer.size = new Vector2(_playerController.WeaponCooltime / _playerController.CurrentWeapon._fireInterval, 1);
            }
            else
            {
                _spriteRenderer.enabled = false;
            }
        }
    }
}