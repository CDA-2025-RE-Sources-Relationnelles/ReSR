using Microsoft.EntityFrameworkCore;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Core;
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) {

    #region PROPERTIES

        private readonly IEncryptionService encryptionService = null!;

        public DbSet<Manager> Managers { get; init; }
        public DbSet<User>    Users    { get; init; }

        public DbSet<Category> Categories { get; init; }

        public DbSet<Comment>        Comments        { get; init; }
        public DbSet<PrivateMessage> PrivateMessages { get; init; }

        public DbSet<QuizSession> QuizSessions { get; init; }

        public DbSet<QuizResource> QuizResources { get; init; }
        public DbSet<TextResource> TextResources { get; init; }


    #endregion
    #region CONSTRUCTORS
    
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IEncryptionService                     encryptionService
        ) : this(options) {
            this.encryptionService = encryptionService;
        }

    #endregion
    #region METHODS

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            base.OnModelCreating(modelBuilder);

            // Automatically ignore aggregate roots' domain events from the mapping.
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(x => x.ClrType.GetInterfaces().Any(x => x.IsGenericType == true && (x.GetGenericTypeDefinition() == typeof(IAggregateRoot<>))))
            ) entityType.AddIgnored(nameof(IAggregateRoot<>.DomainEvents));

            modelBuilder.Entity<Manager>(e => {
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasConversion(
                    x => this.encryptionService.Encrypt(x),
                    x => new (this.encryptionService.Decrypt(x))
                );
                e.Property(x => x.Password).HasConversion(x => x.Hash, x => new (x));
            });

            modelBuilder.Entity<User>(e => {
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Username).HasConversion(
                    x => this.encryptionService.Encrypt(x),
                    x => new (this.encryptionService.Decrypt(x))
                );
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasConversion(
                    x => this.encryptionService.Encrypt(x),
                    x => new (this.encryptionService.Decrypt(x))
                );
                e.Property(x => x.Password).HasConversion(x => x.Hash, x => new (x));
                e.HasMany(x => x.LikedUsers).WithMany(x => x.LikedBy).UsingEntity(join => join.ToTable("Friends"));
            });


            modelBuilder.Entity<Category>(e => {
                e.HasIndex(x => x.Name).IsUnique();
                e.HasMany(x => x.Resources).WithOne(x => x.Category);
            });

            modelBuilder.Entity<Comment>(e => {
                e.HasOne(x => x.SentBy);
                e.HasOne(x => x.CommentedResource).WithMany(x => x.Comments);
                e.HasMany(x => x.Answers).WithOne(x => x.AnsweredComment);
                e.OwnsMany(x => x.Reports, x => {
                    x.HasOne(x => x.ReportedBy);
                });
            });

            modelBuilder.Entity<PrivateMessage>(e => {
                e.HasOne(x => x.SentBy);
                e.HasOne(x => x.SentTo);
                e.HasOne(x => x.QuotedResource);
                e.Property(x => x.Content).HasConversion(
                    x => this.encryptionService.Encrypt(x),
                    x => new (this.encryptionService.Decrypt(x))
                );
            });

            modelBuilder.Entity<QuizSession>(e => {
                e.OwnsMany(x => x.Participations);
            });

            modelBuilder.Entity<Resource>(e => {
                e.HasIndex(nameof(Resource.Title), nameof(Resource.Category)+nameof(Category.Id)).IsUnique();
                e.HasOne(x => x.Owner).WithMany(x => x.OwnedResources);
                e.HasOne(x => x.VerifyingUser).WithMany(x => x.ResourcesToVerify);
                e.HasMany(x => x.LikedBy).WithMany().UsingEntity(join => join.ToTable("Likes"));
                e.HasMany(x => x.BookmarkedBy).WithMany(x => x.Bookmarks).UsingEntity(join => join.ToTable("Bookmarks"));
                e.HasMany(x => x.ExploitedBy).WithMany().UsingEntity(join => join.ToTable("Exploits"));
            });

            modelBuilder.Entity<QuizResource>(e => {
                e.OwnsMany(x => x.Questions, x => {
                    x.OwnsMany(x => x.Answers);
                });
            });
        }

    #endregion
}