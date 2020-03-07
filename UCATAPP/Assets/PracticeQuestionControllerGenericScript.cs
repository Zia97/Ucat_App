using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeQuestionControllerGenericScript : MonoBehaviour
{

    public Text HeaderPanelText;
    // Start is called before the first frame update
    void Start()
    {
        HeaderPanelText.text = GlobalVariables.SelectedPracticeQuestion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
