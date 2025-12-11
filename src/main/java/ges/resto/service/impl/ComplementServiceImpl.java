package ges.resto.service.impl;

import java.util.List;
import java.util.Optional;

import ges.resto.entity.Complement;
import ges.resto.repository.ComplementRepository;
import ges.resto.service.ComplementService;

public class ComplementServiceImpl implements ComplementService {

    private static ComplementServiceImpl instance = null;

    public static ComplementServiceImpl getInstance(ComplementRepository repo) {
        if (instance == null) {
            instance = new ComplementServiceImpl(repo);
        }
        return instance;
    }

    private ComplementRepository complementRepository;

    private ComplementServiceImpl(ComplementRepository complementRepository) {
        this.complementRepository = complementRepository;
    }

    @Override
    public int addComplement(Complement complement) {
        String newCode = generateComplementCode();
        complement.setCode(newCode);
        return complementRepository.insert(complement);
    }

    @Override
    public int updateComplement(Complement complement) {
        return complementRepository.update(complement);
    }

    @Override
    public int deleteComplement(int id) {
        return complementRepository.delete(id);
    }

    @Override
    public List<Complement> findAllComplement() {
        return complementRepository.selectAll();
    }

    @Override
    public Optional<Complement> findById(int id) {
        return complementRepository.selectById(id);
    }

    // Génération du code CMP-01, CMP-02...
    private String generateComplementCode() {
        List<Complement> complements = complementRepository.selectAll();

        // Si aucun complement → CMP-01
        if (complements.isEmpty()) {
            return "CMP-01";
        }

        // Trouver le n° max
        int max = complements.stream()
                .map(Complement::getCode)
                .map(code -> code.replace("CMP-", "")) // CMP-08 → 08
                .mapToInt(Integer::parseInt)
                .max()
                .orElse(0);

        int next = max + 1;

        return String.format("CMP-%02d", next);
    }
}
