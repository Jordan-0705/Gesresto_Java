package ges.resto.repository;

import ges.resto.entity.Complement;
import java.util.List;
import java.util.Optional;

public interface ComplementRepository {
    int insert(Complement complement);
    int update(Complement complement);
    int delete(int id);

    List<Complement> selectAll();
    Optional<Complement> selectById(int id);
    Optional<Complement> selectByCode(String code);  // utile pour valider unicité
}
