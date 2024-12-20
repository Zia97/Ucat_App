[System.Serializable]
public class UserSaveDataModel
{
    public int questionNumber;
    public string usersAnswer;

    private UserSaveDataModel() { }

    public class Builder
    {
        private int questionNumber;
        private string usersAnswer;

        public Builder SetQuestionNumber(int questionNumber)
        {
            this.questionNumber = questionNumber;
            return this;
        }

        public Builder SetUsersAnswer(string usersAnswer)
        {
            this.usersAnswer = usersAnswer;
            return this;
        }

        public UserSaveDataModel Build()
        {
            return new UserSaveDataModel
            {
                questionNumber = this.questionNumber,
                usersAnswer = this.usersAnswer
            };
        }
    }
}