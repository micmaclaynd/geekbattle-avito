using AnalyticsMicroservice.Contexts;
using AnalyticsMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsMicroservice.Services {
    public interface IMatrixService {
        Task<MatrixModel?> GetMatrixById(uint id);
        Task<MatrixModel?> GetMatrixByName(string name);
        Task<IEnumerable<MatrixModel>> SearchMatrix(string name, int limit, int offset);
        Task<MatrixModel> AddMatrix(MatrixModel model);
    }

    public class MatrixService(ApplicationContext context) : IMatrixService {
        private readonly ApplicationContext _context = context;

        public async Task<MatrixModel?> GetMatrixById(uint id) {
            var matrix = await _context.Matrices.FirstOrDefaultAsync(data => data.Id == id);
            return matrix;
        }

        public async Task<MatrixModel?> GetMatrixByName(string name) {
            var matrix = await _context.Matrices.FirstOrDefaultAsync(data => data.Name == name);
            return matrix;
        }

        public async Task<IEnumerable<MatrixModel>> SearchMatrix(string name, int limit, int offset) {
            var matrices = await _context.Matrices.Where(data => EF.Functions.Like(data.Name.ToLower(), $"%{name.ToLower()}%")).OrderBy(data => data.Id).Take(limit).Skip(offset).ToListAsync();
            return matrices;
        }

        public async Task<MatrixModel> AddMatrix(MatrixModel model) {
            var matrix = await _context.Matrices.AddAsync(model);
            await _context.SaveChangesAsync();
            return matrix.Entity;
        }
    }
}
