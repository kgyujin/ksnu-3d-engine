using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // 게임 시작 버튼 클릭 시 호출
    public void OnStartGame()
    {
        // GameScene이라는 이름의 씬을 로드
        SceneManager.LoadScene("Level Design_Respawn");
    }

    // 게임 종료 버튼 클릭 시 호출
    public void OnQuitGame()
    {
        // 에디터에서 작동 확인용
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
