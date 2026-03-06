SET client_encoding = 'UTF8';

-- ==========================================
-- 1. Création des catégories
-- ==========================================
INSERT INTO "Categories" ("Id", "Name") 
VALUES 
    (1, 'Santé psychique'),
    (2, 'Le travail'),
    (3, 'Communication')
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name";


-- ==========================================
-- 2. Nettoyage des anciennes données de test
-- ==========================================
DELETE FROM "Resource" WHERE "Title" LIKE '%(Test n°%';


-- ==========================================
-- 3. Génération des ressources de test
-- ==========================================
DO $$
DECLARE 
    i INT;
    cat_id INT;
    titre TEXT;
    contenu_html TEXT;
    published_at TIMESTAMPTZ;
    edited_at TIMESTAMPTZ;
    seconds_since_publish INT;
    random_titles TEXT[] := ARRAY['Gérer le stress', 'Productivité', 'Sommeil', 'Communication'];
BEGIN
    FOR i IN 1..100 LOOP
        cat_id := floor(random() * 3 + 1);
        titre := random_titles[floor(random() * array_length(random_titles, 1) + 1)] || ' (Test n°' || i || ')';
        contenu_html := '<p>Contenu test ' || i || '</p>';

        -- PublishedAt : date propre, sans microsecondes
        published_at := date_trunc('second', NOW() - (floor(random() * 365) * interval '1 day'));

        -- EditedAt : entre PublishedAt et maintenant
        seconds_since_publish := EXTRACT(EPOCH FROM (NOW() - published_at))::INT;
        edited_at := date_trunc('second', published_at + (floor(random() * seconds_since_publish) * interval '1 second'));

        INSERT INTO "Resource" (
            "Title", "CategoryId", "Relationships", "Visibility", 
            "PublishedAt", "EditedAt", "OwnerId", "Discriminator",
            "Content", "TextResource_Content"
        ) 
        VALUES (
            titre, 
            cat_id, 
            63, 
            2, 
            published_at,
            edited_at,
            NULL, 
            'TextResource',
            NULL,
            contenu_html
        );
    END LOOP;
END $$;