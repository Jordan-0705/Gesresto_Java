#!/bin/bash
set -e

echo "🚀 Démarrage de l'application Symfony..."

# Attendre que la base de données soit prête (pour Neon)
if [ -n "$DATABASE_URL" ]; then
    echo "⏳ Vérification de la connexion à la base de données..."
    until php bin/console doctrine:query:sql "SELECT 1" > /dev/null 2>&1; do
        echo "⌛ En attente de la base de données..."
        sleep 2
    done
    echo "✅ Base de données connectée"
fi

# Exécuter les migrations
echo "🔄 Exécution des migrations..."
php bin/console doctrine:migrations:migrate --no-interaction --allow-no-migration

# Vider et réchauffer le cache
echo "🧹 Nettoyage du cache..."
php bin/console cache:clear --env=prod --no-debug
php bin/console cache:warmup --env=prod --no-debug

# Installer les assets
echo "📦 Installation des assets..."
php bin/console assets:install public --env=prod --no-debug

# Définir les permissions
chown -R www-data:www-data /var/www/html/var
chmod -R 775 /var/www/html/var

echo "✅ Application prête !"

# Démarrer Apache
exec apache2-foreground