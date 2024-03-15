using AnalyticsMicroservice.Contexts;
using AnalyticsMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AnalyticsMicroservice.Services {
    public interface ILocationService {
        Task<LocationModel?> GetLocationById(uint id);
        Task<LocationModel?> GetLocationByName(string name);
        Task<IEnumerable<LocationModel>> SearchLocations(string name, int limit, int offset);
        Task<LocationModel> AddLocation(LocationModel model);
        Task<IEnumerable<LocationModel>> AddLocations(IEnumerable<LocationModel> models);
    }

    public class LocationService(ApplicationContext context) : ILocationService {
        private readonly ApplicationContext _context = context;

        public async Task<LocationModel?> GetLocationById(uint id) {
            var location = await _context.Locations.FirstOrDefaultAsync(data => data.Id == id);
            return location;
        }

        public async Task<LocationModel?> GetLocationByName(string name) {
            var location = await _context.Locations.FirstOrDefaultAsync(data => data.Name == name);
            return location;
        }

        public async Task<IEnumerable<LocationModel>> SearchLocations(string name, int limit, int offset) {
            var locations = await _context.Locations.Where(data => EF.Functions.Like(data.Name.ToLower(), $"%{name.ToLower()}%")).OrderBy(data => data.Id).Take(limit).Skip(offset).ToListAsync();
            return locations;
        }

        public async Task<LocationModel> AddLocation(LocationModel model) {
            var location = await _context.Locations.AddAsync(model);
            await _context.SaveChangesAsync();
            return location.Entity;
        }

        public async Task<IEnumerable<LocationModel>> AddLocations(IEnumerable<LocationModel> models) {
            await _context.AddRangeAsync(models);
            await _context.SaveChangesAsync();
            return models;
        }
    }
}
