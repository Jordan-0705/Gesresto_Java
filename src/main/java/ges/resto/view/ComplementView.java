package ges.resto.view;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Complement;
import ges.resto.entity.ComplementType;
import ges.resto.service.ComplementService;

public class ComplementView extends View {

    private static ComplementView instance = null;

    public static ComplementView getInstance(ComplementService complementService) {
        if (instance == null) {
            instance = new ComplementView(complementService);
        }
        return instance;
    }

    private ComplementService complementService;

    private ComplementView(ComplementService complementService) {
        this.complementService = complementService;
    }

    public void showMenu() {
        int choix;

        do {
            System.out.println("\n===== COMPLEMENT VIEW =====\n");
            System.out.println("1 - Ajouter un complément");
            System.out.println("2 - Lister les compléments");
            System.out.println("3 - Rechercher par ID");
            System.out.println("4 - Modifier un complément");
            System.out.println("5 - Supprimer un complément");
            System.out.println("0 - Quitter");
            System.out.print("Votre choix : ");

            choix = saisieEntier("");

            switch (choix) {
                case 1 -> ajouterComplement();
                case 2 -> listerComplements();
                case 3 -> rechercherComplement();
                case 4 -> modifierComplement();
                case 5 -> supprimerComplement();
            }

        } while (choix != 0);
    }

    private void ajouterComplement() {
        System.out.println("\n--- AJOUT COMPLEMENT ---\n");

        Complement c = new Complement();

        c.setNom(saisieChaine("Nom : "));
        c.setPrix(saisieDouble("Prix : "));
        c.setComplementType(saisieType());

        int result = complementService.addComplement(c);
        System.out.println(result > 0 ? "Complément ajouté !" : "Erreur d'ajout.");
    }

    private void listerComplements() {
        System.out.println("\n--- LISTE DES COMPLEMENTS ---");

        List<Complement> complements = complementService.findAllComplement();
        if (complements.isEmpty()) {
            System.out.println("Aucun complément trouvé.");
            return;
        }

        for (Complement c : complements) {
            System.out.println(c);
        }
    }

    private void rechercherComplement() {
        System.out.println("\n--- RECHERCHE COMPLEMENT ---\n");

        int id = saisieEntier("ID du complément : ");

        Optional<Complement> opt = complementService.findById(id);

        if (opt.isPresent()) {
            Complement c = opt.get();
            System.out.println("Trouvé : \n" + c);
        } else {
            System.out.println("Aucun complément avec cet ID.");
        }
    }

    private void supprimerComplement() {
        int id = saisieEntier("ID du complément à supprimer : ");

        int result = complementService.deleteComplement(id);

        if (result > 0) {
            System.out.println("Complément supprimé !");
        } else {
            System.out.println("Aucun complément supprimé.");
        }
    }

    public void modifierComplement() {
        int id = saisieEntier("ID du complément à modifier : ");

        Optional<Complement> opt = complementService.findById(id);
        if (opt.isEmpty()) {
            System.out.println("Complément introuvable.");
            return;
        }

        Complement c = opt.get();
        System.out.println("=== Modification du complément " + c.getNom() + " ===");

        String nom = saisieChaine("Nouveau nom : ");
        double prix = saisieDouble("Nouveau prix : ");
        ComplementType type = saisieType();

        c.setNom(nom);
        c.setPrix(prix);
        c.setComplementType(type);

        complementService.updateComplement(c);

        System.out.println("Complément modifié avec succès !");
    }
}
