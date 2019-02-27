using System;
using System.Collections.Generic;
using System.Text;
using HW_03WebII.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HW_03WebII.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPostModel> BlogPosts { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogTags>(entity =>
            {
                entity.ToTable("blogtags");
                entity.Property(e => e.BlogId).HasColumnName("blogid");
                entity.Property(e => e.TagId).HasColumnName("tagid");
            });

            builder.Entity<BlogTags>()
                .HasKey(ps => new { ps.BlogId, ps.TagId });

            builder.Entity<BlogTags>()
                .HasOne(ps => ps.Blog)
                .WithMany(p => p.BlogTags)
                .HasForeignKey(ps => ps.BlogId);

            builder.Entity<BlogTags>()
                .HasOne(ps => ps.Tag)
                .WithMany(p => p.BlogTags)
                .HasForeignKey(ps => ps.TagId);
        }
    }
}
