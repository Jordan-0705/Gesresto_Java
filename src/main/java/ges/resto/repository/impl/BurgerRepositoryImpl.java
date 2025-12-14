package ges.resto.repository.impl;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Collections;
import java.util.List;
import java.util.Optional;

import ges.resto.database.Database;
import ges.resto.entity.Burger;
import ges.resto.entity.Etat;
import ges.resto.repository.BurgerRepository;

public class BurgerRepositoryImpl implements BurgerRepository {

    private static BurgerRepositoryImpl instance = null;

    public static BurgerRepositoryImpl getInstance(Database database) {
        if (instance == null) {
            instance = new BurgerRepositoryImpl(database);
        }
        return instance;
    }

    private Database database;

    private BurgerRepositoryImpl(Database database) {
        this.database = database;
    }

    @Override
    public int insert(Burger burger) {
        try {
            Connection conn = database.getConnection();

            PreparedStatement ps = conn.prepareStatement(
                "INSERT INTO burger(code, nom, prix, etat) VALUES (?, ?, ?, ?)"
            );

            ps.setString(1, burger.getCode());
            ps.setString(2, burger.getNom());
            ps.setDouble(3, burger.getPrix());
            ps.setString(4, burger.getEtat().name());

            return ps.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public int update(Burger burger) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "UPDATE burger SET code=?, nom=?, prix=?, etat=? WHERE id=?"
            );

            ps.setString(1, burger.getCode());
            ps.setString(2, burger.getNom());
            ps.setDouble(3, burger.getPrix());
            ps.setString(4, burger.getEtat().name());
            ps.setInt(5, burger.getId());

            return ps.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public int delete(int id) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "DELETE FROM burger WHERE id=?"
            );

            ps.setInt(1, id);

            return ps.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public List<Burger> selectAll() {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement("SELECT * FROM burger ORDER BY id ASC");

            return database.<Burger>fetchAll(ps, this::toEntity);
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return Collections.emptyList();
    }

    @Override
    public Optional<Burger> selectById(int id) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "SELECT * FROM burger WHERE id=?"
            );

            ps.setInt(1, id);

           
            return database.<Burger>fetch(ps, this::toEntity);
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return Optional.empty();
    }


    private Burger toEntity(ResultSet rs) throws SQLException {
        Burger burger = new Burger();
        burger.setId(rs.getInt("id"));
        burger.setCode(rs.getString("code"));
        burger.setNom(rs.getString("nom"));
        burger.setPrix(rs.getDouble("prix"));

        String etatStr = rs.getString("etat");

        if (etatStr != null) {
            try {
                burger.setEtat(Etat.valueOf(etatStr));
            } catch (IllegalArgumentException e) {
                
                burger.setEtat(Etat.Disponible);
            }
        } else {
            burger.setEtat(Etat.Disponible);
        }

        return burger;
    }

}
