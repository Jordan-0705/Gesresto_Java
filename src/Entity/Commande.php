<?php

namespace App\Entity;

use App\Repository\CommandeRepository;
use Doctrine\Common\Collections\ArrayCollection;
use Doctrine\Common\Collections\Collection;
use Doctrine\ORM\Mapping as ORM;
use Symfony\Component\Validator\Constraints as Assert;

#[ORM\Entity(repositoryClass: CommandeRepository::class)]
#[ORM\Table(name: 'commande')]
class Commande
{
    #[ORM\Id]
    #[ORM\GeneratedValue]
    #[ORM\Column]
    private ?int $id = null;

    #[Assert\NotBlank]
    #[ORM\Column(length: 50, unique: true)]
    private ?string $code = null;

    #[Assert\NotBlank]
    #[ORM\Column(type: 'decimal', precision: 10, scale: 2)]
    private ?string $prix = null;

    #[ORM\Column]
    private ?\DateTimeImmutable $date = null;

    #[ORM\Column(name: 'client_id')]
    private ?int $clientId = null;

    #[ORM\ManyToOne]
    #[ORM\JoinColumn(name: 'livreur_id', nullable: true)]
    private ?Livreur $livreur = null;

    #[ORM\ManyToOne(inversedBy: 'commandes')]
    #[ORM\JoinColumn(name: 'zone_id', nullable: true)]
    private ?Zone $zone = null;

    #[Assert\NotBlank]
    #[ORM\Column(name: 'etatCommande', length: 20)]
    private ?string $etatCommande = null;

    #[Assert\NotBlank]
    #[ORM\Column(name: 'commandeType', length: 20)]
    private ?string $commandeType = null;

    #[Assert\NotBlank]
    #[ORM\Column(name: 'consoType', length: 20)]
    private ?string $consoType = null;

    /**
     * @var Collection<int, CommandeBurger>
     */
    #[ORM\OneToMany(mappedBy: 'commande', targetEntity: CommandeBurger::class, cascade: ['persist', 'remove'], orphanRemoval: true)]
    private Collection $commandeBurgers;

    /**
     * @var Collection<int, CommandeMenu>
     */
    #[ORM\OneToMany(mappedBy: 'commande', targetEntity: CommandeMenu::class, cascade: ['persist', 'remove'], orphanRemoval: true)]
    private Collection $commandeMenus;

    /**
     * @var Collection<int, Paiement>
     */
    #[ORM\OneToMany(mappedBy: 'commande', targetEntity: Paiement::class, cascade: ['persist', 'remove'], orphanRemoval: true)]
    private Collection $paiements;

    #[ORM\ManyToOne(inversedBy: 'commandes')]
    #[ORM\JoinColumn(name: 'livraison_id', nullable: true)]
    private ?Livraison $livraison = null;

    public function __construct()
    {
        $this->date = new \DateTimeImmutable();
        $this->commandeBurgers = new ArrayCollection();
        $this->commandeMenus = new ArrayCollection();
        $this->paiements = new ArrayCollection();
    }

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

    public function getPrix(): ?string
    {
        return $this->prix;
    }

    public function setPrix(string $prix): static
    {
        $this->prix = $prix;
        return $this;
    }

    public function getDate(): ?\DateTimeImmutable
    {
        return $this->date;
    }

    public function setDate(\DateTimeImmutable $date): static
    {
        $this->date = $date;
        return $this;
    }

    public function getClientId(): ?int
    {
        return $this->clientId;
    }

    public function setClientId(int $clientId): static
    {
        $this->clientId = $clientId;
        return $this;
    }

    public function getLivreur(): ?Livreur
    {
        return $this->livreur;
    }

    public function setLivreur(?Livreur $livreur): static
    {
        $this->livreur = $livreur;
        return $this;
    }

    public function getZone(): ?Zone
    {
        return $this->zone;
    }

    public function setZone(?Zone $zone): static
    {
        $this->zone = $zone;
        return $this;
    }

    public function getEtatCommande(): ?string
    {
        return $this->etatCommande;
    }

    public function setEtatCommande(string $etatCommande): static
    {
        $this->etatCommande = $etatCommande;
        return $this;
    }

    public function getCommandeType(): ?string
    {
        return $this->commandeType;
    }

    public function setCommandeType(string $commandeType): static
    {
        $this->commandeType = $commandeType;
        return $this;
    }

    public function getConsoType(): ?string
    {
        return $this->consoType;
    }

    public function setConsoType(string $consoType): static
    {
        $this->consoType = $consoType;
        return $this;
    }

    /**
     * @return Collection<int, CommandeBurger>
     */
    public function getCommandeBurgers(): Collection
    {
        return $this->commandeBurgers;
    }

    public function addCommandeBurger(CommandeBurger $commandeBurger): static
    {
        if (!$this->commandeBurgers->contains($commandeBurger)) {
            $this->commandeBurgers->add($commandeBurger);
            $commandeBurger->setCommande($this);
        }

        return $this;
    }

    public function removeCommandeBurger(CommandeBurger $commandeBurger): static
    {
        if ($this->commandeBurgers->removeElement($commandeBurger)) {
            if ($commandeBurger->getCommande() === $this) {
                $commandeBurger->setCommande(null);
            }
        }

        return $this;
    }

    /**
     * @return Collection<int, CommandeMenu>
     */
    public function getCommandeMenus(): Collection
    {
        return $this->commandeMenus;
    }

    public function addCommandeMenu(CommandeMenu $commandeMenu): static
    {
        if (!$this->commandeMenus->contains($commandeMenu)) {
            $this->commandeMenus->add($commandeMenu);
            $commandeMenu->setCommande($this);
        }

        return $this;
    }

    public function removeCommandeMenu(CommandeMenu $commandeMenu): static
    {
        if ($this->commandeMenus->removeElement($commandeMenu)) {
            if ($commandeMenu->getCommande() === $this) {
                $commandeMenu->setCommande(null);
            }
        }

        return $this;
    }

    /**
     * @return Collection<int, Paiement>
     */
    public function getPaiements(): Collection
    {
        return $this->paiements;
    }

    public function addPaiement(Paiement $paiement): static
    {
        if (!$this->paiements->contains($paiement)) {
            $this->paiements->add($paiement);
            $paiement->setCommande($this);
        }

        return $this;
    }

    public function removePaiement(Paiement $paiement): static
    {
        if ($this->paiements->removeElement($paiement)) {
            if ($paiement->getCommande() === $this) {
                $paiement->setCommande(null);
            }
        }

        return $this;
    }

    public function getLivraison(): ?Livraison
    {
        return $this->livraison;
    }

    public function setLivraison(?Livraison $livraison): static
    {
        $this->livraison = $livraison;
        return $this;
    }

    // ================= MÉTHODES UTILITAIRES =================

    public function getTotal(): float
    {
        $total = 0;

        foreach ($this->commandeBurgers as $commandeBurger) {
            $total += (float) $commandeBurger->getBurger()->getPrix() * $commandeBurger->getQuantite();
            if ($commandeBurger->getComplementF()) {
                $total += (float) $commandeBurger->getComplementF()->getPrix() * $commandeBurger->getQuantite();
            }
            if ($commandeBurger->getComplementB()) {
                $total += (float) $commandeBurger->getComplementB()->getPrix() * $commandeBurger->getQuantite();
            }
        }

        foreach ($this->commandeMenus as $commandeMenu) {
            $total += (float) $commandeMenu->getMenu()->getPrix() * $commandeMenu->getQuantite();
        }

        return $total;
    }

    public function getNombreArticles(): int
    {
        $total = 0;

        foreach ($this->commandeBurgers as $commandeBurger) {
            $total += $commandeBurger->getQuantite();
        }

        foreach ($this->commandeMenus as $commandeMenu) {
            $total += $commandeMenu->getQuantite();
        }

        return $total;
    }

    public function hasPaiement(): bool
    {
        return !$this->paiements->isEmpty();
    }

    public function getLastPaiement(): ?Paiement
    {
        return $this->paiements->last() ?: null;
    }
}