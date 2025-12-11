package ges.resto;

import ges.resto.database.DatabaseImpl;
import ges.resto.factory.repository.EntityName;
import ges.resto.factory.service.ServiceFactory;
import ges.resto.repository.BurgerRepository;
import ges.resto.repository.impl.BurgerRepositoryImpl;
import ges.resto.service.BurgerService;
import ges.resto.service.ComplementService;
import ges.resto.service.impl.BurgerServiceImpl;
import ges.resto.view.BurgerView;
import ges.resto.view.ComplementView;

public class Main {
    public static void main(String[] args) {

        
        BurgerService burgerService = (BurgerService)ServiceFactory.getInstance(EntityName.Burger);
        ComplementService complementService = (ComplementService)ServiceFactory.getInstance(EntityName.Complement);

        BurgerView bView = BurgerView.getInstance(burgerService);
        ComplementView cView = ComplementView.getInstance(complementService);


        cView.showMenu();
    }
}
