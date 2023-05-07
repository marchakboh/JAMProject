using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;

    public int sceneNumber;
    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            FadeToNextLevel();
        }
    }

    public void FadeToNextLevel()
    {
        FadeToLevel();
    }
    public void AnimationFade()
    {
        animator.SetTrigger("FadeOut");
    }

    
    public void FadeToLevel()
    {
        AnimationFade();
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        AnimationFade();
        Application.Quit();
    }
}
