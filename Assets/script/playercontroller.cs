using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Tốc độ di chuyển của nhân vật
    public float moveSpeed = 5f;
    // Lực nhảy của nhân vật
    public float jumpForce = 10f;

    // Tham chiếu đến Rigidbody của nhân vật
    private Rigidbody2D rb; // Đối với game 2D

    // Tham chiếu đến Animator của nhân vật
    private Animator animator;

    // Biến kiểm tra xem nhân vật có đang trên mặt đất không
    private bool isGrounded;
    // Biến này để điều khiển hướng của nhân vật (true = phải, false = trái)
    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Đảm bảo ban đầu isGrounded là false, sẽ được cập nhật khi va chạm
        isGrounded = false;
    }

    // Update is called once per frame (được gọi mỗi frame)
    void Update()
    {
        // -------------------- DI CHUYỂN NGANG --------------------
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Cập nhật tham số "IsRunning" trong Animator
        animator.SetBool("isRunning", Mathf.Abs(moveInput) > 0.01f);

        // -------------------- LẬT TRÁI PHẢI --------------------
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // -------------------- NHẢY --------------------
        // Nếu nhấn nút "Jump" (mặc định là Spacebar) và nhân vật đang trên mặt đất
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true); // Đặt IsJumping thành true khi bắt đầu nhảy
            isGrounded = false; // Ngay khi nhảy, đặt isGrounded thành false
        }
    }

    // Hàm này được gọi khi Collider của nhân vật bắt đầu va chạm với một Collider khác
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có Tag là "Ground" không
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Kiểm tra thêm để đảm bảo va chạm là từ phía dưới (tiếp đất)
            // Lấy điểm tiếp xúc đầu tiên của va chạm
            // float dotProduct = Vector2.Dot(collision.contacts[0].normal, Vector2.up);
            // Debug.Log("Normal dot product: " + dotProduct); // Để debug
            // Nếu điểm tiếp xúc có vector normal hướng lên (tức là nhân vật đang đứng trên nó)
            // Giá trị 0.7f là một ngưỡng, điều chỉnh nếu cần.
            if (collision.contacts.Length > 0 && Vector2.Dot(collision.contacts[0].normal, Vector2.up) > 0.7f)
            {
                isGrounded = true; // Đặt isGrounded thành true khi chạm đất
                animator.SetBool("isJumping", false); // Đặt IsJumping về false khi chạm đất
            }
        }
    }

    // Hàm này được gọi khi Collider của nhân vật ngừng va chạm với một Collider khác
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Nếu đối tượng ngừng va chạm là "Ground" và không còn chạm đất với bất kỳ thứ gì khác
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Set isGrounded thành false. (Có thể cần logic phức tạp hơn nếu có nhiều nền tảng)
            // Để chắc chắn hơn, bạn có thể dùng một Raycast nhỏ ở đây để kiểm tra lại.
            // Tuy nhiên, với logic IsJumping=false ở OnCollisionEnter2D, điều này thường đủ.
            // isGrounded = false; // Có thể bỏ dòng này nếu bạn muốn kiểm soát chặt chẽ hơn bằng OnCollisionStay2D
        }
    }

    // (Tùy chọn) Hàm này được gọi liên tục khi Collider của nhân vật đang va chạm với một Collider khác
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Kiểm tra tương tự như OnCollisionEnter2D để đảm bảo vẫn đang đứng trên mặt đất
            if (collision.contacts.Length > 0 && Vector2.Dot(collision.contacts[0].normal, Vector2.up) > 0.7f)
            {
                isGrounded = true;
                // Có thể không cần animator.SetBool("IsJumping", false); ở đây nữa vì đã làm trong Enter
            }
            else
            {
                // Trường hợp va chạm nhưng không phải là mặt đất để đứng lên (ví dụ: chạm vào tường)
                // isGrounded = false; // Cẩn thận với việc đặt false ở đây, có thể gây lỗi nhảy nhiều lần
            }
        }
    }

    // Hàm lật nhân vật (thay đổi hướng của sprite)
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1; // Đảo ngược giá trị x của scale
        transform.localScale = currentScale;
    }
}