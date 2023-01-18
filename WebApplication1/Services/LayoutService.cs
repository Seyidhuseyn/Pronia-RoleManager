using WebApplication1.DAL;

namespace WebApplication1.Services
{
    public class LayoutService
    {
        readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public Dictionary<string , string> GetSettings()
        {
            return _context.Settings.ToDictionary(s=>s.Key, s=>s.Value);
        }

    }
}
