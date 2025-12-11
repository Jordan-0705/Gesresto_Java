package ges.resto.factory.service;

import ges.resto.factory.repository.EntityName;
import ges.resto.factory.repository.RepositoryFactory;
import ges.resto.repository.BurgerRepository;
import ges.resto.repository.ComplementRepository;
import ges.resto.repository.MenuRepository;
// import ges.resto.repository.GestionnaireRepository;
import ges.resto.service.impl.BurgerServiceImpl;
import ges.resto.service.impl.ComplementServiceImpl;
import ges.resto.service.impl.MenuServiceImpl;
// import ges.resto.service.impl.GestionnaireServiceImpl;

public final class ServiceFactory {

    private ServiceFactory() {
    }

    public static Object getInstance(EntityName entityName) {
        switch (entityName) {
            case Burger:
                return BurgerServiceImpl.getInstance(
                        (BurgerRepository) RepositoryFactory.getInstance(EntityName.Burger)
                );
            case Complement:
                return ComplementServiceImpl.getInstance(
                        (ComplementRepository) RepositoryFactory.getInstance(EntityName.Complement)
                );
            case Menu:
                return MenuServiceImpl.getInstance(
                        (MenuRepository) RepositoryFactory.getInstance(EntityName.Menu)
                );
            // case Gestionnaire:
            //     return GestionnaireServiceImpl.getInstance(
            //             (GestionnaireRepository) RepositoryFactory.getInstance(EntityName.Gestionnaire)
            //    );
            default:
                throw new IllegalArgumentException("Unknown Entity: " + entityName);
        }
    }
}

