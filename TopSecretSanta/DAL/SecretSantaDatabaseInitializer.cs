using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopSecretSanta.Models;
using System.Data.Entity;
using WebMatrix.WebData;
using System.Web.Security;

namespace TopSecretSanta.DAL
{
    public class SecretSantaDatabaseInitializer : DropCreateDatabaseAlways<SecretSantaContext>
    {
        protected override void Seed(SecretSantaContext context)
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            WebSecurity.CreateUserAndAccount("admin", "admin1", new { Email = "admin@ssanta.com" });
            
            if (!Roles.RoleExists("Administrator")) {
                Roles.CreateRole("Administrator");
            }
            
            Roles.AddUsersToRoles(new string[] { "admin" }, new string[] { "Administrator" });

            WebSecurity.CreateUserAndAccount("john.doe", "123456", new { Email = "johndoe@ssanta.com", FirstName = "John", LastName = "Doe" });
            WebSecurity.CreateUserAndAccount("james.doe", "123456", new { Email = "jamesdoe@ssanta.com", FirstName = "James", LastName = "Doe" });
            WebSecurity.CreateUserAndAccount("jane.doe", "123456", new { Email = "janedoe@ssanta.com", FirstName = "Jane", LastName = "Doe" });
            WebSecurity.CreateUserAndAccount("jack.doe", "123456", new { Email = "jackdoe@ssanta.com", FirstName = "Jack", LastName = "Doe" });

            var game = context.Games.Add(
                new Game() {
                    Title = "My first game",
                    DateCreated = DateTime.Now,
                    Deadline = DateTime.Parse("2013/12/30"),
                    RequireNicknames = true,
                    OwnerId = context.UserProfiles.Where(s => s.UserName ==  "john.doe").Single().UserId
                }
            );
            context.SaveChanges();

            var player1 = new Player() {
                GameId = game.GameId,
                Nickname = "nickname1",
                UserId = context.UserProfiles.Where(s => s.UserName == "john.doe").Single().UserId
            };
            var player2 = new Player() {
                GameId = game.GameId,
                Nickname = "nickname2",
                UserId = context.UserProfiles.Where(s => s.UserName == "james.doe").Single().UserId
            };
            var player3 = new Player() {
                GameId = game.GameId,
                Nickname = "nickname3",
                UserId = context.UserProfiles.Where(s => s.UserName == "jane.doe").Single().UserId
            };

            context.Players.Add(player1);
            context.Players.Add(player2);
            context.Players.Add(player3);

            context.SaveChanges();
        }
    }
}
