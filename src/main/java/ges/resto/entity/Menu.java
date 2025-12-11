package ges.resto.entity;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor

public class Menu {
    private int id;
    private String code;
    private String nom;
    private Burger burger;
    private Complement frites;
    private Complement boisson;
    private double prix;
    private Etat etat;

    @Override
    public String toString() {
        return String.format(
                "%03d | %-7s | %-20s | BURGER: %-20s | FRITE: %-15s | BOISSON: %-15s | %8.2f",
                id,
                code,
                nom,
                burger != null ? burger.getNom() : "Aucun",
                frites != null ? frites.getNom() : "Aucune",
                boisson != null ? boisson.getNom() : "Aucune",
                prix
        );
    }
}
