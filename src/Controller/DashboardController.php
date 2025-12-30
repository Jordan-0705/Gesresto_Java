<?php

namespace App\Controller;

use App\Repository\CommandeRepository;
use App\Repository\CommandeBurgerRepository;
use App\Repository\CommandeMenuRepository;
use App\Repository\PaiementRepository;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class DashboardController extends AbstractController
{
    #[Route('/dashboard', name: 'app_dashboard')]
    public function index(
        CommandeRepository $commandeRepository,
        CommandeBurgerRepository $commandeBurgerRepository,
        CommandeMenuRepository $commandeMenuRepository,
        PaiementRepository $paiementRepository
    ): Response {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');
        
        $today = new \DateTimeImmutable();
        $startOfDay = $today->setTime(0, 0, 0);
        $endOfDay = $today->setTime(23, 59, 59);
        
        $commandesEnCours = $commandeRepository->createQueryBuilder('c')
            ->where('c.etatCommande = :etat')
            ->andWhere('c.date BETWEEN :start AND :end')
            ->setParameter('etat', 'En Attente')
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        $commandesValidees = $commandeRepository->createQueryBuilder('c')
            ->where('c.etatCommande = :etat')
            ->andWhere('c.date BETWEEN :start AND :end')
            ->setParameter('etat', 'Valide')
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        $recettesJournalieres = $paiementRepository->createQueryBuilder('p')
            ->select('SUM(p.montant) as total')
            ->where('p.date BETWEEN :start AND :end')
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getSingleScalarResult() ?? 0;
        
        $burgersPlusVendus = $commandeBurgerRepository->createQueryBuilder('cb')
            ->join('cb.burger', 'b')
            ->join('cb.commande', 'c')
            ->select('b.nom as burger_nom, SUM(cb.quantite) as total_vendu')
            ->where('c.date BETWEEN :start AND :end')
            ->groupBy('b.id', 'b.nom')
            ->orderBy('total_vendu', 'DESC')
            ->setMaxResults(5)
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        $menusPlusVendus = $commandeMenuRepository->createQueryBuilder('cm')
            ->join('cm.menu', 'm')
            ->join('cm.commande', 'c')
            ->select('m.nom as menu_nom, SUM(cm.quantite) as total_vendu')
            ->where('c.date BETWEEN :start AND :end')
            ->groupBy('m.id', 'm.nom')
            ->orderBy('total_vendu', 'DESC')
            ->setMaxResults(5)
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        $commandesAnnulees = $commandeRepository->createQueryBuilder('c')
            ->where('c.etatCommande = :etat')
            ->andWhere('c.date BETWEEN :start AND :end')
            ->setParameter('etat', 'Annule')
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        $commandesALivrer = $commandeRepository->createQueryBuilder('c')
            ->where('c.consoType = :type')
            ->andWhere('c.etatCommande = :etat')
            ->andWhere('c.date BETWEEN :start AND :end')
            ->setParameter('type', 'ALivrer')
            ->setParameter('etat', 'Valide')
            ->setParameter('start', $startOfDay)
            ->setParameter('end', $endOfDay)
            ->getQuery()
            ->getResult();
        
        return $this->render('dashboard/index.html.twig', [
            'gestionnaire' => $this->getUser(),
            'today' => $today,
            
            'commandes_en_cours' => count($commandesEnCours),
            'commandes_validees' => count($commandesValidees),
            'recettes_journalieres' => $recettesJournalieres,
            'burgers_plus_vendus' => $burgersPlusVendus,
            'menus_plus_vendus' => $menusPlusVendus,
            'commandes_annulees' => count($commandesAnnulees),
            'commandes_a_livrer' => count($commandesALivrer),
            
            'commandes_en_cours_list' => $commandesEnCours,
            'commandes_a_livrer_list' => $commandesALivrer,
        ]);
    }
}