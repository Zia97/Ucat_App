using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class QuantitativeReasoningQuestion
    {
        public String resource;
        public bool hasImage;
        public string imageLocation;
        public bool answerClicked = false;
        public int questionNumber;
        public string imageUri;
        public string questionText;
        public string answerReasoning;
        public string answer;
        public string option1;
        public string option2;
        public string option3;
        public string option4;
        public string option5;

        public String userAnswer = "";

        public bool flagged = false;


        public QuantitativeReasoningQuestion(String _resource, bool _hasImage, string _imageUri, string _imageLocation, int _questionNumber, string _questionText, string _answerReasoning, string _answer, string _option1, string _option2, string _option3, string _option4, string _option5)
        {
            resource = _resource;
            hasImage = _hasImage;
            imageUri = _imageUri;
            imageLocation = _imageLocation;
            questionNumber = _questionNumber;
            questionText = _questionText;
            answerReasoning = _answerReasoning;
            answer = _answer;
            option1 = _option1;
            option2 = _option2;
            option3 = _option3;
            option4 = _option4;
            option5 = _option5;         
        }

    }
}
