using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** The script controlling the Baking Minigame. */
public class UI_Foodgame : MonoBehaviour
{
    private List<Ingredient> currFoods;
    /** A UI element which houses all the itemSlotTemplates. */
    private Transform ingredientContainer;
    /** A UI element representing a template for items on the UI. */
    private Transform ingredientTemplate;
    [SerializeField]
    private int maxCapacity = 5;

    /** Variables to control the cooking meter. */
    private float cookingBar = 0;

    /** Controls how fast or slow the Cook Timing oscillates. */
    [SerializeField]
    private float maxCooking = 2f;

    /** Controls the allowance of the Cook Timing. */
    [SerializeField]
    private float maxAllowance = 0.1f;

    /** Represents which direction the Cook Timing oscillates. */
    private int sense = 1;

    /** Refernence to the Cook Timing slider. */
    [SerializeField]
    private Slider cookingSlider;

    /** True if the player times the Cook Timing properly. */
    private bool timingSuccess;

    /** True if the player is successful in making the cookie. */
    private bool cookingSuccess;

    /** Reference to the correct recipe. */
    private string[] correctRecipe;

    /* References to the Bottom Buttons and the Cook Timing function. */
    private Transform bottomBtns;
    private Transform cookTiming;

    /** Reference to the Baking Box/Oven that triggers the minigame. */
    [SerializeField]
    private GameObject bakingBox;

    private void Awake()
    {
        ingredientContainer = transform.Find("ingredientContainer");
        ingredientTemplate = ingredientContainer.Find("ingredientTemplate");
        currFoods = new List<Ingredient>();
        correctRecipe = new string[4] {"C", "B", "D", "A"};
        bottomBtns = transform.Find("BottomBtns");
        cookTiming = transform.Find("CookTiming");
    }

    private void Update()
    {
        cookingBar = Mathf.Min(maxCooking, cookingBar);
        if (cookingBar == maxCooking)
            sense = -1;
        else if (cookingBar <= 0)
            sense = 1;
        cookingBar += (Time.deltaTime * sense);
        cookingSlider.value = cookingBar / maxCooking;
    }

    #region Ingredient functions

    /** Displays the ingredient in the pot on the UI. */
    private void RefreshIngredients()
    {
        // deletes all existing itemSlotTemplates
        foreach (Transform child in ingredientContainer)
        {
            if (child == ingredientTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 50f;
        foreach (Ingredient ingredient in currFoods)
        {
            RectTransform itemSlotRectTransform = Instantiate(ingredientTemplate, ingredientContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y);
            Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
            image.sprite = ingredient.GetSprite();

            x++;
        }
    }

    public void ClearIngredients()
    {
        currFoods = new List<Ingredient>();
        RefreshIngredients();
    }

    public void AddIngredient(string type)
    {
        if (currFoods.Count == maxCapacity)
        {
            StartCoroutine(DisplayMessage("Pot is full!"));
            return;
        }
        currFoods.Add(new Ingredient(type));
        RefreshIngredients();
    }
    #endregion

    private IEnumerator DisplayMessage(string s)
    {
        Transform message = transform.Find("Message");
        TextMeshProUGUI messageTMP = message.GetComponent<TextMeshProUGUI>();
        messageTMP.text = s;
        //Waits for 3 seconds before erasing the message
        yield return new WaitForSeconds(3);

        messageTMP.text = "";
    }

    public void End()
    {
        bakingBox.GetComponent<BakingBox>().EndMinigame();
    }

    #region Cook functions
    public void StartCook()
    {
        bottomBtns.gameObject.SetActive(false);
        cookTiming.gameObject.SetActive(true);
        StartCoroutine(DisplayMessage("Press the Cook Button to cook!"));
    }

    private void EndCook()
    {
        bottomBtns.gameObject.SetActive(true);
        cookTiming.gameObject.SetActive(false);
    }

    public void CheckCookTiming()
    {
        timingSuccess = false;
        float test = cookingBar / maxCooking;
        if (Mathf.Abs(test - 0.5f) <= maxAllowance)
            timingSuccess = true;
        Debug.Log("Timing success: " + timingSuccess);
        StartCoroutine(CookingProcess());
    }

    public IEnumerator CookingProcess()
    {
        Animator animator = FindObjectOfType<LevelChanger>().animator;
        animator.SetTrigger("FadeOutNoSwitch");
        yield return new WaitForSeconds(1);
        CheckCookingSuccess();
        ClearIngredients();
        EndCook();
        animator.SetTrigger("FadeIn");

        if (cookingSuccess)
        {
            bottomBtns.gameObject.SetActive(false);
            StartCoroutine(DisplayMessage("Success! You just made the best cookie ever!"));
            yield return new WaitForSeconds(3);
            End();
            bakingBox.GetComponent<BakingBox>().GiveCookie();
        } else
        {
            StartCoroutine(DisplayMessage("Oh no! Something went wrong..."));
        }
    }

    private void CheckCookingSuccess()
    {
        if (timingSuccess == false || currFoods.Count != correctRecipe.Length)
        {
            cookingSuccess = false;
            return;
        }
        for (int i = 0; i < currFoods.Count; i++)
        {
            if (!currFoods[i]._type.Equals(correctRecipe[i]))
            {
                cookingSuccess = false;
                Debug.Log("Wrong ingredient at index " + i);
                return;
            }
        }
        cookingSuccess = true;
        Debug.Log("Cooking success: " + cookingSuccess);
    }
    #endregion


    [System.Serializable]
    public class Ingredient
    {
        public Ingredient(string type)
        {
            _type = type;
        }

        /** My Ingredient type. */
        public string _type;


        /** Gets the sprite of this ingredient. */
        public Sprite GetSprite()
        {
            switch (_type)
            {
                default:
                case "A": return ItemAssets.Instance.ingrASprite;
                case "B": return ItemAssets.Instance.ingrBSprite;
                case "C": return ItemAssets.Instance.ingrCSprite;
                case "D": return ItemAssets.Instance.ingrDSprite;
                case "E": return ItemAssets.Instance.ingrESprite;
            }
        }
    }

}
