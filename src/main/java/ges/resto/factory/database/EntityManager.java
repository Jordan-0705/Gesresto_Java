package ges.resto.factory.database;

import java.util.HashMap;
import java.util.Map;

public final class EntityManager {

    private EntityManager() {
    }

    public static Map<String, String> PersistenceUnit(SgbdName sgbdName) {
        switch (sgbdName) {
            case MYSQL:
                return persistenceUnitMySql();
            case POSTGRESQL:
                return persistenceUnitPostgreSql();
            default:
                throw new IllegalArgumentException("Unknown SGBD: " + sgbdName);
        }
    }

    private static Map<String, String> persistenceUnitMySql(){
        Map<String, String> config = new HashMap<>();

        config.put("driver", "com.mysql.cj.jdbc.Driver");
        config.put("url", "jdbc:mysql://localhost:3306/gesresto");
        config.put("user", "root");
        config.put("password", "");

        return config;
    }

    private static Map<String, String> persistenceUnitPostgreSql() {
        Map<String, String> config = new HashMap<>();

        config.put("driver", "org.postgresql.Driver");
        config.put("url", "jdbc:postgresql://ep-small-rain-ahcxmlt5-pooler.c-3.us-east-1.aws.neon.tech:5432/neondb?sslmode=require&channelBinding=require");
        config.put("user", "neondb_owner");
        config.put("password", "npg_YBwMo1pc5tPd");
        // config.put("url", System.getenv("DB_URL"));
        // config.put("user", System.getenv("DB_USER"));
        // config.put("password", System.getenv("DB_PASSWORD"));

        return config;
    }
}

