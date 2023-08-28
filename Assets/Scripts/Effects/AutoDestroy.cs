using UnityEngine;

namespace Effects
{
    public class AutoDestroy : MonoBehaviour
    {
        public float _destroyTime = 2f;
        
        private void Start()
        {
            Destroy(gameObject, _destroyTime);
        }
    }
}