# Dockerfile pour Symfony avec PostgreSQL (Neon) sur Render
FROM php:8.2-apache

# Installer les extensions nécessaires
RUN apt-get update && apt-get install -y \
    libpng-dev \
    libzip-dev \
    libpq-dev \
    libicu-dev \
    zip \
    unzip \
    git \
    curl \
    && docker-php-ext-configure intl \
    && docker-php-ext-install \
    pdo \
    pdo_pgsql \
    pdo_mysql \
    gd \
    zip \
    intl \
    opcache \
    bcmath \
    && a2enmod rewrite headers

# Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# Configurer Apache pour Symfony
ENV APACHE_DOCUMENT_ROOT /var/www/html/public
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

# Configurer PHP pour la production
RUN mv "$PHP_INI_DIR/php.ini-production" "$PHP_INI_DIR/php.ini" \
    && echo "memory_limit = 256M" >> "$PHP_INI_DIR/conf.d/memory.ini" \
    && echo "upload_max_filesize = 50M" >> "$PHP_INI_DIR/conf.d/uploads.ini" \
    && echo "post_max_size = 50M" >> "$PHP_INI_DIR/conf.d/uploads.ini" \
    && echo "max_execution_time = 300" >> "$PHP_INI_DIR/conf.d/timeout.ini" \
    && echo "date.timezone = UTC" >> "$PHP_INI_DIR/conf.d/timezone.ini"

# Installer Node.js pour Webpack Encore (si nécessaire)
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g yarn

# Copier l'application
COPY . /var/www/html
WORKDIR /var/www/html

# Installer les dépendances PHP
RUN composer install --no-dev --optimize-autoloader --no-interaction --no-progress \
    && composer dump-autoload --optimize --no-dev --classmap-authoritative

# Installer les dépendances Node et builder les assets
RUN if [ -f package.json ]; then \
    npm install && npm run build; \
    fi

# Configurer les permissions
RUN chown -R www-data:www-data /var/www/html/var \
    && chmod -R 775 /var/www/html/var \
    && chown -R www-data:www-data /var/www/html/public

# Exécuter les migrations et vider le cache
RUN php bin/console cache:clear --env=prod --no-debug \
    && php bin/console cache:warmup --env=prod --no-debug

# Port pour Render
EXPOSE 8080

# Configurer Apache pour écouter sur le port 8080
RUN echo "Listen 8080" >> /etc/apache2/ports.conf \
    && sed -i 's/<VirtualHost \*:80>/<VirtualHost \*:8080>/g' /etc/apache2/sites-available/000-default.conf \
    && sed -i 's/Listen 80/Listen 8080/g' /etc/apache2/ports.conf

# Health check pour Render
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

# Commande de démarrage
CMD ["apache2-foreground"]