using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class AbstractReasoningTupleHolder
    {
        public int questionNumber;
        public string imageURI;
        public string questionAnswer;
        public string usersAnswer;

        public AbstractReasoningTupleHolder(int _questionNumber, string _imageUri, string _questionAnswer, string _usersAnswer)
        {
            questionNumber = _questionNumber;
            imageURI = _imageUri;
            questionAnswer = _questionAnswer;
            usersAnswer = _usersAnswer;

        }

    }
}
