using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxSpeed = 0.2f;
    private Transform cam;
    private Vector3 startPos;
    private float startCamX;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
        startCamX = cam.position.x;
    }

    void LateUpdate()
    {
        float travel = cam.position.x - startCamX;
        transform.position = new Vector3(startPos.x + (travel * parallaxSpeed),
                                         transform.position.y,
                                         transform.position.z);
    }
}