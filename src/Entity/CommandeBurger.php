<?php

namespace App\Entity;

use App\Repository\CommandeBurgerRepository;
use Doctrine\ORM\Mapping as ORM;

#[ORM\Entity(repositoryClass: CommandeBurgerRepository::class)]
#[ORM\Table(name: 'commandeburger')]
class CommandeBurger
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[ORM\ManyToOne(targetEntity: Commande::class, inversedBy: 'commandeBurgers')]
    #[ORM\JoinColumn(name: 'commande_id', referencedColumnName: 'id', nullable: false, onDelete: 'CASCADE')]
    private ?Commande $commande = null;

    #[ORM\ManyToOne(targetEntity: Burger::class)]
    #[ORM\JoinColumn(name: 'burger_id', referencedColumnName: 'id', nullable: false)]
    private ?Burger $burger = null;

    #[ORM\ManyToOne(targetEntity: Complement::class)]
    #[ORM\JoinColumn(name: 'complementf_id', referencedColumnName: 'id')]
    private ?Complement $complementF = null;

    #[ORM\ManyToOne(targetEntity: Complement::class)]
    #[ORM\JoinColumn(name: 'complementb_id', referencedColumnName: 'id')]
    private ?Complement $complementB = null;

    #[ORM\Column]
    private int $quantite = 1;

    // ================= GETTERS / SETTERS =================

    public function getId(): ?int
    {
        return $this->id;
    }

    public function getCommande(): ?Commande
    {
        return $this->commande;
    }

    public function setCommande(?Commande $commande): static
    {
        $this->commande = $commande;
        return $this;
    }

    public function getBurger(): ?Burger
    {
        return $this->burger;
    }

    public function setBurger(?Burger $burger): static
    {
        $this->burger = $burger;
        return $this;
    }

    public function getComplementF(): ?Complement
    {
        return $this->complementF;
    }

    public function setComplementF(?Complement $complementF): static
    {
        $this->complementF = $complementF;
        return $this;
    }

    public function getComplementB(): ?Complement
    {
        return $this->complementB;
    }

    public function setComplementB(?Complement $complementB): static
    {
        $this->complementB = $complementB;
        return $this;
    }

    public function getQuantite(): int
    {
        return $this->quantite;
    }

    public function setQuantite(int $quantite): static
    {
        $this->quantite = $quantite;
        return $this;
    }
}