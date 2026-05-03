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
        Debug.Log(sceneFadeImage.color.a);
        gameObject.SetActive(true);
        sceneFadeImage.raycastTarget = false;

        Debug.Log("Point 2");
        if (sceneFadeImage != null)
        {
            sceneFadeImage.raycastTarget = true;
        }
        
        
        Debug.Log("Point 3");
        Color startColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 1);
        Color targetColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 0);
        
        
        Debug.Log ("Point 4");
        yield return FadeCoroutine(startColor, targetColor, duration);

        Debug.Log ("Point 5");
        sceneFadeImage.color = targetColor;
        sceneFadeImage.raycastTarget = false;
        Debug.Log("Point 6");
        gameObject.SetActive(false);
        Debug.Log("Point 7");
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        gameObject.SetActive(true);

        if (sceneFadeImage != null)
        {
            sceneFadeImage.raycastTarget = true;
        }

        Color startColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 0f);
        Color targetColor = new Color(sceneFadeImage.color.r, sceneFadeImage.color.g, sceneFadeImage.color.b, 1f);

        yield return FadeCoroutine(startColor, targetColor, duration);

        sceneFadeImage.color = targetColor;
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
