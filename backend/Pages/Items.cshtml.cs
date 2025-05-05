using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Pages
{
    public class ItemsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ItemsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Item> Items { get; set; } = new List<Item>();

        public async Task OnGetAsync()
        {
            Items = await _context.Items.ToListAsync();
        }
    }
}