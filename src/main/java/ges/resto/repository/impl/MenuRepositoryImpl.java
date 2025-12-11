package ges.resto.repository.impl;

import ges.resto.database.Database;
import ges.resto.entity.Burger;
import ges.resto.entity.Complement;
import ges.resto.entity.Etat;
import ges.resto.entity.Menu;
import ges.resto.repository.MenuRepository;

import java.sql.*;
import java.util.*;

public class MenuRepositoryImpl implements MenuRepository {

    private static MenuRepositoryImpl instance = null;

    public static MenuRepositoryImpl getInstance(Database database) {
        if (instance == null) {
            instance = new MenuRepositoryImpl(database);
        }
        return instance;
    }

    private Database database;

    private MenuRepositoryImpl(Database db) {
        this.database = db;
    }

    @Override
    public int insert(Menu menu) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "INSERT INTO menu(code, nom, burger_id, frites_id, boisson_id, prix, etat) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?)"
            );

            ps.setString(1, menu.getCode());
            ps.setString(2, menu.getNom());
            ps.setInt(3, menu.getBurger().getId());
            ps.setInt(4, menu.getFrites().getId());
            ps.setInt(5, menu.getBoisson().getId());
            ps.setDouble(6, menu.getPrix());
            ps.setString(7, menu.getEtat().name());

            return ps.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public int update(Menu menu) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "UPDATE menu SET nom=?, burger_id=?, frites_id=?, boisson_id=?, prix=?, etat=? WHERE id=?"
            );

            ps.setString(1, menu.getNom());
            ps.setInt(2, menu.getBurger().getId());
            ps.setInt(3, menu.getFrites().getId());
            ps.setInt(4, menu.getBoisson().getId());
            ps.setDouble(5, menu.getPrix());
            ps.setString(6, menu.getEtat().name());
            ps.setInt(7, menu.getId());

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
                "DELETE FROM menu WHERE id=?"
            );
            ps.setInt(1, id);

            return ps.executeUpdate();
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public List<Menu> selectAll() {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement("SELECT * FROM menu");

            return database.fetchAll(ps, this::toEntity);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return Collections.emptyList();
    }

    @Override
    public Optional<Menu> selectById(int id) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "SELECT * FROM menu WHERE id=?"
            );
            ps.setInt(1, id);

            return database.fetch(ps, this::toEntity);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return Optional.empty();
    }

    private Menu toEntity(ResultSet rs) throws SQLException {
        Menu m = new Menu();

        m.setId(rs.getInt("id"));
        m.setCode(rs.getString("code"));
        m.setNom(rs.getString("nom"));
        m.setPrix(rs.getDouble("prix"));

        // Etat (enum)
        m.setEtat(
            rs.getString("etat").equals("Disponible")
                ? Etat.Disponible
                : Etat.Archived
        );

        // Burger minimal (ID seulement)
        Burger b = new Burger();
        b.setId(rs.getInt("burger_id"));
        m.setBurger(b);

        // Frites
        Complement f = new Complement();
        f.setId(rs.getInt("frites_id"));
        m.setFrites(f);

        // Boisson
        Complement bo = new Complement();
        bo.setId(rs.getInt("boisson_id"));
        m.setBoisson(bo);

        return m;
    }

}
