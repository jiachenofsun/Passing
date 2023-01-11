using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Help : MonoBehaviour
{
    [SerializeField]
    private GameObject UIHelpImage;
    [SerializeField]
    private GameObject UIHelpText;

    private TextMeshProUGUI helpTextTMP;
    private Animator animator;
    

    // Start is called before the first frame update
    private void Awake()
    {
        helpTextTMP = UIHelpText.GetComponent<TextMeshProUGUI>();
        animator = UIHelpImage.GetComponent<Animator>();
    }
    public void disableUIHelp()
    {
        animator.SetTrigger("Blank");
        helpTextTMP.text = "";
    }

    public void setUIHelp(string imagetext, string texttext)
    {
        setUIHelpImage(imagetext);
        setUIHelpText(texttext);
    }

    public void setUIHelpImage(string text)
    {
        animator.SetTrigger(text);
    }

    public void setUIHelpText(string text)
    {
        helpTextTMP.text = text;
    }
  
}
