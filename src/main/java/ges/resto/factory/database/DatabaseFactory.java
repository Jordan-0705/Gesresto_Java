package ges.resto.factory.database;

import ges.resto.database.Database;
import ges.resto.database.DatabaseImpl;

public final class DatabaseFactory {

    private static final SgbdName sgbdName = SgbdName.POSTGRESQL;

    private DatabaseFactory() {
    }

    public static Database getInstance() {
        return DatabaseImpl.getIsntance(EntityManager.PersistenceUnit(sgbdName));
    }
}
