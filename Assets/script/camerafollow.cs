using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Đối tượng mà camera sẽ theo dõi
    public float smoothSpeed = 0.125f;  // Tốc độ mượt mà của camera
    public Vector3 offset;  // Khoảng cách giữa camera và đối tượng

    void LateUpdate()
    {
        // Kiểm tra nếu không có đối tượng để theo dõi
        if (target == null)
            return;

        // Tính toán vị trí mong muốn của camera (vị trí target cộng với offset)
        Vector3 desiredPosition = target.position + offset;
        
        // Dùng Lerp để làm cho camera di chuyển mượt mà
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Cập nhật vị trí của camera
        transform.position = smoothedPosition;

        // Nếu bạn muốn camera luôn quay về hướng đối tượng theo dõi, uncomment dòng dưới
        // transform.LookAt(target);
    }
}