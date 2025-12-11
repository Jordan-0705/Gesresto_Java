package ges.resto.factory.repository;

import ges.resto.database.Database;
import ges.resto.factory.database.DatabaseFactory;
import ges.resto.repository.impl.BurgerRepositoryImpl;
import ges.resto.repository.impl.ComplementRepositoryImpl;
import ges.resto.repository.impl.MenuRepositoryImpl;

public final class RepositoryFactory {

    private static final PersistanceName PERSISTENCE_NAME = PersistanceName.Database;

    private RepositoryFactory() {
    }

    public static Object getInstance(EntityName entityName) {
        switch (PERSISTENCE_NAME) {
            case Database:
                return getRepositoryDatabase(entityName);
            // case Memory:
            //     return getRepositoryMemory(entityName);
            default:
                throw new IllegalArgumentException("Unknown Persistence: " + PERSISTENCE_NAME);
        }
    }

    private static Object getRepositoryDatabase(EntityName entityName) {
        Database db = DatabaseFactory.getInstance();
        switch (entityName) {
            case Burger:
                return BurgerRepositoryImpl.getInstance(db);
            case Complement:
                return ComplementRepositoryImpl.getInstance(db);
            case Menu:
                return MenuRepositoryImpl.getInstance(db);
            // case Gestionnaire:
            //     return GestionnaireRepositoryImpl.getInstance(db);
            default:
                throw new IllegalArgumentException("Unknown Entity: " + entityName);
        }
    }

    // private static Object getRepositoryMemory(EntityName entityName) {
    //     switch (entityName) {
    //         case Burger:
    //             return new com.gesresto.repository.impl.list.BurgerRepositoryImpl();
    //         case Complement:
    //             return new com.gesresto.repository.impl.list.ComplementRepositoryImpl();
    //         case Menu:
    //             return new com.gesresto.repository.impl.list.MenuRepositoryImpl();
    //         case Gestionnaire:
    //             return new com.gesresto.repository.impl.list.GestionnaireRepositoryImpl();
    //         default:
    //             throw new IllegalArgumentException("Unknown Entity: " + entityName);
    //     }
    // }
}

