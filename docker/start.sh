#!/bin/bash
set -e

# Attendre que la base de données soit prête si nécessaire
# (optionnel, selon votre configuration)

# Créer les répertoires nécessaires
mkdir -p var/cache var/log

# Définir les permissions
chown -R www-data:www-data var/
chmod -R 775 var/

# Exécuter les migrations si en production
if [ "$APP_ENV" = "prod" ]; then
    php bin/console doctrine:migrations:migrate --no-interaction --allow-no-migration
fi

# Vider et réchauffer le cache
php bin/console cache:clear --no-debug
php bin/console cache:warmup

# Lancer Apache
exec apache2-foreground