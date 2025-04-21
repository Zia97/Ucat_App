using System;

namespace Assets.Scripts
{
    class DecisionMakingQuestion
    {
        public string Resource { get; private set; }
        public bool HasImage { get; private set; }
        public bool AnswerClicked { get; set; } = false;
        public string ImageURI { get; private set; }

        public string ImageLocation { get; private set; }

        public int QuestionNumber { get; private set; }
        public string QuestionText { get; private set; }
        public string QuestionAnswer { get; private set; }
        public string AnswerReasoning { get; private set; }
        public string Option1 { get; private set; }
        public string Option2 { get; private set; }
        public string Option3 { get; private set; }
        public string Option4 { get; private set; }

        // QuestionNumber:URI:questionAnswer:usersAnswer
        public string UserAnswer { get; set; } = "";

        public bool QuestionAnswered { get; set; } = false;

        public bool Flagged { get; set; } = false;

        public DecisionMakingQuestion(string resource, bool hasImage, string imageUri, string imageLocation, int questionNumber, string questionText, string answer, string answerReasoning, string option1, string option2, string option3, string option4)
        {
            Resource = resource;
            HasImage = hasImage;
            ImageURI = imageUri;
            ImageLocation = imageLocation;
            QuestionNumber = questionNumber;
            QuestionText = questionText;
            QuestionAnswer = answer;
            AnswerReasoning = answerReasoning;
            Option1 = option1;
            Option2 = option2;
            Option3 = option3;
            Option4 = option4;
        }
    }
}