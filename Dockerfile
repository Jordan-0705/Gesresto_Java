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
    curl \
    libzip-dev \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libonig-dev \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install \
        pdo \
        pdo_pgsql \
        zip \
        gd \
        mbstring \
        exif \
        pcntl \
        bcmath \
    && a2enmod rewrite \
    && a2enmod headers \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# --------------------------------------
# 2. Installer Composer
# --------------------------------------
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# --------------------------------------
# 3. DocumentRoot Apache → Symfony public/
# --------------------------------------
ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

# --------------------------------------
# 4. Configuration Apache pour Render
# --------------------------------------
RUN echo '<Directory "/var/www/html/public">\n\
    Options -Indexes +FollowSymLinks\n\
    AllowOverride All\n\
    Require all granted\n\
    DirectoryIndex index.php\n\
    FallbackResource /index.php\n\
</Directory>' > /etc/apache2/conf-available/symfony.conf \
    && a2enconf symfony

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
# D'abord copier uniquement composer.json pour optimiser le cache Docker
COPY composer.json composer.lock symfony.lock ./
RUN composer install \
    --no-dev \
    --no-scripts \
    --no-autoloader \
    --prefer-dist \
    --optimize-autoloader \
    --no-interaction

# Copier le reste du code
COPY . .

# Exécuter les scripts post-install
RUN composer dump-autoload --optimize --no-dev --classmap-authoritative

# --------------------------------------
# 8. Build Symfony
# --------------------------------------
RUN php bin/console cache:clear --no-debug --no-warmup \
    && php bin/console cache:warmup \
    && php bin/console assets:install public --symlink --relative \
    && chmod -R 755 public/

# --------------------------------------
# 9. Permissions (CRITIQUE pour Render)
# --------------------------------------
RUN mkdir -p var/cache var/log \
    && chown -R www-data:www-data var/ \
    && chmod -R 775 var/ \
    && chown -R www-data:www-data /var/www/html \
    && find var/ -type d -exec chmod 775 {} \; \
    && find var/ -type f -exec chmod 664 {} \;

# --------------------------------------
# 10. Port Render
# --------------------------------------
EXPOSE 8080
RUN sed -i 's/Listen 80/Listen 8080/g' /etc/apache2/ports.conf \
    && sed -i 's/:80/:8080/g' /etc/apache2/sites-available/*.conf

# --------------------------------------
# 11. Script de démarrage
# --------------------------------------
COPY docker/start.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/start.sh
CMD ["start.sh"]