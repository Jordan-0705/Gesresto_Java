package ges.resto.database;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Optional;

public class DatabaseImpl implements Database {

    private static DatabaseImpl instance = null;

    public static DatabaseImpl getIsntance(Map<String, String> config) {
        if (instance == null) {
            instance = new DatabaseImpl(config);
        }
        return instance;
    }

    public static DatabaseImpl getIsntance(String driver, String url, String user, String password) {
        if (instance == null) {
            instance = new DatabaseImpl(driver, url, user, password);
        }
        return instance;
    }

    private Connection connection;

    private DatabaseImpl(Map<String, String> config) {
        String driver = config.get("driver");
        String url = config.get("url");
        String user = config.get("user");
        String password = config.get("password");
        connection = openConnection(driver, url, user, password);
        
    }

    private DatabaseImpl(String driver, String url, String user, String password) {
        connection = openConnection(driver, url, user, password);
    }

    public Connection openConnection(String driver, String url, String user, String password) {
        try {
            Class.forName(driver);
            return DriverManager.getConnection(url, user, password);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (Throwable e) {
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public Connection getConnection() {
        return connection;
    }

    @Override
    public boolean isConnected() {  
        return connection != null;
    }

    @Override
    public void closeConnection() {
        if (connection != null) {
            try {
                connection.close();
            } catch (Throwable e) {
                e.printStackTrace();
            }
        }
    }   

    @Override
    public <T> List<T> fetchAll(PreparedStatement ps, Convert<T> convert) throws java.sql.SQLException {
        ResultSet rs = ps.executeQuery();
        List<T> datas = new ArrayList<>();
        while (rs.next()) {
            datas.add(convert.toEntity(rs));
        }
        return datas;
    }

    @Override
    public <T> Optional<T> fetch(PreparedStatement ps, Convert<T> convert) throws java.sql.SQLException {
        ResultSet rs = ps.executeQuery();
        T data = null;
        if (rs.next()) {
            data = convert.toEntity(rs);
        }
        return Optional.of((T) data);
    }

    @Override
    public void testConnection() {
        Connection conn = getConnection();    // ✔ on NE le met PAS dans un try-with-resources !
        if (conn != null) {
            System.out.println("Connexion OK !");
        } else {
            System.out.println("Connexion échouée !");
        }
    }

}