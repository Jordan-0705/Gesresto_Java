// ViewModels/CommandeViewModel.cs
using System.Collections.Generic;

namespace GesResto.ViewModels
{
    public class CommandeViewModel
    {
        public string CommandeType { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public string ConsoType { get; set; } = string.Empty;
        
        // Pour burgers : Dictionnaire pour faciliter le binding
        public Dictionary<int, BurgerSelection> BurgerSelections { get; set; } = new();
        
        // Pour menus : Dictionnaire pour faciliter le binding  
        public Dictionary<int, MenuSelection> MenuSelections { get; set; } = new();
        
        // Compléments optionnels
        public List<ComplementSelection> ComplementSelections { get; set; } = new();
    }

    public class BurgerSelection
    {
        public int BurgerId { get; set; }
        public bool IsSelected { get; set; }
        public int Quantite { get; set; } = 1;
        public string BurgerNom { get; set; } = string.Empty;
        public decimal Prix { get; set; }
    }

    public class MenuSelection
    {
        public int MenuId { get; set; }
        public bool IsSelected { get; set; }
        public int Quantite { get; set; } = 1;
        public string MenuNom { get; set; } = string.Empty;
        public decimal Prix { get; set; }
    }

    public class ComplementSelection
    {
        public int ComplementId { get; set; }
        public bool IsSelected { get; set; }
        public int Quantite { get; set; } = 1;
        public string ComplementNom { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}