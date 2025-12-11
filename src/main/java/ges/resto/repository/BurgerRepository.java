package ges.resto.repository;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Burger;

public interface BurgerRepository {
    int insert (Burger burger);
    int update (Burger burger);
    int delete (int id);
    List<Burger> selectAll();
    Optional<Burger> selectById(int id);
} 