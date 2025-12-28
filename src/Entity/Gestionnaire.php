<?php

namespace App\Entity;

use App\Repository\GestionnaireRepository;
use Doctrine\ORM\Mapping as ORM;
use Symfony\Bridge\Doctrine\Validator\Constraints\UniqueEntity;
use Symfony\Component\Security\Core\User\PasswordAuthenticatedUserInterface;
use Symfony\Component\Security\Core\User\UserInterface;
use Symfony\Component\Validator\Constraints as Assert;

#[ORM\Entity(repositoryClass: GestionnaireRepository::class)]
#[ORM\Table(name: 'gestionnaire')]
#[UniqueEntity(fields: ['login'], message: '{{ value }} existe déjà.')]
class Gestionnaire implements UserInterface, PasswordAuthenticatedUserInterface
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[Assert\NotBlank]
    #[ORM\Column(length: 50, unique: true)]
    private ?string $code = null;

    #[Assert\NotBlank]
    #[Assert\Length(min: 3, max: 100)]
    #[ORM\Column(length: 100)]
    private ?string $nom = null;

    // AJOUTEZ CETTE PROPRIÉTÉ (elle existe dans votre table)
    #[ORM\Column(length: 100, nullable: true)] // nullable si la colonne peut être NULL
    private ?string $prenom = null;

    #[Assert\NotBlank]
    #[ORM\Column(length: 50, unique: true)]
    private ?string $login = null;

    #[Assert\NotBlank]
    #[ORM\Column(length: 100)]
    private ?string $password = null;

    // ================= GETTERS / SETTERS =================

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getCode(): ?string
    {
        return $this->code;
    }

    public function setCode(string $code): static
    {
        $this->code = $code;
        return $this;
    }

    public function getNom(): ?string
    {
        return $this->nom;
    }

    public function setNom(string $nom): static
    {
        $this->nom = $nom;
        return $this;
    }

    // AJOUTEZ CES MÉTHODES
    public function getPrenom(): ?string
    {
        return $this->prenom;
    }

    public function setPrenom(?string $prenom): static
    {
        $this->prenom = $prenom;
        return $this;
    }

    public function getLogin(): ?string
    {
        return $this->login;
    }

    public function setLogin(string $login): static
    {
        $this->login = $login;
        return $this;
    }

    public function getPassword(): ?string
    {
        return $this->password;
    }

    public function setPassword(string $password): static
    {
        $this->password = $password;
        return $this;
    }

    // ================= IMPLEMENTATION UserInterface =================

    public function getRoles(): array
    {
        return ['ROLE_GESTIONNAIRE'];
    }

    public function eraseCredentials(): void
    {
        // Si vous stockez des données temporaires sensibles, effacez-les ici
    }

    public function getUserIdentifier(): string
    {
        return $this->login;
    }

    public function getUsername(): string
    {
        return $this->getUserIdentifier();
    }
    
    // Méthode utile pour afficher le nom complet
    public function getNomComplet(): string
    {
        return trim($this->nom . ' ' . ($this->prenom ?? ''));
    }
}