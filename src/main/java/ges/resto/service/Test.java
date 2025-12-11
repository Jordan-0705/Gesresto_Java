package ges.resto.service;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;

public class Test {
    
    public static boolean isNumeric(String saisie) {
        return saisie.trim().matches("-?\\d+");
    }

    public static boolean isDouble(String str) {
    if (str == null || str.trim().isEmpty()) {
        return false;
    }
    try {
        Double.parseDouble(str);
        return true;
    } catch (NumberFormatException e) {
        return false;
    }
}


    public static boolean isAlpha(String saisie) {
        return saisie.trim().matches("[a-zA-Z]+");
    }

    public static int isTel(String numero){
        if (numero.length() != 9){
            return 0;
        }
        if (!numero.substring(0, 2).equals("77") && 
        !numero.substring(0, 2).equals("78") && 
        !numero.substring(0, 2).equals("70") && 
        !numero.substring(0, 2).equals("76")) {
            return 1;
        }
        char[] chiffre = numero.toCharArray();
        for (int i = 2; i < 9; i++){
            if (chiffre[i] < '0' || chiffre[i] > '9'){
                return 2;
            }
        }
        return 3;
    }

    public static int isFix(String numero){
        if (numero.length() != 9){
            return 0;
        }
        if (!numero.substring(0, 2).equals("33")) {
            return 1;
        }
        char[] chiffre = numero.toCharArray();
        for (int i = 2; i < 9; i++){
            if (chiffre[i] < '0' || chiffre[i] > '9'){
                return 2;
            }
        }
        return 3;
    }

    public static boolean isDateValide(String date) {
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd-MM-yyyy");
        try {
            LocalDate dateSaisie = LocalDate.parse(date, formatter);
            LocalDate aujourdHui = LocalDate.now();
            LocalDate dateMin = LocalDate.of(2000, 1, 1); // borne minimale autorisée

            // Vérifie si la date est entre 01-01-2000 et aujourd'hui
            return !dateSaisie.isAfter(aujourdHui) && !dateSaisie.isBefore(dateMin);

        } catch (DateTimeParseException e) {
            return false;
        }
    }


}
