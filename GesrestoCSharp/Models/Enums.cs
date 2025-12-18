namespace Models.Enums
{
    public enum EtatProduit
    {
        Disponible,
        Archived
    }

    public enum ComplementType
    {
        Frites,
        Boisson
    }

    public enum EtatCommande
    {
        EnAttente,
        Valide,
        Termine,
        Annule
    }

    public enum CommandeType
    {
        Burger,
        Menu
    }

    public enum ConsoType
    {
        SurPlace,
        AEmporter,
        ALivrer
    }

    public enum PaiementType
    {
        Wave,
        OrangeMoney
    }
}
