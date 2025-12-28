<?php

namespace App\Repository;

use App\Entity\CommandeBurger;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;

/**
 * @extends ServiceEntityRepository<CommandeBurger>
 */
class CommandeBurgerRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, CommandeBurger::class);
    }

    public function save(CommandeBurger $entity, bool $flush = false): void
    {
        $this->getEntityManager()->persist($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function remove(CommandeBurger $entity, bool $flush = false): void
    {
        $this->getEntityManager()->remove($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function findByCommande(int $commandeId): array
    {
        return $this->createQueryBuilder('cb')
            ->andWhere('cb.commande = :commandeId')
            ->setParameter('commandeId', $commandeId)
            ->orderBy('cb.id', 'ASC')
            ->getQuery()
            ->getResult();
    }

    public function findWithDetailsByCommande(int $commandeId): array
    {
        return $this->createQueryBuilder('cb')
            ->leftJoin('cb.burger', 'b')
            ->leftJoin('cb.complementF', 'f')
            ->leftJoin('cb.complementB', 'bo')
            ->addSelect('b', 'f', 'bo')
            ->andWhere('cb.commande = :commandeId')
            ->setParameter('commandeId', $commandeId)
            ->orderBy('cb.id', 'ASC')
            ->getQuery()
            ->getResult();
    }
}