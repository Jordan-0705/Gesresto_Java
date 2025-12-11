package ges.resto.repository;

import ges.resto.entity.Menu;
import java.util.List;
import java.util.Optional;

public interface MenuRepository {

    int insert(Menu menu);
    int update(Menu menu);
    int delete(int id);

    List<Menu> selectAll();
    Optional<Menu> selectById(int id);
}
