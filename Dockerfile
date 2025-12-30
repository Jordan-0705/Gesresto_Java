# Dockerfile
FROM php:8.4-apache

# 1. Installer les extensions
RUN docker-php-ext-install pdo pdo_pgsql && a2enmod rewrite

# 2. Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# 3. Configurer Apache
ENV APACHE_DOCUMENT_ROOT /var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf

# 4. Copier l'application (ton .env sera copié automatiquement)
COPY . /var/www/html
WORKDIR /var/www/html

# 5. Installer les dépendances SANS exécuter les scripts problématiques
RUN composer install --no-dev --optimize-autoloader --no-interaction --no-scripts

# 6. Port Render
EXPOSE 8080
RUN sed -i 's/Listen 80/Listen 8080/' /etc/apache2/ports.conf

# 7. Démarrer
CMD ["apache2-foreground"]