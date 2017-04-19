using System;

namespace ACD.App
{
	public partial class QuestionView
	{
		public static QuestionView Create<T>(int number, T question)
		{
			if (question is RateScaleQuestion)
				return new RateScaleQuestionView(number, question as RateScaleQuestion);
            else if (question is OptionQuestion)
                return new OptionQuestionView(number, question as OptionQuestion);

			throw new NotImplementedException();
		}
	}
}

