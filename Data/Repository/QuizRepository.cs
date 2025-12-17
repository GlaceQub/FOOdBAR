namespace Restaurant.Data.Repository
{
    public class QuizRepository : IQuizRepository
    {
        private readonly RestaurantContext _context;

        public QuizRepository(RestaurantContext context)
        {
            _context = context;
        }

        // Haal quiz-eigenschappen op voor gerechten (categorie type "Eten")
        public async Task<List<Quiz>> GetQuizEigenschappenVoorGerechtenAsync()
        {
            return await _context.QuizEigenschappen
                .Include(q => q.Product)
                    .ThenInclude(p => p.Categorie)
                        .ThenInclude(c => c.Type)
                .Where(q => q.Product.Categorie.Type.Naam == "Eten")
                .ToListAsync();
        }

        // Haal quiz-eigenschappen op voor dranken (categorie type "Dranken")
        public async Task<List<Quiz>> GetQuizEigenschappenVoorDrankenAsync()
        {
            return await _context.QuizEigenschappen
                .Include(q => q.Product)
                    .ThenInclude(p => p.Categorie)
                        .ThenInclude(c => c.Type)
                .Where(q => q.Product.Categorie.Type.Naam == "Dranken")
                .ToListAsync();
        }
    }
}
