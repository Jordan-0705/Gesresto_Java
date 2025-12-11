package ges.resto.view;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Burger;
import ges.resto.entity.Etat;
import ges.resto.service.BurgerService;

public class BurgerView extends View {

    private static BurgerView instance = null;

    public static BurgerView getInstance(BurgerService burgerService) {
        if (instance == null) {
            instance = new BurgerView(burgerService);
        }
        return instance;
    }

    private BurgerService burgerService;

    private BurgerView(BurgerService burgerService) {
        this.burgerService = burgerService;
    }

    public void showMenu() {
    int choix;

    do {
        System.out.println("\n===== BURGER VIEW =====\n");
        System.out.println("1 - Ajouter un burger");
        System.out.println("2 - Lister les burgers");
        System.out.println("3 - Rechercher par ID");
        System.out.println("4 - Modifier un burger");
        System.out.println("5 - Supprimer un burger");
        System.out.println("6 - Archiver un burger");
        System.out.println("0 - Quitter");
        System.out.print("Votre choix : ");

        choix = saisieEntier("");

        switch (choix) {
            case 1 -> ajouterBurger();
            case 2 -> listerBurgers();
            case 3 -> rechercherBurger();
            case 4 -> modifierBurger();
            case 5 -> supprimerBurger();
            case 6 -> archiverBurger();
        }

    } while (choix != 0);
}


    private void ajouterBurger() {
        System.out.println("\n--- AJOUT BURGER ---\n");

        Burger b = new Burger();

        b.setNom(saisieChaine("Nom : "));
        b.setPrix((saisieDouble("Prix : ")));
        b.setEtat(Etat.Disponible);

        int result = burgerService.addBurger(b);
        System.out.println(result > 0 ? "Burger ajouté !" : "Erreur d'ajout.");
    }

    private void listerBurgers() {
        System.out.println("\n--- LISTE DES BURGERS ---");

        List<Burger> burgers = burgerService.findAllBurger();
        if (burgers.isEmpty()) {
            System.out.println("Aucun burger trouvé.");
            return;
        }

        for (Burger b : burgers) {
            System.out.println(b);
        }
    }

    private void rechercherBurger() {
        System.out.println("\n--- RECHERCHE BURGER ---\n");

        int id = saisieEntier("ID du burger : ");

        Optional<Burger> opt = burgerService.findById(id);

        if (opt.isPresent()) {
            Burger b = opt.get();
            System.out.println("Trouvé : \n" + b);
        } else {
            System.out.println("Aucun burger avec cet ID.");
        }
    }

    private void supprimerBurger() {
        int id = saisieEntier("ID du burger à supprimer : ");

        int result = burgerService.deleteBurger(id);

        if (result > 0) {
            System.out.println("Burger supprimé !");
        } else {
            System.out.println("Aucun burger supprimé.");
        }
    }


    private void archiverBurger() {
        int id = saisieEntier("ID du burger à archiver : ");

        Optional<Burger> opt = burgerService.findById(id);

        if (opt.isPresent()) {
            Burger b = opt.get();
            b.setEtat(Etat.Archived);

            int result = burgerService.updateBurger(b);

            if (result > 0) {
                System.out.println("Burger archivé !");
            } else {
                System.out.println("Erreur lors de l'archivage.");
            }
        } else {
            System.out.println("Aucun burger avec cet ID.");
        }
    }

    public void modifierBurger() {
    int id = saisieEntier("Entrer l'ID du burger à modifier : ");

    Optional<Burger> opt = burgerService.findById(id);
    if (opt == null) {
        System.out.println("Burger introuvable.");
        return;
    }
    Burger burger = opt.get();
    System.out.println("=== Modification du burger " + burger.getNom() + " ===");

    String nom = saisieChaine("Nouveau nom : ");
    int prix = saisieEntier("Nouveau prix : ");
    Etat etat = saisieEtat();

    burger.setNom(nom);
    burger.setPrix(prix);
    burger.setEtat(etat);

    burgerService.updateBurger(burger);

    System.out.println("Burger modifié avec succès !");
}

}
