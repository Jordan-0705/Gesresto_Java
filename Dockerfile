# Étape 1 : Build avec Maven
FROM maven:3.9.2-eclipse-temurin-17 AS build
WORKDIR /app

COPY pom.xml .
COPY src ./src

RUN mvn clean package -DskipTests

# Étape 2 : Runtime
FROM eclipse-temurin:17-jdk-alpine
WORKDIR /app

# Copier le JAR final
COPY --from=build /app/target/gesresto.jar app.jar

# Variables d'environnement pour Neon PostgreSQL
ENV DB_URL=${DB_URL}
ENV DB_USER=${DB_USER}
ENV DB_PASSWORD=${DB_PASSWORD}

# Pour la sécurité : définir le timezone si nécessaire
ENV TZ=UTC

# Exposer un port si ton app lance une API
EXPOSE 8080

# Lancer l'application
CMD ["java", "-jar", "app.jar"]
