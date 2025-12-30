# Dockerfile CORRIGÉ
FROM php:8.4-apache

# Installer les extensions (version simplifiée)
RUN apt-get update && apt-get install -y \
    libpng-dev libzip-dev libpq-dev \
    && docker-php-ext-install pdo pdo_pgsql gd zip \
    && a2enmod rewrite

# Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# Configurer Apache pour Symfony
ENV APACHE_DOCUMENT_ROOT /var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

# Copier l'application
COPY . /var/www/html
WORKDIR /var/www/html

# Installer les dépendances PHP (SIMPLIFIÉ)
RUN composer install --no-dev --optimize-autoloader --no-interaction

# Configurer les permissions
RUN chown -R www-data:www-data /var/www/html/var \
    && chmod -R 775 /var/www/html/var

# Port pour Render
EXPOSE 8080

# Configurer le port 8080 (CORRIGÉ)
RUN sed -i 's/Listen 80/Listen 8080/g' /etc/apache2/ports.conf \
    && sed -i 's/:80/:8080/g' /etc/apache2/sites-available/*.conf

# Démarrer Apache
CMD ["apache2-foreground"]