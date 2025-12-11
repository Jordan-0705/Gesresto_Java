package ges.resto.service;

import ges.resto.entity.Menu;
import java.util.List;
import java.util.Optional;

public interface MenuService {

    int addMenu(Menu menu);
    int updateMenu(Menu menu);
    int deleteMenu(int id);

    List<Menu> findAllMenu();
    Optional<Menu> findById(int id);
}
