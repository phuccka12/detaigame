using UnityEngine;

public class camerfollow : MonoBehaviour
{
    private Transform playerTransform;
    public Vector3 offset = new Vector3(0, 2, -10); // Chỉnh offset cho phù hợp
    public float smoothSpeed = 0.125f;

    // Start được gọi khi camera này được tạo ra ở màn chơi mới
    void Start()
    {
        // Cố gắng tìm instance của Player ngay lập tức
        if (JumpKingController.instance != null)
        {
            playerTransform = JumpKingController.instance.transform;
        }
    }

    void LateUpdate()
    {
        // Nếu vì lý do nào đó chưa tìm thấy player, hãy thử tìm lại
        if (playerTransform == null && JumpKingController.instance != null)
        {
            playerTransform = JumpKingController.instance.transform;
        }

        // Nếu đã tìm thấy player thì đi theo
        if (playerTransform != null)
        {
            Vector3 desiredPosition = playerTransform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}