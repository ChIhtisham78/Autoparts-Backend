using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface IQuestionService
	{
		Task<QuestionDto> AddQuestion(QuestionDto questionDto);
		//Task<IEnumerable<GetQuestionDto>> GetQuestions();
		Task<(IEnumerable<GetQuestionDto> Questions, int TotalCount)> GetQuestions(int? userId = null, int? productId = null, int page = 1, int limit = 10);
		Task<IEnumerable<GetQuestionDto>> GetQuestion(int id);

		Task<bool> DeleteQuestionAsync(int id);
	}
}
