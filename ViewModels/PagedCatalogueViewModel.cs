using Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class PagedCatalogueViewModel
    {
        public IEnumerable<Burger> Burgers { get; set; } = new List<Burger>();
        public IEnumerable<Menu> Menus { get; set; } = new List<Menu>();
        public IEnumerable<Complement> Complements { get; set; } = new List<Complement>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Filter { get; set; } = "Burger";
        public string? Search { get; set; }
    }
}
