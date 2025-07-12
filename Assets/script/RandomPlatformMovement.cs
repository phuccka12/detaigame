using UnityEngine;

public class RandomPlatformMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector2 moveRange = new Vector2(5f, 0f);

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        SetNewRandomTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomTarget();
        }
    }

    void SetNewRandomTarget()
    {
        float randomX = Random.Range(-moveRange.x, moveRange.x);
        float randomY = Random.Range(-moveRange.y, moveRange.y);
        targetPosition = startPosition + new Vector3(randomX, randomY, 0);
    }

    // Khi có đối tượng va chạm và đứng trên platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu đó là Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gán Player làm con của platform
            collision.transform.SetParent(transform);
        }
    }

    // Khi đối tượng rời khỏi platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Kiểm tra nếu đó là Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Hủy việc Player là con của platform
            collision.transform.SetParent(null);
        }
    }
}