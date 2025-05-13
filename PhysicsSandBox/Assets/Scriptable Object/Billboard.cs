using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera _cam;
    void Start() {
        _cam = Camera.main;
    }
    void LateUpdate() {
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }
}
