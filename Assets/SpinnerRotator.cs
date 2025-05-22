using UnityEngine;

public class SpinnerRotator : MonoBehaviour
{
    [SerializeField] private float speed = 180f;

    private void Update()
    {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}