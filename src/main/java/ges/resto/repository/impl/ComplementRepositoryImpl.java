package ges.resto.repository.impl;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Collections;
import java.util.List;
import java.util.Optional;

import ges.resto.database.Database;
import ges.resto.entity.Complement;
import ges.resto.entity.ComplementType;
import ges.resto.repository.ComplementRepository;

public class ComplementRepositoryImpl implements ComplementRepository {

    private static ComplementRepositoryImpl instance = null;

    public static ComplementRepositoryImpl getInstance(Database database) {
        if (instance == null) {
            instance = new ComplementRepositoryImpl(database);
        }
        return instance;
    }

    private Database database;

    private ComplementRepositoryImpl(Database database) {
        this.database = database;
    }

    @Override
    public int insert(Complement complement) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                    "INSERT INTO complement(code, nom, prix, complement_type) VALUES (?, ?, ?, ?)"
            );

            ps.setString(1, complement.getCode());
            ps.setString(2, complement.getNom());
            ps.setDouble(3, complement.getPrix());
            ps.setString(4, complement.getComplementType().name());   // enum → string

            return ps.executeUpdate();

        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public int update(Complement complement) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "UPDATE complement SET code=?, nom=?, prix=?, complement_type=? WHERE id=?"
            );

            ps.setString(1, complement.getCode());
            ps.setString(2, complement.getNom());
            ps.setDouble(3, complement.getPrix());
            ps.setString(4, complement.getComplementType().name());
            ps.setInt(5, complement.getId());

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
                "DELETE FROM complement WHERE id=?"
            );

            ps.setInt(1, id);

            return ps.executeUpdate();

        } catch (SQLException e) {
            e.printStackTrace();
        }
        return 0;
    }

    @Override
    public List<Complement> selectAll() {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "SELECT * FROM complement"
            );

            return database.<Complement>fetchAll(ps, this::toEntity);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return Collections.emptyList();
    }

    @Override
    public Optional<Complement> selectById(int id) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "SELECT * FROM complement WHERE id=?"
            );
            ps.setInt(1, id);

            return database.<Complement>fetch(ps, this::toEntity);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return Optional.empty();
    }

    @Override
    public Optional<Complement> selectByCode(String code) {
        try {
            Connection conn = database.getConnection();
            PreparedStatement ps = conn.prepareStatement(
                "SELECT * FROM complement WHERE code=?"
            );
            ps.setString(1, code);

            return database.<Complement>fetch(ps, this::toEntity);

        } catch (SQLException e) {
            e.printStackTrace();
        }

        return Optional.empty();
    }

    private Complement toEntity(ResultSet rs) throws SQLException {
        Complement c = new Complement();

        c.setId(rs.getInt("id"));
        c.setCode(rs.getString("code"));
        c.setNom(rs.getString("nom"));
        c.setPrix(rs.getDouble("prix"));

        c.setComplementType(ComplementType.valueOf(rs.getString("complement_type")));

        return c;
    }
}
