using Autopart.Application.Interfaces;
using Autopart.Application.Options;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Microsoft.Extensions.Options;

namespace Autopart.Application.Services
{
    public class TempService : ITempService
    {
        private readonly ITempRepository _tempRepository;
        private readonly AuthenticationOptions _authenticationOptions;
        public TempService(ITempRepository tempRepository,
            IOptions<AuthenticationOptions> options)
        {
            _tempRepository = tempRepository;
            _authenticationOptions = options.Value;
        }

        public async Task<Temp> AddTemp(string name)
        {
            var temp = new Temp();
            temp.Name = name;
            _tempRepository.AddTemp(temp);
            await _tempRepository.UnitOfWork.SaveChangesAsync();

            return temp;
        }

        public async Task<Temp> GetTempById(int id)
        {
            return await _tempRepository.GetTempById(id);
        }

        public async Task<List<Temp>> GetTemps()
        {
            return await _tempRepository.GetTemps();
        }

        public async Task<Temp> RemoveTemp(int id)
        {
            var temp = await _tempRepository.GetTempById(id);
            if (temp == null)
            {
                throw new DomainException("temp not exists");
            }
            _tempRepository.DeleteTemp(temp);
            await _tempRepository.UnitOfWork.SaveChangesAsync();

            return temp;
        }

        public async Task<Temp> UpdateTemp(int id, string name)
        {
            var temp = await _tempRepository.GetTempById(id);
            if (temp == null)
            {
                throw new DomainException("temp not exists");
            }
            temp.Name = name;
            _tempRepository.UpdateTemp(temp);
            await _tempRepository.UnitOfWork.SaveChangesAsync();

            return temp;
        }
    }
}
