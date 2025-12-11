package ges.resto.service;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.Scanner;

public class Sys {
    
    public static void cls() {
        try {
            if (System.getProperty("os.name").contains("Windows")) {
                new ProcessBuilder("cmd", "/c", "cls").inheritIO().start().waitFor();
            } else {
                new ProcessBuilder("clear").inheritIO().start().waitFor();
            }
        } catch (Exception e) {
            System.out.println("Impossible de nettoyer la console.");
        }
    }

    public static void pause() {
        System.out.println("\nAppuyez sur Entrée pour continuer...");
        Scanner sc = new Scanner(System.in);
        sc.nextLine();
    }

    public static String capitalize(String mot) {
        if (mot == null || mot.isEmpty()) {
            return mot;
        }
        return mot.substring(0, 1).toUpperCase() + mot.substring(1).toLowerCase();
    }

    public static String date(LocalDate date) {
    DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd-MM-yyyy");
    return date.format(formatter);
    }

}
