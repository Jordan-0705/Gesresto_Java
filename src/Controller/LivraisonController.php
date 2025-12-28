<?php

namespace App\Controller;

use App\Entity\Livraison;
use App\Entity\Commande;
use App\Entity\Zone;
use App\Entity\Livreur;
use App\Repository\LivraisonRepository;
use App\Repository\CommandeRepository;
use App\Repository\ZoneRepository;
use App\Repository\LivreurRepository;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class LivraisonController extends AbstractController
{
    #[Route('/livraisons', name: 'app_livraisons_index')]
    public function index(LivraisonRepository $livraisonRepository): Response
    {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        $livraisons = $livraisonRepository->findAllOrderedByDate();

        return $this->render('livraison/index.html.twig', [
            'livraisons' => $livraisons,
        ]);
    }

    #[Route('/livraisons/affecter', name: 'app_livraisons_affecter')]
    public function affecter(
        ZoneRepository $zoneRepository,
        LivreurRepository $livreurRepository,
        CommandeRepository $commandeRepository
    ): Response {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        // Récupérer les zones et livreurs
        $zones = $zoneRepository->findAll();
        $livreurs = $livreurRepository->findAll();

        // Prix par zone
        $prixParZone = [
            'Zone 1' => '500',
            'Zone 2' => '1000',
            'Zone 3' => '1500',
            'Zone 4' => '2000',
            'Zone 5' => '2500',
        ];

        // Préparer les zones avec leurs prix
        $zonesAvecPrix = [];
        foreach ($zones as $zone) {
            $prix = '1000'; // Par défaut
            foreach ($prixParZone as $nomZone => $prixZone) {
                if (strpos($zone->getNom(), $nomZone) !== false) {
                    $prix = $prixZone;
                    break;
                }
            }
            
            $zonesAvecPrix[] = [
                'zone' => $zone,
                'prix_livraison' => $prix,
            ];
        }

        // Récupérer les commandes par zone
        $commandesParZone = [];
        foreach ($zones as $zone) {
            $commandes = $commandeRepository->findCommandesALivrerParZone($zone->getId());
            if (count($commandes) > 0) {
                // Déterminer le prix pour cette zone
                $prixLivraison = '1000';
                foreach ($prixParZone as $nomZone => $prix) {
                    if (strpos($zone->getNom(), $nomZone) !== false) {
                        $prixLivraison = $prix;
                        break;
                    }
                }
                
                $commandesParZone[] = [
                    'zone' => $zone,
                    'commandes' => $commandes,
                    'count' => count($commandes),
                    'total_commandes' => array_sum(array_map(fn($c) => (float) $c->getPrix(), $commandes)),
                    'prix_livraison' => $prixLivraison,
                    'total_general' => array_sum(array_map(fn($c) => (float) $c->getPrix(), $commandes)) + (float) $prixLivraison,
                ];
            }
        }

        return $this->render('livraison/affecter.html.twig', [
            'zones' => $zones,
            'livreurs' => $livreurs,
            'commandesParZone' => $commandesParZone,
            'prixParZone' => $prixParZone, // Passer les prix au template
        ]);
    }

    #[Route('/livraisons/creer', name: 'app_livraisons_creer', methods: ['POST'])]
    public function creer(
        Request $request,
        EntityManagerInterface $entityManager,
        ZoneRepository $zoneRepository,
        LivreurRepository $livreurRepository,
        CommandeRepository $commandeRepository
    ): Response
    {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        $zoneId = $request->request->get('zone_id');
        $livreurId = $request->request->get('livreur_id');

        $zone = $zoneRepository->find($zoneId);
        $livreur = $livreurRepository->find($livreurId);

        if (!$zone || !$livreur) {
            $this->addFlash('error', 'Zone ou livreur invalide');
            return $this->redirectToRoute('app_livraisons_affecter');
        }

        // Récupérer les commandes à livrer pour cette zone
        $commandes = $commandeRepository->findCommandesALivrerParZone($zoneId);

        if (count($commandes) === 0) {
            $this->addFlash('warning', 'Aucune commande à livrer pour cette zone');
            return $this->redirectToRoute('app_livraisons_affecter');
        }

        // === ÉTAPE 1: Créer la livraison avec un code temporaire ===
        $livraison = new Livraison();
        
        // Code temporaire en attendant l'ID
        $tempCode = 'LIV-TEMP-' . date('His');
        $livraison->setCode($tempCode);
        
        $livraison->setZone($zone);
        $livraison->setLivreur($livreur);
        
        // Calculer le prix de la livraison selon la zone
        $prixLivraison = $this->getPrixLivraisonParZone($zone->getNom());
        $livraison->setPrix($prixLivraison);

        // Définir la première commande comme référence
        if (count($commandes) > 0) {
            $livraison->setCommande($commandes[0]);
        }

        // Sauvegarder pour obtenir l'ID
        $entityManager->persist($livraison);
        $entityManager->flush();

        // === ÉTAPE 2: Maintenant qu'on a l'ID, générer le vrai code ===
        $vraiCode = 'LIV-' . str_pad($livraison->getId(), 2, '0', STR_PAD_LEFT);
        $livraison->setCode($vraiCode);

        // === ÉTAPE 3: Affecter les commandes ===
        $totalCommandes = 0;
        foreach ($commandes as $commande) {
            // Affecter le livreur à la commande
            $commande->setLivreur($livreur);
            $commande->setLivraison($livraison);
            
            // Ajouter le prix de la commande au total
            $totalCommandes += (float) $commande->getPrix();
            
            $entityManager->persist($commande);
        }

        // === ÉTAPE 4: Sauvegarder les modifications ===
        $entityManager->flush();

        $this->addFlash('success', sprintf(
            'Livraison %s créée avec succès ! %s commande(s) affectée(s) au livreur %s pour la zone %s.',
            $livraison->getCode(),
            count($commandes),
            $livreur->getNom(),
            $zone->getNom()
        ));

        return $this->redirectToRoute('app_livraisons_index');
    }

    #[Route('/livraisons/{id}', name: 'app_livraisons_show')]
    public function show(Livraison $livraison, CommandeRepository $commandeRepository): Response
    {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        // Récupérer toutes les commandes de cette livraison (par zone et livreur à la même date)
        $commandes = $commandeRepository->findBy([
            'zone' => $livraison->getZone(),
            'livreur' => $livraison->getLivreur(),
            // On pourrait aussi filtrer par date si nécessaire
        ]);

        $totalCommandes = array_sum(array_map(fn($c) => (float) $c->getPrix(), $commandes));
        $totalGeneral = $totalCommandes + (float) $livraison->getPrix();

        return $this->render('livraison/show.html.twig', [
            'livraison' => $livraison,
            'commandes' => $commandes,
            'total_commandes' => $totalCommandes,
            'total_general' => $totalGeneral,
        ]);
    }

    /**
     * Détermine le prix de livraison selon la zone
     */
    private function getPrixLivraisonParZone(string $nomZone): string
    {
        $prixParZone = [
            'Zone 1' => '500',
            'Zone 2' => '1000',
            'Zone 3' => '1500',
            'Zone 4' => '2000',
            'Zone 5' => '2500',
        ];

        // Chercher par nom exact
        if (isset($prixParZone[$nomZone])) {
            return $prixParZone[$nomZone];
        }

        // Chercher par pattern (si le nom contient "Zone 1", etc.)
        foreach ($prixParZone as $zone => $prix) {
            if (strpos($nomZone, $zone) !== false) {
                return $prix;
            }
        }

        // Par défaut
        return '1000';
    }
}