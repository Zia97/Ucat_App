using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickedScript : MonoBehaviour
{

    public GameObject VRReviewPanel;
    public GameObject ReviewFooterPanel;

    public GameObject BaseHeaderPanel;
    public GameObject BaseQuestionSelectorPanel;
    public GameObject BaseQuestionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void questionButtonClicked(int questionNumber)
    {
        Debug.Log(questionNumber);
        VRReviewPanel.SetActive(false);
        ReviewFooterPanel.SetActive(false);
        BaseHeaderPanel.SetActive(true);
        BaseQuestionSelectorPanel.SetActive(true);
        BaseQuestionsPanel.SetActive(true);
        VerbalReasoningControllerScriptTest.Instance.loadSetFromReview(questionNumber);
    }
}
