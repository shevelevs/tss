using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel;

namespace TopSecretSanta.Models
{
    public class SecretSantaContext : DbContext
    {
        public SecretSantaContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Entity<Game>().HasRequired(x => x.Owner).WithMany().Map(a => a.ToTable("UserProfile"));
            //modelBuilder.Entity<Game>().HasOptional(g => g.Players
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public virtual string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }

    public class Game
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date created")]
        public DateTime DateCreated { get; set; }

        [Required]
        [Display(Name = "Deadline")]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        [Display(Name = "Require nicknames")]
        public bool RequireNicknames { get; set; }

        [Display(Name = "Owner")]
        [ForeignKey("OwnerId")]
        public virtual UserProfile Owner { get; set; }

        [Display(Name = "# Players")]
        public virtual int NumberOfPlayers { get { return Players.Count; } }

        [Display(Name = "Players")]
        public virtual ICollection<Player> Players { get; set; }

        [Display(Name = "Invitation")]
        public virtual ICollection<Invitation> Invitations { get; set; }
    }

    public class Player
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [HiddenInput(DisplayValue = false)]
        public int PlayerId { get; set; }

        [Required]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
        public virtual Game Game { get; set; }
    }

    public class Invitation
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required]
        public Guid InvitationId { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Accepted")]
        public bool? Accepted { get; set; }

        public virtual Game Game { get; set; }
    }
}