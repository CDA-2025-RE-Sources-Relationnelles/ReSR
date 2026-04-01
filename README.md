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
    image: postgres
    restart: always
    shm_size: 128mb
    ports:
      - ${DB__Port}:5432
    environment:
      POSTGRES_PASSWORD: ${DB__Password}
      POSTGRES_USER: ${DB__Username}
      POSTGRES_DB: ${DB__Database}

  smtp:
    image: maildev/maildev
    environment:
      MAILDEV_SMTP_PORT: 1026
    ports:
      - "3000:1080"          # Interface web
      - "${Smtp__Port}:1026" # SMTP

  resr-api:
    build:
      context: .
      dockerfile: src/Presentation/Api/Dockerfile
    ports:
      - <Port HTTPS de l'API sur votre machine>:443
      - <Port HTTP de l'API sur votre machine>:8080
    env_file:
      - .env
    environment:
      ASPNETCORE_URLS: "https://+;http://+"
    volumes:
      - ${USERPROFILE}/.aspnet/https:/https
```

Puis, assurez vous que le dossier `%USERPROFILE%/.aspnet/https/` existe avant d'entrez cette commande dans le terminal powershell en remplaçant `ASPNETCORE_Kestrel__Certificates__Default__Password` par la valeur configurée dans le `.env` :

```shell
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p <ASPNETCORE_Kestrel__Certificates__Default__Password> --trust
```

\* Si vous êtes sur MacOS, remplacez la variable d'environnement `USERPROFILE` par `HOME`.

##### Exécution
```shell
docker-compose -f docker-compose.yml up -d
```

### Hébergement local
Si vous souhaitez démarrer la solution localement, entrez cette commande après avoir initialisé les variables d'environnement :

```shell
dotnet run --launch-profile https --project src\\Presentation\\Api --
```

\* Si vous souhaitez initialiser la base de données avec des données de test, ajoutez `-n` ou `new-test-db`.
\* Si vous souhaitez forcer cette action en réinitialisant la base de données, ajoutez aussi `-f` ou `force-init`.