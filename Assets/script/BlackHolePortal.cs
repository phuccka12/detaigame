using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackHolePortal : MonoBehaviour
{
    [Header("Cài đặt cổng")]
    public string sceneToLoad;
    public float rotationSpeed = 90f;

    [Header("Hiệu ứng hút")]
    public float pullDuration = 1.5f;

    private bool isPlayerBeingPulled = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerBeingPulled)
        {
            StartCoroutine(PullPlayerAndLoadScene(other.gameObject));
        }
    }

    private IEnumerator PullPlayerAndLoadScene(GameObject player)
    {
        isPlayerBeingPulled = true;

        JumpKingController playerController = player.GetComponent<JumpKingController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        float elapsedTime = 0f;
        Vector3 startPosition = player.transform.position;
        Vector3 startScale = player.transform.localScale;

        while (elapsedTime < pullDuration)
        {
            player.transform.position = Vector3.Lerp(startPosition, transform.position, elapsedTime / pullDuration);
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsedTime / pullDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }
        player.transform.localScale = startScale;

        SceneManager.LoadScene(sceneToLoad);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToLoad)
        {
            MovePlayerToSpawnPoint();
        }
    }

    void MovePlayerToSpawnPoint()
    {
        GameObject spawnPoint = GameObject.Find("SpawnPoint");
        if (spawnPoint != null)
        {
            if (JumpKingController.instance != null)
            {
                JumpKingController player = JumpKingController.instance;
                player.transform.position = spawnPoint.transform.position;

                // === DÒNG LỆNH QUAN TRỌNG NHẤT ĐƯỢC THÊM VÀO ===
                // Cập nhật lại vị trí hồi sinh cho nhân vật để khi chết sẽ hồi sinh ở map hiện tại
                player.SetNewSpawnPosition(spawnPoint.transform.position);

                Debug.Log("Nhân vật đã được dịch chuyển và cập nhật spawn point!");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy 'SpawnPoint' trong màn chơi: " + sceneToLoad);
        }
    }
}