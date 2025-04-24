using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class QuestionRepository : IQuestionRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;
		public QuestionRepository(autopartContext context)
		{
			_context = context;
		}

		public void AddQuestions(Question question)
		{
			_context.Questions.Add(question);
		}

		public async Task<List<Question>> GetQuestions()
		{
			return await _context.Questions.Include(r => r.Product).Include(r => r.User).ToListAsync();
		}

		public async Task<(IEnumerable<Question> Questions, int TotalCount)> GetQuestions(int page = 1, int limit = 10, int? userId = null, int? productId = null)
		{
			var query = _context.Questions
								.Include(q => q.Product)
								.Include(q => q.User)
								.AsQueryable();

			if (userId.HasValue)
			{
				query = query.Where(q => q.UserId == userId.Value);
			}

			if (productId.HasValue)
			{
				query = query.Where(q => q.ProductId == productId.Value);
			}

			var totalCount = await query.CountAsync();

			var questions = await query
								.Skip((page - 1) * limit)
								.Take(limit)
								.ToListAsync();

			return (questions, totalCount);
		}

		public async Task<Question> GetByIdAsync(int id)
		{
			var question = await _context.Questions.Include(x=>x.Product).Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            return question!;
        }

		public void Delete(Question question)
		{
			_context.Questions.Remove(question);
		}
	}
}
