<?php

namespace App\Repository;

use App\Entity\CommandeMenu;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;

/**
 * @extends ServiceEntityRepository<CommandeMenu>
 */
class CommandeMenuRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, CommandeMenu::class);
    }

    public function save(CommandeMenu $entity, bool $flush = false): void
    {
        $this->getEntityManager()->persist($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function remove(CommandeMenu $entity, bool $flush = false): void
    {
        $this->getEntityManager()->remove($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function findByCommande(int $commandeId): array
    {
        return $this->createQueryBuilder('cm')
            ->andWhere('cm.commande = :commandeId')
            ->setParameter('commandeId', $commandeId)
            ->orderBy('cm.id', 'ASC')
            ->getQuery()
            ->getResult();
    }

    public function findWithDetailsByCommande(int $commandeId): array
    {
        return $this->createQueryBuilder('cm')
            ->leftJoin('cm.menu', 'm')
            ->leftJoin('m.burger', 'b')
            ->leftJoin('m.frites', 'f')
            ->leftJoin('m.boisson', 'bo')
            ->addSelect('m', 'b', 'f', 'bo')
            ->andWhere('cm.commande = :commandeId')
            ->setParameter('commandeId', $commandeId)
            ->orderBy('cm.id', 'ASC')
            ->getQuery()
            ->getResult();
    }
}