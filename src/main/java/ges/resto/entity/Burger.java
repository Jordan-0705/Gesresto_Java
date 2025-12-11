package ges.resto.entity;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor

public class Burger {
    private int id;
    private String code;
    private String nom;
    private double prix;
    private Etat etat;

    @Override
    public String toString() {
        return String.format("%03d | %-7s | %-20s | %8.2f | %s",
                id, code, nom, prix, etat);
    }
}