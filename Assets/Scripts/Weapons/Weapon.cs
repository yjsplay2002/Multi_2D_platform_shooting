using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    [CreateAssetMenu(menuName = "My Assets/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Sprite _sprite;
        public Vector3 _anchorRotation;
        public string _bulletPrefabName;
        public float _damage;
        public float _fireInterval;
    }
}