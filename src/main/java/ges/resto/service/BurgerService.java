package ges.resto.service;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Burger;

public interface BurgerService {

    int addBurger(Burger burger);
    int updateBurger(Burger burger);
    int deleteBurger(int id);
    List<Burger> findAllBurger();
    Optional<Burger> findById(int id);
}
