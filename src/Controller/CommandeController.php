<?php

namespace App\Controller;

use App\Entity\Commande;
use App\Form\CommandeFilterType;
use App\Repository\CommandeRepository;
use App\Repository\ClientRepository;
use App\Repository\BurgerRepository;
use App\Repository\MenuRepository;
use Doctrine\ORM\EntityManagerInterface;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class CommandeController extends AbstractController
{
    #[Route('/commandes', name: 'app_commandes_index')]
    public function index(
        Request $request,
        CommandeRepository $commandeRepository,
        ClientRepository $clientRepository,
        BurgerRepository $burgerRepository,
        MenuRepository $menuRepository
    ): Response {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        // Récupérer les paramètres de filtrage
        $etat = $request->query->get('etat');
        $date = $request->query->get('date');
        $clientId = $request->query->get('client');
        $type = $request->query->get('type'); // burger ou menu

        // Construire la requête avec filtres
        $qb = $commandeRepository->createQueryBuilder('c')
            ->orderBy('c.date', 'DESC');

        // Filtre par état
        if ($etat && $etat !== 'Tous') {
            $qb->andWhere('c.etatCommande = :etat')
               ->setParameter('etat', $etat);
        }

        // Filtre par date
        if ($date) {
            $dateObj = \DateTimeImmutable::createFromFormat('Y-m-d', $date);
            if ($dateObj) {
                $startOfDay = $dateObj->setTime(0, 0, 0);
                $endOfDay = $dateObj->setTime(23, 59, 59);
                $qb->andWhere('c.date BETWEEN :start AND :end')
                   ->setParameter('start', $startOfDay)
                   ->setParameter('end', $endOfDay);
            }
        }

        // Filtre par client
        if ($clientId) {
            $qb->andWhere('c.clientId = :clientId')
               ->setParameter('clientId', $clientId);
        }

        // Filtre par type (burger ou menu)
        if ($type) {
            $qb->andWhere('c.commandeType = :type')
               ->setParameter('type', $type);
        }

        // Pagination
        $page = $request->query->getInt('page', 1);
        $limit = 5;
        $offset = ($page - 1) * $limit;

        $totalCommandes = count($qb->getQuery()->getResult());
        $totalPages = ceil($totalCommandes / $limit);

        $commandes = $qb->setFirstResult($offset)
                       ->setMaxResults($limit)
                       ->getQuery()
                       ->getResult();

        // Récupérer les données pour les filtres
        $clients = $clientRepository->findAll();
        $burgers = $burgerRepository->findAll();
        $menus = $menuRepository->findAll();

        // États possibles
        $etats = ['En Attente', 'Valide', 'Termine', 'Annule', 'Tous'];

        return $this->render('commande/index.html.twig', [
            'commandes' => $commandes,
            'clients' => $clients,
            'burgers' => $burgers,
            'menus' => $menus,
            'etats' => $etats,
            'current_filters' => [
                'etat' => $etat,
                'date' => $date,
                'client' => $clientId,
                'type' => $type,
            ],
            'page' => $page,
            'total_pages' => $totalPages,
            'total_commandes' => $totalCommandes,
        ]);
    }

    #[Route('/commandes/{id}', name: 'app_commandes_show')]
    public function show(Commande $commande): Response
    {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        return $this->render('commande/show.html.twig', [
            'commande' => $commande,
        ]);
    }

    #[Route('/commandes/{id}/changer-etat/{nouvelEtat}', name: 'app_commandes_changer_etat')]
    public function changerEtat(
        Commande $commande, 
        string $nouvelEtat,
        EntityManagerInterface $entityManager
    ): Response {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        $etatsValides = ['En Attente', 'Valide', 'Termine', 'Annule'];
        
        if (!in_array($nouvelEtat, $etatsValides)) {
            $this->addFlash('error', 'État invalide');
            return $this->redirectToRoute('app_commandes_show', ['id' => $commande->getId()]);
        }

        $commande->setEtatCommande($nouvelEtat);
        $entityManager->flush();

        $this->addFlash('success', "État de la commande changé à '$nouvelEtat'");

        return $this->redirectToRoute('app_commandes_show', ['id' => $commande->getId()]);
    }

    #[Route('/commandes/{id}/affecter-livreur/{livreurId}', name: 'app_commandes_affecter_livreur')]
    public function affecterLivreur(
        Commande $commande,
        int $livreurId,
        EntityManagerInterface $entityManager
    ): Response {
        $this->denyAccessUnlessGranted('ROLE_GESTIONNAIRE');

        // Ici vous devriez récupérer le livreur depuis la base
        // Pour l'instant, on simule
        $commande->setLivreur(null); // À remplacer par le vrai livreur
        $entityManager->flush();

        $this->addFlash('success', 'Livreur affecté à la commande');

        return $this->redirectToRoute('app_commandes_show', ['id' => $commande->getId()]);
    }
}