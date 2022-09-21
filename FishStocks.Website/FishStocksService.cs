using FishStocks.Website.Data.FishStocks;
using Microsoft.EntityFrameworkCore;

namespace FishStocks.Website
{
    public class FishStocksService
    {
        private readonly FishStocksContext _context;
        public FishStocksService(FishStocksContext context)
        {
            _context = context;
        }
        public async Task<List<FishTransaction>> GetFishTransactionsAsync()
        {
            return await _context.FishTransaction.Where(p => p.Id == 1).AsNoTracking().ToListAsync();
        }
    }
}
