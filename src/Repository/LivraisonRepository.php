<?php

namespace App\Repository;

use App\Entity\Livraison;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;

/**
 * @extends ServiceEntityRepository<Livraison>
 */
class LivraisonRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, Livraison::class);
    }

    public function save(Livraison $entity, bool $flush = false): void
    {
        $this->getEntityManager()->persist($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function remove(Livraison $entity, bool $flush = false): void
    {
        $this->getEntityManager()->remove($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function findByCode(string $code): ?Livraison
    {
        return $this->createQueryBuilder('l')
            ->andWhere('l.code = :code')
            ->setParameter('code', $code)
            ->getQuery()
            ->getOneOrNullResult();
    }

    public function findByLivreur(int $livreurId): array
    {
        return $this->createQueryBuilder('l')
            ->andWhere('l.livreur = :livreurId')
            ->setParameter('livreurId', $livreurId)
            ->orderBy('l.id', 'DESC')
            ->getQuery()
            ->getResult();
    }

    public function findByZone(int $zoneId): array
    {
        return $this->createQueryBuilder('l')
            ->andWhere('l.zone = :zoneId')
            ->setParameter('zoneId', $zoneId)
            ->orderBy('l.id', 'DESC')
            ->getQuery()
            ->getResult();
    }

    public function findWithDetails(int $livraisonId): ?Livraison
    {
        return $this->createQueryBuilder('l')
            ->leftJoin('l.zone', 'z')
            ->leftJoin('l.livreur', 'li')
            ->leftJoin('l.commande', 'c')
            ->addSelect('z', 'li', 'c')
            ->andWhere('l.id = :id')
            ->setParameter('id', $livraisonId)
            ->getQuery()
            ->getOneOrNullResult();
    }

    public function findAllOrderedByDate(): array
    {
        return $this->createQueryBuilder('l')
            ->orderBy('l.id', 'DESC')
            ->getQuery()
            ->getResult();
    }
}