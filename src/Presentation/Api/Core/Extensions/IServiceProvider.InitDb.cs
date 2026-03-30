using FluentResponse;
using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Extensions;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    private static readonly string[] DefaultCategoryNames = [
        "Communication",
        "Cultures",
        "Développement personnel",
        "Intelligence émotionnelle",
        "Loisirs",
        "Monde professionnel",
        "Parentalité",
        "Qualité de vie",
        "Recherche de sens",
        "Santé physique",
        "Santé psychique",
        "Spiritualité",
        "Vie affective",
    ];

    /// <summary>
    /// Initializes the database.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitDb(this WebApplication app) {

        using var scope = app.Services.CreateScope();
        app.Logger.LogInformation("Database initialization started...");

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        dbContext.Add(Manager.TryCreate(app.Configuration["Root:Email"]!, app.Configuration["Root:Password"]!, ManagerPermissions.SuperAdminRole).Unwrap());
        
        dbContext.InitUsers();
        dbContext.InitCategories();
        dbContext.InitResources();
        dbContext.InitComments();

        app.Logger.LogInformation("Database initialization completed!");

    }

    private static void InitUsers(this DbContext dbContext) {

        Random rand = new();
        
        string GeneratePassword() {

            int    randValue;
            string password = "";
            char   character;

            for (int i = 0; i < 4; i++) {

                randValue = rand.Next(0, 26);
                character    = Convert.ToChar(randValue + 'a');
                password += character;
                
            } for (int i = 0; i < 4; i++) {

                randValue = rand.Next(0, 26);
                character    = Convert.ToChar(randValue + 'A');
                password += character;

            } for (int i = 0; i < 4; i++) {

                randValue = rand.Next(0, 10);
                character    = Convert.ToChar(randValue + '0');
                password += character;

            }

            return password;
        }

        List<UserPermissions> userPermissions = [..FlagsManipulation.GetUniqueValues<UserPermissions>(), UserPermissions.None];

        dbContext.AddRange(
            Enumerable.Range(0, 100).Select(i =>
                User.TryCreate(
                    username    : $"utilisateur_{i}",
                    email       : $"user{i}@domaine.com",
                    password    : GeneratePassword(),
                    permissions : userPermissions[i % userPermissions.Count]
                ).Unwrap()
            )
        );

        dbContext.SaveChanges();
    }

    private static void InitCategories(this DbContext dbContext) {
        dbContext.AddRange(DefaultCategoryNames.Select(name => Category.TryCreate(name).Unwrap()));
        dbContext.SaveChanges();
    }

    private static void InitResources(this DbContext dbContext) {

        var random = new Random();

        var users = dbContext.Set<User>().ToList();
        var categories = dbContext.Set<Category>().ToList();
        var relationships = Relationships.All.GetUniqueValues().ToList();

        dbContext.AddRange(
            Enumerable.Range(0, 50).Select(i =>
                TextResource.TryCreate(
                    title       : $"Article N°{i}",
                    description : """
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.
                        """
                    ,
                    category      : categories[random.Next(categories.Count)],
                    relationships : Enumerable
                        .Range(0, random.Next(1, relationships.Count))
                        .Select(i => relationships[i % relationships.Count])
                        .Aggregate((a, b) => a | relationships[random.Next(relationships.Count)]),
                    content : """
                        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.</p>
                        <p>Morbi a mi semper, malesuada diam in, dapibus eros. Vivamus tincidunt lobortis leo, eu scelerisque neque tristique a. Nullam mauris arcu, lacinia eget augue ac, volutpat imperdiet augue. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin bibendum, libero id scelerisque finibus, felis augue tristique est, et tristique eros nibh vel urna. Ut ac est magna. Nulla luctus libero ac magna finibus aliquam. In hac habitasse platea dictumst. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean ac accumsan orci. Pellentesque finibus sit amet nunc id pretium. Sed a ligula mollis, molestie lacus non, fringilla leo.</p>
                        <p>Nullam auctor, velit nec feugiat dapibus, lacus ipsum tempus ante, eget dignissim mi ipsum ac mauris. Etiam porta euismod massa, quis porttitor nunc vehicula non. Proin sollicitudin libero sapien, non sodales sapien eleifend eu. Morbi quis urna sit amet felis cursus vehicula quis non nulla. Sed vel ligula mi. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Curabitur fermentum ultrices lorem, quis euismod felis mattis a.</p>
                        <p>Pellentesque sollicitudin felis nec vestibulum facilisis. Mauris eget elit in dolor ultricies tempus. Ut metus mauris, tristique in tristique quis, tempus a sem. Sed euismod lorem lacus, ut auctor erat aliquam nec. Suspendisse quam dui, congue ut commodo ac, posuere in erat. Pellentesque eu ante sed nulla pulvinar auctor vitae in elit. Cras malesuada cursus enim vitae bibendum. Quisque volutpat accumsan risus a porttitor. Integer ullamcorper, lectus vitae scelerisque auctor, tortor urna tincidunt dui, quis malesuada lorem ex nec sem. Quisque et mi ac ipsum consectetur egestas.</p>
                        <p>Aliquam erat volutpat. Fusce metus erat, euismod nec urna ut, iaculis fermentum mauris. Ut ac tincidunt nisi. In scelerisque dui ante, id sodales dolor tempor ac. Morbi elementum vel tellus vel rutrum. Integer porta ornare lectus non egestas. Maecenas pellentesque erat ex, a consequat metus pretium sit amet. Suspendisse tellus orci, convallis in mollis ac, egestas in nisl. Donec aliquet dignissim feugiat. Nullam euismod nec ex ut tempus. Mauris sit amet bibendum mi, at aliquet sem. Donec et laoreet nisl. Nullam pharetra fermentum enim, a pretium lacus pellentesque id. Mauris facilisis hendrerit odio, ut aliquam ex blandit eget. Vivamus facilisis mattis pharetra.</p>
                        """
                    ,
                    isPrivate : (i & 0b11) == 0b11,
                    owner     : (i & 0b01) == 0b01 ? users[random.Next(users.Count)] : null
                ).OnSuccess(x => (i & 0b11) == 0b11 ? x.AsPublic() : x).Unwrap()
            )
        );

    
        dbContext.AddRange(
            Enumerable.Range(0, 50).Select(i =>
                QuizResource.TryCreate(
                    title       : $"Quiz N°{i}",
                    description : """
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.
                        """
                    ,
                    category      : categories[random.Next(categories.Count)],
                    relationships : Enumerable
                        .Range(0, random.Next(1, relationships.Count))
                        .Select(i => relationships[i % relationships.Count])
                        .Aggregate((a, b) => a | relationships[random.Next(relationships.Count)]),
                    questions : Enumerable.Range(0, random.Next(1, 20)).Select(i => new QuizQuestion() {
                        Content = $"Question N°{i}",
                        Score   = random.Next(1, 256),
                        Answers = [.. Enumerable.Range(0, random.Next(2, 4)).Select(j => new QuizAnswer() {
                            IsCorrect = j == 0,
                            Content   = $"Réponse N°{j}"
                        })]
                    }),
                    isPrivate : (i & 0b01) == 0b01,
                    owner     : (i & 0b01) == 0b01 ? users[random.Next(users.Count)] : null
                ).OnSuccess(x => (i & 0b11) == 0b11 ? x.AsPublic() : x).Unwrap()
            )
        );
        
        dbContext.SaveChanges();
    }

    private static void InitComments(this DbContext dbContext) {

        var random = new Random();

        var users = dbContext.Set<User>().ToList();
        var resources = dbContext.Set<Resource>().Where(x => x.Visibility == Visibility.Public).ToList();

        dbContext.AddRange(
            Enumerable.Range(0, 200).Select(i =>
                Comment.TryCreate(
                    sentBy            : users[random.Next(users.Count)],
                    content           : "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.",
                    commentedResource : resources[random.Next(resources.Count)]
                ).OnSuccess(x => (i & 0b10) == 0b10 ? x.TryWithReport(
                    by: users[random.Next(users.Count)],
                    content: "Lorem ipsum dolor sit amet!"
                ).Unwrap(y => y, e => x) : x)
                .Unwrap()
            )
        );

        dbContext.SaveChanges();

        var comments = dbContext.Set<Comment>().ToList();
        dbContext.AddRange(
            Enumerable.Range(0, 100).Select(i =>
                Comment.TryCreate(
                    sentBy            : users[random.Next(users.Count)],
                    content           : "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.",
                    commentedResource : resources[random.Next(resources.Count)],
                    answeredComment   : comments[random.Next(comments.Count)]
                ).OnSuccess(x => (i & 0b10) == 0b10 ? x.TryWithReport(
                    by: users[random.Next(users.Count)],
                    content: "Lorem ipsum dolor sit amet!"
                ).Unwrap(y => y, e => x) : x)
                .Unwrap()
            )
        );

        dbContext.SaveChanges();

        comments = [.. dbContext.Set<Comment>()];
        dbContext.AddRange(
            Enumerable.Range(0, 100).Select(i =>
                Comment.TryCreate(
                    sentBy            : users[random.Next(users.Count)],
                    content           : "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.",
                    commentedResource : resources[random.Next(resources.Count)],
                    answeredComment   : comments[random.Next(comments.Count)]
                ).OnSuccess(x => (i & 0b10) == 0b10 ? x.TryWithReport(
                    by: users[random.Next(users.Count)],
                    content: "Lorem ipsum dolor sit amet!"
                ).Unwrap(y => y, e => x) : x)
                .Unwrap()
            )
        );
        
        dbContext.SaveChanges();
    }
}