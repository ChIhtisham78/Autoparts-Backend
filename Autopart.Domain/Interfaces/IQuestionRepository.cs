using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IQuestionRepository : IRepository<Question>
	{
		void AddQuestions(Question question);

		Task<(IEnumerable<Question> Questions, int TotalCount)> GetQuestions(int page = 1, int limit = 10, int? userId = null, int? productId = null);

		Task<List<Question>> GetQuestions();
		Task<Question> GetByIdAsync(int id);
		void Delete(Question question);
	}
}
