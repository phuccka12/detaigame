using UnityEngine;

// T<T> là một "generic type", cho phép script này hoạt động với bất kỳ component nào (Player, GameManager,...)
public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        // Nếu chưa có instance nào tồn tại
        if (instance == null)
        {
            // Thì gán instance là component này
            instance = this as T;
            // Và không phá hủy nó khi chuyển scene
            DontDestroyOnLoad(this.gameObject);
        }
        // Nếu đã có instance khác tồn tại
        else
        {
            // Thì phá hủy đối tượng mới này đi để tránh trùng lặp
            Destroy(gameObject);
        }
    }
}