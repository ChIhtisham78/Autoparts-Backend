using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class QuestionService : IQuestionService
	{
		private readonly IQuestionRepository _questionRepository;
		private readonly ITypeAdapter _typeAdapter;

		public QuestionService(IQuestionRepository questionRepository, ITypeAdapter typeAdapter)
		{
			_questionRepository = questionRepository;
			_typeAdapter = typeAdapter;
		}


		public async Task<QuestionDto> AddQuestion(QuestionDto questionDto)
		{
			try
			{
				var Question = new Question()
				{
					UserId = questionDto.UserId,
					ProductId = questionDto.ProductId,
					ShopId = questionDto.ShopId,
					Question1 = questionDto.Question,
					Answer = questionDto.Answer,
					PositiveFeedbacksCount = questionDto.PositiveFeedbacksCount,
					NegativeFeedbacksCount = questionDto.NegativeFeedbacksCount


				};


				_questionRepository.AddQuestions(Question);
				await _questionRepository.UnitOfWork.SaveChangesAsync();

			return _typeAdapter.Adapt<QuestionDto>(Question);
				
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to invalidate JWT token.", ex);

			}

		}


		public async Task<(IEnumerable<GetQuestionDto> Questions, int TotalCount)> GetQuestions(int? userId = null, int? productId = null, int page = 1, int limit = 10)
		{
			var (questions, totalCount) = await _questionRepository.GetQuestions(page, limit, userId, productId);

			var questionDtos = questions.Select(q => new GetQuestionDto
			{
				Id = q.Id,
				UserId = q.UserId,
				ShopId = q.ShopId ?? 0,
				ProductId = q.ProductId ?? 0,
				Question = q.Question1,
				Answer = q.Answer,
				PositiveFeedbacksCount = q.PositiveFeedbacksCount ?? 0,
				NegativeFeedbacksCount = q.NegativeFeedbacksCount ?? 0,
				AbusiveReportsCount = q.AbusiveReportsCount ?? 0,
				CreatedAt = q.CreatedAt,
				MyFeedback = q.MyFeedback,

				Product = q.Product != null ? new RatingProductsDto
				{
					Id = q.Product.Id,
					Name = q.Product.Name ?? string.Empty,
					Slug = q.Product.Slug ?? string.Empty,
					Description = q.Product.Description ?? string.Empty,
					TypeId = q.Product.TypeId ?? 0,
					Price = q.Product.Price ?? 0,
					ShopId = q.Product.ShopId ?? 0,
					SalePrice = q.Product.SalePrice ?? 0,
					Language = q.Product.Language ?? string.Empty,
					MinPrice = q.Product.MinPrice ?? 0,
					MaxPrice = q.Product.MaxPrice ?? 0,
					Sku = q.Product.Sku ?? string.Empty,
					Quantity = q.Product.Quantity ?? 0
				} : null ?? new RatingProductsDto(),

				User = q.User != null ? new RatingUsersDto
				{
					Id = q.UserId ?? 0,
					Name = q.User.UserName ?? string.Empty,
					Email = q.User.Email ?? string.Empty,
					CreatedAt = q.User.CreatedAt,
					UpdatedAt = q.User.UpdatedAt,
					IsActive = q.User.IsActive
				} : null ?? new RatingUsersDto()
			}).ToList();

			return (questionDtos, totalCount);
		}







		public async Task<IEnumerable<GetQuestionDto>> GetQuestion(int id)
		{
			try
			{
				var question = await _questionRepository.GetByIdAsync(id);

				if (question == null)
				{
					return Enumerable.Empty<GetQuestionDto>();
				}

				var questionDtos = new List<GetQuestionDto>
		{
			new GetQuestionDto
			{
				Id = question.Id,
				UserId = question.UserId,
				ShopId = question.ShopId ?? 0,
				ProductId = question.ProductId ?? 0,
				Question = question.Question1,
				Answer = question.Answer,
				PositiveFeedbacksCount = question.PositiveFeedbacksCount ?? 0,
				NegativeFeedbacksCount = question.NegativeFeedbacksCount ?? 0,
				AbusiveReportsCount = question.AbusiveReportsCount ?? 0,
				CreatedAt = question.CreatedAt,
				MyFeedback = question.MyFeedback,

				Product = question.Product != null ? new RatingProductsDto
				{
					Id = question.Product.Id,
					Name = question.Product.Name ?? string.Empty,
					Slug = question.Product.Slug ?? string.Empty,
					Description = question.Product.Description ?? string.Empty,
					TypeId = question.Product.TypeId ?? 0,
					Price = question.Product.Price ?? 0,
					ShopId = question.Product.ShopId ?? 0,
					SalePrice = question.Product.SalePrice ?? 0,
					Language = question.Product.Language ?? string.Empty,
					MinPrice = question.Product.MinPrice ?? 0,
					MaxPrice = question.Product.MaxPrice ?? 0,
					Sku = question.Product.Sku ?? string.Empty,
					Quantity = question.Product.Quantity ?? 0
				} : null ?? new RatingProductsDto(),

				User = question.User != null ? new RatingUsersDto
				{
					Id = question.UserId ?? 0,
					Name = question.User.UserName ?? string.Empty,
					Email = question.User.Email ?? string.Empty,
					CreatedAt = question.User.CreatedAt,
					UpdatedAt = question.User.UpdatedAt,
					IsActive = question.User.IsActive
				} : null ?? new RatingUsersDto()
			}
		};

				return questionDtos;
			}
			catch (Exception ex)
			{
				// Log the error and return an empty list
				Console.WriteLine($"An error occurred while retrieving the question: {ex.Message}");

				return Enumerable.Empty<GetQuestionDto>();
			}
		}



		public async Task<bool> DeleteQuestionAsync(int id)
		{
			var question = await _questionRepository.GetByIdAsync(id);
			if (question == null)
			{
				return false;
			}

			_questionRepository.Delete(question);
			await _questionRepository.UnitOfWork.SaveChangesAsync();
			return true;
		}
	}
}
