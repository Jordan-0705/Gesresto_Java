package ges.resto.view;

import java.util.List;
import java.util.Optional;
import java.util.Scanner;

import ges.resto.entity.Menu;
import ges.resto.entity.Burger;
import ges.resto.entity.Complement;
import ges.resto.entity.Etat;
import ges.resto.service.MenuService;

public class MenuView extends View {

    private static MenuView instance = null;

    public static MenuView getInstance(MenuService menuService) {
        if (instance == null) {
            instance = new MenuView(menuService);
        }
        return instance;
    }

    private MenuService menuService;

    private MenuView(MenuService menuService) {
        this.menuService = menuService;
    }

    public void showMenu() {
        int choix;

        do {
            System.out.println("\n===== MENU VIEW =====\n");
            System.out.println("1 - Ajouter un menu");
            System.out.println("2 - Lister les menus");
            System.out.println("3 - Rechercher par ID");
            System.out.println("4 - Modifier un menu");
            System.out.println("5 - Supprimer un menu");
            System.out.println("6 - Archiver un menu");
            System.out.println("0 - Quitter");
            System.out.print("Votre choix : ");

            choix = saisieEntier("");

            switch (choix) {
                case 1 -> ajouterMenu();
                case 2 -> listerMenus();
                case 3 -> rechercherMenu();
                case 4 -> modifierMenu();
                case 5 -> supprimerMenu();
                case 6 -> archiverMenu();
            }

        } while (choix != 0);
    }

    private void ajouterMenu() {
        System.out.println("\n--- AJOUT MENU ---\n");

        Menu m = new Menu();

        m.setNom(saisieChaine("Nom du menu : "));
        m.setPrix(saisieDouble("Prix du menu : "));

        int burgerId = saisieEntier("ID du burger : ");
        Burger burger = new Burger();
        burger.setId(burgerId);
        m.setBurger(burger);

        int fritesId = saisieEntier("ID des frites : ");
        Complement frites = new Complement();
        frites.setId(fritesId);
        m.setFrites(frites);

        int boissonId = saisieEntier("ID de la boisson : ");
        Complement boisson = new Complement();
        boisson.setId(boissonId);
        m.setBoisson(boisson);

        m.setEtat(saisieEtat());

        int result = menuService.addMenu(m);
        System.out.println(result > 0 ? "Menu ajouté !" : "Erreur d'ajout.");
    }

    private void listerMenus() {
        System.out.println("\n--- LISTE DES MENUS ---");

        List<Menu> menus = menuService.findAllMenu();
        if (menus.isEmpty()) {
            System.out.println("Aucun menu trouvé.");
            return;
        }

        for (Menu m : menus) {
            System.out.println(m);
        }
    }

    private void rechercherMenu() {
        System.out.println("\n--- RECHERCHE MENU ---\n");

        int id = saisieEntier("ID du menu : ");

        Optional<Menu> opt = menuService.findById(id);

        if (opt.isPresent()) {
            Menu m = opt.get();
            System.out.println("Trouvé : \n" + m);
        } else {
            System.out.println("Aucun menu avec cet ID.");
        }
    }

    private void supprimerMenu() {
        int id = saisieEntier("ID du menu à supprimer : ");

        int result = menuService.deleteMenu(id);

        if (result > 0) {
            System.out.println("Menu supprimé !");
        } else {
            System.out.println("Aucun menu supprimé.");
        }
    }

    private void archiverMenu() {
        int id = saisieEntier("ID du menu à archiver : ");

        Optional<Menu> opt = menuService.findById(id);

        if (opt.isPresent()) {
            Menu m = opt.get();
            m.setEtat(Etat.Archived);

            int result = menuService.updateMenu(m);

            if (result > 0) {
                System.out.println("Menu archivé !");
            } else {
                System.out.println("Erreur lors de l'archivage.");
            }
        } else {
            System.out.println("Aucun menu avec cet ID.");
        }
    }

    private void modifierMenu() {
        int id = saisieEntier("ID du menu à modifier : ");

        Optional<Menu> opt = menuService.findById(id);
        if (opt.isEmpty()) {
            System.out.println("Menu introuvable.");
            return;
        }

        Menu m = opt.get();
        System.out.println("=== Modification du menu " + m.getNom() + " ===");

        String nom = saisieChaine("Nouveau nom : ");
        double prix = saisieDouble("Nouveau prix : ");
        int burgerId = saisieEntier("ID du burger : ");
        int fritesId = saisieEntier("ID des frites : ");
        int boissonId = saisieEntier("ID de la boisson : ");
        Etat etat = saisieEtat();

        m.setNom(nom);
        m.setPrix(prix);

        Burger burger = new Burger();
        burger.setId(burgerId);
        m.setBurger(burger);

        Complement frites = new Complement();
        frites.setId(fritesId);
        m.setFrites(frites);

        Complement boisson = new Complement();
        boisson.setId(boissonId);
        m.setBoisson(boisson);

        m.setEtat(etat);

        menuService.updateMenu(m);
        System.out.println("Menu modifié avec succès !");
    }
}
