using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{

    public float value;

    private Transform cameraTr;

    private Vector3 camOriginPos;
    private Vector3 originPos;

    private void Start()
    {
        cameraTr = Camera.main.transform;
        camOriginPos = cameraTr.position;
        originPos = transform.position;
    }

    public void LateUpdate()
    {
        Vector3 position = originPos;
        position.x = value * (cameraTr.position.x - camOriginPos.x);

        transform.position = position;
    }
}
