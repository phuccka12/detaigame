using System.Collections;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    // Lực gió và thời gian gió thổi
    public Vector2 windForce = new Vector2(10f, 0f); // Gió theo hướng X
    public float windDuration = 3f; // Thời gian gió thổi
    public float windPauseDuration = 2f; // Thời gian gió ngừng thổi
    private bool isWindActive = false; // Trạng thái gió

    // Coroutine kiểm soát gió
    private void Start()
    {
        StartCoroutine(WindCycle());
    }

    private IEnumerator WindCycle()
    {
        while (true) // Lặp lại vô hạn
        {
            // Thổi gió
            isWindActive = true;
            yield return new WaitForSeconds(windDuration); // Gió thổi trong khoảng thời gian

            // Dừng gió
            isWindActive = false;
            yield return new WaitForSeconds(windPauseDuration); // Gió ngừng trong khoảng thời gian
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng là nhân vật (ví dụ: nhân vật có tag "Player")
        if (other.CompareTag("Player"))
        {
            // Lấy Rigidbody2D của nhân vật
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null && isWindActive)
            {
                // Áp dụng lực gió vào nhân vật khi gió đang hoạt động
                rb.AddForce(windForce);
            }
        }
    }
}
