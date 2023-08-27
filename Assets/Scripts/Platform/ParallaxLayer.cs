using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    /// <summary>
    /// Position = camera position * movementScale
    /// </summary>
    public Vector3 movementScale = Vector3.one;

    Transform _camera;

    void Awake()
    {
        _camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Scale(_camera.position, movementScale);
    }

}