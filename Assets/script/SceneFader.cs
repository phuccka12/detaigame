using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    // Tạo một ô trống trong Inspector để kéo Animator của đối tượng Fade vào
    public Animator fadeAnimator;

    // Hàm này sẽ được gọi bởi nút bấm
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        // Ra lệnh cho Animator chạy animation tên là "FadeToBlack"
        fadeAnimator.Play("FadeToBlack");

        // Chờ 1 giây để animation chạy xong
        yield return new WaitForSeconds(1);

        // Sau khi chờ, tải màn chơi mới
        SceneManager.LoadScene(sceneName);
    }
}