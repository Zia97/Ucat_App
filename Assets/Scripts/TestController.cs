using System;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{

    //Info Panels
    public GameObject testInfoPanel;
    public GameObject VerbalReasoningInfoPanel;
    public GameObject DecisionMakingInfoPanel;
    public GameObject QuantitativeReasoningInfoPanel;
    public GameObject AbstractReasoningInfoPanel;
    public GameObject SituationalJudgementInfoPanel;


    //Section Canvases
    public GameObject VerbalReasoningCanvas;
    public GameObject DecisionMakingCanvas;
    public GameObject QuantitativeReasoningCanvas;
    public GameObject AbstractReasoningCanvas;
    public GameObject SituationalJudgementCanvas;

    //Buttons
    public Button testInfoNextButton;
    public Button VerbalReasoningStartButton;
    public Button DecisionMakingStartButton;
    public Button QuantitativeReasoningStartButton;
    public Button AbstractReasoningStartButton;
    public Button SituationalJudgementStartButton;

    //Start is called before the first frame update - will need a new one when resuming tests
    void Start()
    {
        loadPanels();
        addButtonListeners();
        loadTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Loads the test using the json
    void loadTest()
    {
       
    }

    void loadPanels()
    {
        //Will need a check to see if test has been started before
        VerbalReasoningInfoPanel.SetActive(false);
        DecisionMakingInfoPanel.SetActive(false);
        QuantitativeReasoningInfoPanel.SetActive(false);
        AbstractReasoningInfoPanel.SetActive(false);
        SituationalJudgementInfoPanel.SetActive(false);

        VerbalReasoningCanvas.SetActive(false);
        DecisionMakingCanvas.SetActive(false);
        QuantitativeReasoningCanvas.SetActive(false);
        AbstractReasoningCanvas.SetActive(false);
        SituationalJudgementCanvas.SetActive(false);

    }

    #region Button Clicks
    void addButtonListeners()
    {
        testInfoNextButton.onClick.AddListener(testInfoNextButtonClicked);
        VerbalReasoningStartButton.onClick.AddListener(VerbalReasoningStartButtonClicked);
        DecisionMakingStartButton.onClick.AddListener(DecisionMakingStartButtonClicked);
        QuantitativeReasoningStartButton.onClick.AddListener(QuantitativeReasoningStartButtonClicked);
        AbstractReasoningStartButton.onClick.AddListener(AbstractReasoningStartButtonClicked);
        SituationalJudgementStartButton.onClick.AddListener(SituationalJudgementStartButtonClicked);
    }

    private void SituationalJudgementStartButtonClicked()
    {
        SituationalJudgementInfoPanel.SetActive(false);
        SituationalJudgementCanvas.SetActive(true);
    }

    private void AbstractReasoningStartButtonClicked()
    {
        AbstractReasoningInfoPanel.SetActive(false);
        AbstractReasoningCanvas.SetActive(true);
    }

    private void QuantitativeReasoningStartButtonClicked()
    {
        QuantitativeReasoningInfoPanel.SetActive(false);
        QuantitativeReasoningCanvas.SetActive(true);
    }

    private void DecisionMakingStartButtonClicked()
    {
        DecisionMakingInfoPanel.SetActive(false);
        DecisionMakingCanvas.SetActive(true);
    }

    private void VerbalReasoningStartButtonClicked()
    {
        VerbalReasoningInfoPanel.SetActive(false);
        VerbalReasoningCanvas.SetActive(true);
    }

    private void testInfoNextButtonClicked()
    {
        testInfoPanel.SetActive(false);
        VerbalReasoningInfoPanel.SetActive(true);
    }



    #endregion

}
