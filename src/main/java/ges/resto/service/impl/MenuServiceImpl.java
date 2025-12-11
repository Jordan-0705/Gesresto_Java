package ges.resto.service.impl;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Menu;
import ges.resto.repository.MenuRepository;
import ges.resto.service.MenuService;

public class MenuServiceImpl implements MenuService {

    private static MenuServiceImpl instance = null;

    public static MenuServiceImpl getInstance(MenuRepository repo) {
        if (instance == null) {
            instance = new MenuServiceImpl(repo);
        }
        return instance;
    }

    private MenuRepository menuRepository;

    private MenuServiceImpl(MenuRepository menuRepository) {
        this.menuRepository = menuRepository;
    }

    @Override
    public int addMenu(Menu menu) {
        String newCode = generateMenuCode();
        menu.setCode(newCode);
        return menuRepository.insert(menu);
    }

    @Override
    public int updateMenu(Menu menu) {
        return menuRepository.update(menu);
    }

    @Override
    public int deleteMenu(int id) {
        return menuRepository.delete(id);
    }

    @Override
    public List<Menu> findAllMenu() {
        return menuRepository.selectAll();
    }

    @Override
    public Optional<Menu> findById(int id) {
        return menuRepository.selectById(id);
    }

    private String generateMenuCode() {
        List<Menu> menus = menuRepository.selectAll();

        if (menus.isEmpty()) {
            return "MEN-01";
        }

        int max = menus.stream()
                .map(Menu::getCode)
                .map(code -> code.replace("MEN-", ""))
                .mapToInt(Integer::parseInt)
                .max().orElse(0);

        return String.format("MEN-%02d", max + 1);
    }
}
