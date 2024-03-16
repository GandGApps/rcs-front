using System;
using System.Collections.Generic;
using System.Text;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.Repositories;
public interface IDistrictRepository : IRepository<District>
{
    public Task<IEnumerable<Street>> StreetList(Guid districtId);

    internal static IDistrictRepository CreateMock(string jsonResourceName, IRepository<Street> repository)
    {
        var districtRepository = CreateMock(jsonResourceName);

        return new Mock(districtRepository, repository);
    }

    private class Mock : IDistrictRepository
    {
        private readonly IRepository<District> _repository;
        private readonly IRepository<Street> _streetRepository;

        public Mock(IRepository<District> repository, IRepository<Street> streetRepository)
        {
            _repository = repository;
            _streetRepository = streetRepository;
        }

        public Task Add(District item) => _repository.Add(item);
        public Task Delete(District item) => _repository.Delete(item);
        public Task DeleteAll() => _repository.DeleteAll();
        public Task<District?> Get(Guid id) => _repository.Get(id);
        public Task<IEnumerable<District>> GetAll() => _repository.GetAll();
        public async Task<IEnumerable<Street>> StreetList(Guid districtId)
        {
            var streets = await _streetRepository.GetAll();

            return streets.Where(street => street.DistrictId == districtId);
        }
        public Task Update(District item) => _repository.Update(item);
    }
}
