<?php

namespace App\Repository;

use App\Entity\Commande;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;

/**
 * @extends ServiceEntityRepository<Commande>
 */
class CommandeRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, Commande::class);
    }

    public function save(Commande $entity, bool $flush = false): void
    {
        $this->getEntityManager()->persist($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function remove(Commande $entity, bool $flush = false): void
    {
        $this->getEntityManager()->remove($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function findByCode(string $code): ?Commande
    {
        return $this->createQueryBuilder('c')
            ->andWhere('c.code = :code')
            ->setParameter('code', $code)
            ->getQuery()
            ->getOneOrNullResult();
    }

    public function findByClient(int $clientId): array
    {
        return $this->createQueryBuilder('c')
            ->andWhere('c.clientId = :clientId')
            ->setParameter('clientId', $clientId)
            ->orderBy('c.date', 'DESC')
            ->getQuery()
            ->getResult();
    }

    public function findByEtat(string $etat): array
    {
        return $this->createQueryBuilder('c')
            ->andWhere('c.etatCommande = :etat')
            ->setParameter('etat', $etat)
            ->orderBy('c.date', 'DESC')
            ->getQuery()
            ->getResult();
    }

    public function findWithDetails(int $commandeId): ?Commande
    {
        return $this->createQueryBuilder('c')
            ->leftJoin('c.zone', 'z')
            ->leftJoin('c.livreur', 'l')
            ->leftJoin('c.commandeBurgers', 'cb')
            ->leftJoin('c.commandeMenus', 'cm')
            ->leftJoin('c.paiements', 'p')
            ->addSelect('z', 'l', 'cb', 'cm', 'p')
            ->andWhere('c.id = :id')
            ->setParameter('id', $commandeId)
            ->getQuery()
            ->getOneOrNullResult();
    }

    public function findCommandesALivrer(): array
    {
        return $this->createQueryBuilder('c')
            ->andWhere('c.consoType = :type')
            ->andWhere('c.etatCommande = :etat')
            ->setParameter('type', 'ALivrer')
            ->setParameter('etat', 'Valide')
            ->orderBy('c.date', 'ASC')
            ->getQuery()
            ->getResult();
    }

    public function findCommandesALivrerParZone(int $zoneId): array
    {
        return $this->createQueryBuilder('c')
            ->where('c.zone = :zoneId')
            ->andWhere('c.consoType = :type')
            ->andWhere('c.etatCommande = :etat')
            ->andWhere('c.livreur IS NULL')
            ->setParameter('zoneId', $zoneId)
            ->setParameter('type', 'ALivrer')
            ->setParameter('etat', 'Valide')
            ->orderBy('c.date', 'ASC')
            ->getQuery()
            ->getResult();
    }
}