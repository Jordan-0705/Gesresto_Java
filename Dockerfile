FROM php:8.4-apache

# 1. Dépendances système requises
RUN apt-get update && apt-get install -y \
    libpq-dev \
    git \
    unzip \
    && docker-php-ext-install pdo pdo_pgsql \
    && a2enmod rewrite \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# 2. Apache DocumentRoot (Symfony)
ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf

# 3. Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# 4. Copier le projet
WORKDIR /var/www/html
COPY . .

# 5. Installer les dépendances Symfony
RUN composer install \
    --no-dev \
    --optimize-autoloader \
    --no-interaction

# 6. Permissions
RUN chown -R www-data:www-data /var/www/html

# 7. Port Render
EXPOSE 8080
RUN sed -i 's/Listen 80/Listen 8080/' /etc/apache2/ports.conf

# 8. Lancement
CMD ["apache2-foreground"]
