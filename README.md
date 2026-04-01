### Initialiser le projet

Clonez d'abord le dépôt en entrant cette commande dans le terminale :

```shell
git clone https://github.com/CDA-2025-RE-Sources-Relationnelles/ReSR.git
```

### Configurer la solution

Vous pouvez configurer la solution à l'aide du fichier `.env` (un exemple est donné avec `.env.example`) :

```env
Root__Email   = <Adresse électronique de l'administrateur racine>
Root_Password = <Mot de passe de l'administrateur racine>

DB__Host          = <Adresse de la base de données PostgreSQL>
DB__Port          = <Port de la base de données PostgreSQL>
DB__Database      = <Nom de la base de données PostgreSQL>
DB__Username      = <Utilisateur de la base de données PostgreSQL>
DB__Password      = <Mot de passe de la base de données PostgreSQL>
DB__EncryptionKey = <Clé de chiffrement 128bit pour la base de données>

Jwt__Key             = <Clé de chiffrement 512bit pour l'authentification>
Jwt__Issuer          = "resr.fr"
Jwt__Audience        = "resr.fr"
Jwt__Expiry__Manager = <Durée de l'authentification administrateur (hh:mm)>
Jwt__Expiry__User    = <Durée de l'authentification utilisateur (hh:mm)>

Pin__RegistrationValidationRequestExpiry = <Durée des codes PIN de création de compte utilisateur (hh:mm)>
Pin__PasswordResetRequestExpiry          = <Durée des codes PIN de réinistialisation de mot de passe utilisateur (hh:mm)>

Smtp__Host        = <Adresse du service d'envoi de courrier électronique>
Smtp__Port        = <Port du service d'envoi de courrier électronique>
Smtp__SenderEmail = "noreply@resr.fr"
  
Logging__LogLevel__Default                       = "Information"
Logging__LogLevel__Microsoft.EntityFrameworkCore = "Warning"
```

### Hébergement Docker
#### Configuration

Configurez votre `docker-compose.yml` en remplaçant les données entre `<...>` par celle désirées (un exemple est donné avec `docker-compose.yml.example`) :

```yml
services:
  db:
    image: postgres:14.22
    restart: always
    shm_size: 128mb
    networks:
      - resr
    environment:
      POSTGRES_PASSWORD: ${DB__Password}
      POSTGRES_USER: ${DB__Username}
      POSTGRES_DB: ${DB__Database}
    volumes:
      - resr-db:/var/lib/postgresql/data/

  resr-api:
    build:
      context: .
      dockerfile: src/Presentation/Api/Dockerfile
    ports:
      - <Port HTTP de l'API>:80
      - <Port HTTPS de l'API>:443
    env_file:
      - .env
    environment:
      ASPNETCORE_ENVIRONMENT: "Development"
      ASPNETCORE_URLS: "https://+;http://+"
      ASPNETCORE_HTTPS_PORTS: "443"
    command: ["-n"]
    volumes:
      - ${USERPROFILE}/.aspnet/https:/https
      - ~/.vsdbg:/remote_debugger:rw
    depends_on:
      - db

networks:
  resr:

volumes:
  resr-db:
```

Puis, assurez vous que le dossier `%USERPROFILE%/.aspnet/https/` existe avant d'entrez cette commande dans le terminal powershell en remplaçant `ASPNETCORE_Kestrel__Certificates__Default__Password` par la valeur configurée dans le `.env` :

```shell
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p <ASPNETCORE_Kestrel__Certificates__Default__Password> --trust
```

\* Si vous êtes sur MacOS, remplacez la variable d'environnement `USERPROFILE` par `HOME`.

##### Exécution

Si vous souhaitez démarrer la solution en environnement de developpement, entrez cette commande :

```shell
docker-compose -f docker-compose.dev.yml up -d --build
```

Si vous souhaitez démarrer la solution en environnement de production, entrez cette commande :

```shell
docker-compose -f docker-compose.yml up -d --build
```

L'API sera accessible sur `http://localhost:<Port HTTP de l'API>` et automatiquement redirigée vers `https://localhost:<Port HTTPS de l'API>`.

### Hébergement local
Si vous souhaitez démarrer la solution localement, entrez cette commande après avoir initialisé les variables d'environnement :

```shell
dotnet run --launch-profile https --project src\\Presentation\\Api --
```

\* Si vous souhaitez initialiser la base de données, ajoutez `-n` ou `--new-db`.
\* Si vous souhaitez indiquer la base de données avec des données de test, ajoutez aussi `-d` ou `--dev`.
\* Si vous souhaitez forcer cette action en réinitialisant la base de données, ajoutez aussi `-f` ou `--force-init`.