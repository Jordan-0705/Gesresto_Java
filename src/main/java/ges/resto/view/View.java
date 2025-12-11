package ges.resto.view;

import java.util.Scanner;

import ges.resto.entity.ComplementType;
import ges.resto.entity.Etat;
import ges.resto.service.Test;


public class View {
    
    protected static String saisieChaine(String message) {

    String mot;
    Scanner scanner = new Scanner(System.in);

    do {
        System.out.print(message);
        mot = scanner.nextLine();
    } while (!mot.matches("[a-zA-Z ]+"));  // <-- Autorise lettres + espaces

    return mot.trim();  // retire les espaces inutiles au début et fin
}


    // protected static String saisieTel(String message) {
        
    //     String tel;
    //     Scanner scanner = new Scanner(System.in);

    //     do {
    //         System.out.println(message);
    //         tel = scanner.nextLine();
    //         switch (Test.isTel(tel)) {
    //             case 0:
    //                 System.err.println("Le numero doit contenir 9 chiffres !");
    //                 break;
    //             case 1:
    //                 System.err.println("Le numero doit commencer par 77, 78, 70 ou 76 !");
    //                 break;
    //             case 2:
    //                 System.err.println("Le numero ne doit contenir que des chiffres !");
    //                 break;
    //             default:
    //                 break;
    //         }
    //     } while (Test.isTel(tel) != 3);
    //     return tel;
    // }

    // protected static String saisieFix(String message) {
        
    //     String fix;
    //     Scanner scanner = new Scanner(System.in);

    //     do {
    //         System.out.println(message);
    //         fix = scanner.nextLine();
    //         switch (Test.isFix(fix)) {
    //             case 0:
    //                 System.err.println("Le numero doit contenir 9 chiffres !");
    //                 break;
    //             case 1:
    //                 System.err.println("Le numero doit commencer par 33 !");
    //                 break;
    //             case 2:
    //                 System.err.println("Le numero ne doit contenir que des chiffres !");
    //                 break;
    //             default:
    //                 break;
    //         }
    //     } while (Test.isFix(fix) != 3);
    //     return fix;
    // }

    protected static double saisieDouble(String message) {
        
        String mot;
        double reel;
        Scanner scanner = new Scanner(System.in);

        do {
            System.out.print(message);
            mot = scanner.nextLine();
        } while (!Test.isDouble(mot));
        reel = Double.parseDouble(mot);
        return reel;
    }

    protected static int saisieEntier(String message) {
        
        String mot;
        int entier;
        Scanner scanner = new Scanner(System.in);

        do {
            System.out.print(message);
            mot = scanner.nextLine();
        } while (!Test.isNumeric(mot));
        entier = Integer.parseInt(mot);
        return entier;
    }

    protected static Etat saisieEtat() {
        Scanner sc = new Scanner(System.in);
        String choix;

        while (true) {
            System.out.println("Choisissez l'état :");
            System.out.println("1 - Disponible");
            System.out.println("2 - Archived");
            System.out.print("Votre choix : ");

            choix = sc.nextLine().trim();

            switch (choix) {
                case "1":
                    return Etat.Disponible;
                case "2":
                    return Etat.Archived;
                default:
                    System.out.println("Choix invalide, veuillez réessayer.\n");
            }
        }
    }

    protected static ComplementType saisieType() {
        Scanner sc = new Scanner(System.in);
        String choix;

        while (true) {
            System.out.println("Choisissez le type de complément :");
            System.out.println("1 - FRITE");
            System.out.println("2 - BOISSON");
            System.out.print("Votre choix : ");

            choix = sc.nextLine().trim();

            switch (choix) {
                case "1":
                    return ComplementType.Frites;
                case "2":
                    return ComplementType.Boisson;
                default:
                    System.out.println("Choix invalide, veuillez réessayer.\n");
            }
        }
    }


}