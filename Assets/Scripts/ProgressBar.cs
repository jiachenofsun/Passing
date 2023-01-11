using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ProgressBar : MonoBehaviour
{

    #region Progress variables
    [SerializeField]
    private static int tasksCompleted = 0;
    [SerializeField]
    private int maxGoodDeeds = 6;
    private bool[] goodDeedsTracker;
    #endregion

    #region RenderingVariables
    [SerializeField]
    private Sprite empty;
    [SerializeField]
    private Sprite filled;
    /** A UI element which houses all the itemSlotTemplates. */
    private Transform heartContainer;
    /** A UI element representing a template for the hearts. */
    private Transform heartTemplate;

    private TextMeshProUGUI progressText;
    private AudioManager audioManager;

    public static bool isActive;

    public int endscene;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        goodDeedsTracker = new bool[maxGoodDeeds];
        heartContainer = transform.Find("heartContainer");
        progressText = transform.Find("Announcement Text").GetComponent<TextMeshProUGUI>();
        heartTemplate = heartContainer.Find("heartTemplate");
        audioManager = FindObjectOfType<AudioManager>();
        UpdateUI();
    }

    // Updates the UI to show the current 
    public void UpdateUI()
    {
        // deletes all existing itemSlotTemplates
        foreach (Transform child in heartContainer)
        {
            if (child == heartTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        float itemSlotCellSize = 135f;
        for (int i = 0; i < maxGoodDeeds; i++)
        {
            RectTransform itemSlotRectTransform = Instantiate(heartTemplate, heartContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, 0);
            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();

            if (goodDeedsTracker[i])
                image.sprite = filled;
            else
                image.sprite = empty;
            x++;
        }
    }

    private IEnumerator TextTransition()
    {
        isActive = true;
        if (tasksCompleted == maxGoodDeeds)
        {
            progressText.text = "You are ready for the Passing!";
        }
        float duration = 1.5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            progressText.color = new Color(progressText.color.r, progressText.color.g, progressText.color.b, newAlpha);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            progressText.color = new Color(progressText.color.r, progressText.color.g, progressText.color.b, newAlpha);
            yield return null;
        }
        audioManager.Play("GainHeart");
        UpdateUI();
        isActive = false;
        if (tasksCompleted == maxGoodDeeds)
        {
            StartCoroutine(EndGame());
        }
    }

    /** Called after a storyline task/good deed is completed. */
    public void newTaskCompleted()
    {
        if (tasksCompleted >= maxGoodDeeds)
        {
            Debug.LogWarning("Cannot increment tasksCompleted anymore");
        } else
        {
            goodDeedsTracker[tasksCompleted] = true;
            tasksCompleted += 1;
        }
        StartCoroutine(TextTransition());
    }

    /** Called after enough hearts have been reached. Triggers end sequence. */
    public IEnumerator EndGame()
    {
        Animator animator = FindObjectOfType<LevelChanger>().animator;
        animator.SetTrigger("FadeOutNoSwitch");
        yield return new WaitForSeconds(1.5f);
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("DoNotDestroy"))
        {
            Destroy(o);
        }
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        SceneManager.LoadScene(endscene);
    }
}
