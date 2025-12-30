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

        $prixParZone = [
            'Zone 1' => '500',
            'Zone 2' => '1000',
            'Zone 3' => '1500',
            'Zone 4' => '2000',
            'Zone 5' => '2500',
        ];

        $zonesAvecPrix = [];
        foreach ($zones as $zone) {
            $prix = '1000';
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

        $commandesParZone = [];
        foreach ($zones as $zone) {
            $commandes = $commandeRepository->findCommandesALivrerParZone($zone->getId());
            if (count($commandes) > 0) {
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
            'prixParZone' => $prixParZone,
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

        $commandes = $commandeRepository->findCommandesALivrerParZone($zoneId);

        if (count($commandes) === 0) {
            $this->addFlash('warning', 'Aucune commande à livrer pour cette zone');
            return $this->redirectToRoute('app_livraisons_affecter');
        }

        $livraison = new Livraison();
        
        $tempCode = 'LIV-TEMP-' . date('His');
        $livraison->setCode($tempCode);
        
        $livraison->setZone($zone);
        $livraison->setLivreur($livreur);
        
        $prixLivraison = $this->getPrixLivraisonParZone($zone->getNom());
        $livraison->setPrix($prixLivraison);

        if (count($commandes) > 0) {
            $livraison->setCommande($commandes[0]);
        }

        $entityManager->persist($livraison);
        $entityManager->flush();

        $vraiCode = 'LIV-' . str_pad($livraison->getId(), 2, '0', STR_PAD_LEFT);
        $livraison->setCode($vraiCode);
        $totalCommandes = 0;
        foreach ($commandes as $commande) {
            $commande->setLivreur($livreur);
            $commande->setLivraison($livraison);
            
            $totalCommandes += (float) $commande->getPrix();
            
            $entityManager->persist($commande);
        }

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

        $commandes = $commandeRepository->findBy([
            'zone' => $livraison->getZone(),
            'livreur' => $livraison->getLivreur(),
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

    private function getPrixLivraisonParZone(string $nomZone): string
    {
        $prixParZone = [
            'Zone 1' => '500',
            'Zone 2' => '1000',
            'Zone 3' => '1500',
            'Zone 4' => '2000',
            'Zone 5' => '2500',
        ];

        if (isset($prixParZone[$nomZone])) {
            return $prixParZone[$nomZone];
        }

        foreach ($prixParZone as $zone => $prix) {
            if (strpos($nomZone, $zone) !== false) {
                return $prix;
            }
        }

        return '1000';
    }
}