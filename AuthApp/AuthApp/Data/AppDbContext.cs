using Microsoft.EntityFrameworkCore;
using AuthApp.Models;

namespace AuthApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<UserActionLog> UserActionLog { get; set; }
        public DbSet<FileDocuments> FileDocuments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<FileTag> FileTags { get; set; }
        public DbSet<TagCategory> TagCategories { get; set; }
        public DbSet<TagCategoryTag> TagCategoryTags { get; set; }
        public DbSet<FavoriteDocument> FavoriteDocuments { get; set; }
        public DbSet<AuthApp.Models.KatoEntry> KatoEntries { get; set; }
        public DbSet<Counterparty> Counterparties => Set<Counterparty>();
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractParticipant> ContractParticipants { get; set; }
        public DbSet<ContractDocument> ContractDocument {  get; set; }
        public DbSet<Project>Projects { get; set; }
        public DbSet<ProjectDocument> ProjectDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TagCategoryTag>()
                .HasOne(tct => tct.Tag)
                .WithMany(t => t.TagCategoryTags)
                .HasForeignKey(tct => tct.TagId);

            modelBuilder.Entity<TagCategoryTag>()
                .HasOne(tct => tct.TagCategory)
                .WithMany(tc => tc.TagCategoryTags)
                .HasForeignKey(tct => tct.TagCategoryId);

           
            modelBuilder.Entity<TagCategoryTag>()
                .HasIndex(tct => new { tct.TagId, tct.TagCategoryId })
                .IsUnique();

            modelBuilder.Entity<ContractParticipant>()
    .HasIndex(cp => new { cp.ContractId, cp.UserId })
    .IsUnique();

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.SourceContract)
                .WithMany()
                .HasForeignKey(c => c.SourceContractId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
