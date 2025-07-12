using UnityEngine;

public class Item : MonoBehaviour
{
    // Tạo một menu để chọn loại vật phẩm trong Inspector
    public enum ItemType
    {
        Heart,
        Shield
    }

    public ItemType itemType;

    // Biến này để xử lý hiệu ứng nhấp nháy hoặc xoay tròn cho đẹp mắt
    public float rotationSpeed = 50f;
    public float floatSpeed = 0.5f;
    public float floatHeight = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Xoay tròn vật phẩm
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Làm vật phẩm trôi nổi lên xuống
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Kích hoạt khi có đối tượng đi vào collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có phải là người chơi không
        if (other.CompareTag("Player"))
        {
            // Lấy component script của người chơi
            JumpKingController player = other.GetComponent<JumpKingController>();

            // Xử lý tùy theo loại vật phẩm
            switch (itemType)
            {
                case ItemType.Heart:
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.Heal(1); // Hồi 1 máu
                    }
                    break;

                case ItemType.Shield:
                    if (player != null)
                    {
                        player.ActivateShield(); // Kích hoạt khiên
                    }
                    break;
            }

            // Sau khi nhặt, phá hủy vật phẩm
            Destroy(gameObject);
        }
    }
}