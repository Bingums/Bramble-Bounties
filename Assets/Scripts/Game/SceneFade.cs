using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;
    
    public Animator screenFade;

    [SerializeField] private Image sceneFadeImage;
    
    void Awake()
    {
        /*
        FadeCanvas = GameObject.Find("FadeCanvas");
        FadeScreen = FadeCanvas.transform.GetChild(0).gameObject;
        screenFade = FadeScreen.GetComponent<Animator>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(FadeScreen);
        */
    }

    public void OnEnable()
    {
        /*
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            screenFade.Play("FadeIn");
        }
        */
    }

    /*
    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneRoutine());
    }
    

    IEnumerator ChangeSceneRoutine()
    {
        screenFade.SetBool("Start", true);
        yield return new WaitForSeconds(0.5f);
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    */

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 1);
        Color targetColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 0);
        
        yield return FadeCoroutine(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 0);
        Color targetColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 1);
        
        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }
    

    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            sceneFadeImage.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
        
    }
}
