using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _range = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf(_range));
    }

    // Update is called once per frame
    void Update()
    {
        // Take aim
        Vector2 direction = transform.up;
        Vector2 position = transform.position;
        direction = direction.normalized * _speed * Time.deltaTime;

        // Fire
        transform.position = position + direction;
    }

    IEnumerator DestroySelf(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(gameObject);
    }
}
