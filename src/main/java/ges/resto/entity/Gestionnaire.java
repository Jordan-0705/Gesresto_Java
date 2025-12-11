package ges.resto.entity;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
@AllArgsConstructor
@NoArgsConstructor

public class Gestionnaire {
    private int id;
    private String code;
    private String nom;
    private String prenom;
    private String login;
    private String password;
}
