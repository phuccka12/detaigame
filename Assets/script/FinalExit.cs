using UnityEngine;
using System.Collections; // Cần thư viện này để dùng Coroutine

public class FinalExit : MonoBehaviour
{
    [Header("Hiệu ứng hút")]
    public float rotationSpeed = 90f;
    public float pullDuration = 1.5f;

    private bool isPlayerBeingPulled = false;

    void Update()
    {
        // Làm cho cổng xoay tròn
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi người chơi chạm vào và chưa bị hút
        if (other.CompareTag("Player") && !isPlayerBeingPulled)
        {
            // Bắt đầu chuỗi hiệu ứng
            StartCoroutine(PullPlayerAndShowVictory(other.gameObject));
        }
    }

    private IEnumerator PullPlayerAndShowVictory(GameObject player)
    {
        isPlayerBeingPulled = true;

        // Tạm thời tắt điều khiển của người chơi để họ không di chuyển được
        JumpKingController playerController = player.GetComponent<JumpKingController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Bắt đầu hiệu ứng hút và thu nhỏ
        float elapsedTime = 0f;
        Vector3 startPosition = player.transform.position;
        Vector3 startScale = player.transform.localScale;

        while (elapsedTime < pullDuration)
        {
            // Di chuyển người chơi về tâm của cổng
            player.transform.position = Vector3.Lerp(startPosition, transform.position, elapsedTime / pullDuration);
            // Thu nhỏ người chơi
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsedTime / pullDuration);

            elapsedTime += Time.deltaTime;
            yield return null; // Đợi đến frame tiếp theo
        }

        // Sau khi hiệu ứng kết thúc, ra lệnh cho GameManager xử lý chiến thắng
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerWon();
        }
    }
}