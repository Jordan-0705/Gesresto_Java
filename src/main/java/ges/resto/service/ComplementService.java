package ges.resto.service;

import java.util.List;
import java.util.Optional;
import ges.resto.entity.Complement;

public interface ComplementService {
    int addComplement(Complement complement);
    int updateComplement(Complement complement);
    int deleteComplement(int id);
    List<Complement> findAllComplement();
    Optional<Complement> findById(int id);
}

