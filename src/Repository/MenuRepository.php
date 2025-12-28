<?php

namespace App\Repository;

use App\Entity\Menu;
use Doctrine\Bundle\DoctrineBundle\Repository\ServiceEntityRepository;
use Doctrine\Persistence\ManagerRegistry;

/**
 * @extends ServiceEntityRepository<Menu>
 */
class MenuRepository extends ServiceEntityRepository
{
    public function __construct(ManagerRegistry $registry)
    {
        parent::__construct($registry, Menu::class);
    }

    public function save(Menu $entity, bool $flush = false): void
    {
        $this->getEntityManager()->persist($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function remove(Menu $entity, bool $flush = false): void
    {
        $this->getEntityManager()->remove($entity);

        if ($flush) {
            $this->getEntityManager()->flush();
        }
    }

    public function findByCode(string $code): ?Menu
    {
        return $this->createQueryBuilder('m')
            ->andWhere('m.code = :code')
            ->setParameter('code', $code)
            ->getQuery()
            ->getOneOrNullResult();
    }

    public function findAvailable(): array
    {
        return $this->createQueryBuilder('m')
            ->andWhere('m.etat = :etat')
            ->setParameter('etat', 'Disponible')
            ->orderBy('m.nom', 'ASC')
            ->getQuery()
            ->getResult();
    }

    public function findWithDetails(): array
    {
        return $this->createQueryBuilder('m')
            ->leftJoin('m.burger', 'b')
            ->leftJoin('m.frites', 'f')
            ->leftJoin('m.boisson', 'bo')
            ->addSelect('b', 'f', 'bo')
            ->andWhere('m.etat = :etat')
            ->setParameter('etat', 'Disponible')
            ->orderBy('m.nom', 'ASC')
            ->getQuery()
            ->getResult();
    }
}