package ges.resto.service.impl;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Burger;
import ges.resto.repository.BurgerRepository;
import ges.resto.service.BurgerService;

public class BurgerServiceImpl implements BurgerService {

    private static BurgerServiceImpl instance = null;

    public static BurgerServiceImpl getInstance(BurgerRepository burgerRepository) {
        if (instance == null) {
            instance = new BurgerServiceImpl(burgerRepository);
        }
        return instance;
    }

    private BurgerRepository burgerRepository;

    private BurgerServiceImpl(BurgerRepository burgerRepository) {
        this.burgerRepository = burgerRepository;
    }

    @Override
    public int addBurger(Burger burger) {
        // Générer le code BUR-XX automatiquement
        String code = generateBurgerCode();
        burger.setCode(code);

        return burgerRepository.insert(burger);
    }

    @Override
    public int updateBurger(Burger burger) {
        return burgerRepository.update(burger);
    }

    @Override
    public int deleteBurger(int id) {
        return burgerRepository.delete(id);
    }

    @Override
    public List<Burger> findAllBurger() {
        return burgerRepository.selectAll();
    }

    @Override
    public Optional<Burger> findById(int id) {
        return burgerRepository.selectById(id);
    }

    // Générer un code BUR-01, BUR-02, ...
    private String generateBurgerCode() {
        List<Burger> burgers = burgerRepository.selectAll();

        if (burgers.isEmpty()) {
            return "BUR-01";
        }

        // Récupérer le plus grand numéro existant
        int max = burgers.stream()
                .map(Burger::getCode)
                .map(code -> code.replace("BUR-", "")) // Ex: BUR-07 -> 07
                .mapToInt(Integer::parseInt)
                .max()
                .orElse(0);

        int next = max + 1;
        return String.format("BUR-%02d", next);
    }
}
