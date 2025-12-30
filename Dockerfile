# --------------------------------------
# IMAGE DE BASE
# --------------------------------------
FROM php:8.4-apache

# --------------------------------------
# 1. Dépendances système
# --------------------------------------
RUN apt-get update && apt-get install -y \
    libpq-dev \
    git \
    unzip \
    && docker-php-ext-install pdo pdo_pgsql \
    && a2enmod rewrite \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# --------------------------------------
# 2. DocumentRoot Apache → Symfony public/
# --------------------------------------
ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf

# --------------------------------------
# 3. Autoriser l’accès (fix 403)
# --------------------------------------
RUN echo '<Directory "/var/www/html/public">\n\
    AllowOverride All\n\
    Require all granted\n\
</Directory>\n\
ServerName localhost' > /etc/apache2/conf-available/symfony.conf \
    && a2enconf symfony

# --------------------------------------
# 4. Installer Composer
# --------------------------------------
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# --------------------------------------
# 5. Copier le projet
# --------------------------------------
WORKDIR /var/www/html
COPY . .

# --------------------------------------
# 6. Variables d'environnement
# --------------------------------------
ENV APP_ENV=prod
ENV APP_DEBUG=0

# --------------------------------------
# 7. Installer les dépendances Symfony
# --------------------------------------
RUN composer install \
    --no-dev \
    --optimize-autoloader \
    --no-interaction

# --------------------------------------
# 8. Permissions
# --------------------------------------
RUN chown -R www-data:www-data /var/www/html

# --------------------------------------
# 9. Port Render
# --------------------------------------
EXPOSE 8080
RUN sed -i 's/Listen 80/Listen 8080/' /etc/apache2/ports.conf

# --------------------------------------
# 10. Lancement Apache
# --------------------------------------
CMD ["apache2-foreground"]
