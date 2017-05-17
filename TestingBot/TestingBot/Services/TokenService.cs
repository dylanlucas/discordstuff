using Discord;
using Discord.Commands;
using System;
using System.Linq;
using TestingBot.Entities;

namespace TestingBot.Services
{
    public class TokenService
    {
        public static void AwardTokens(User user, string tokenAmount)
        {
            using (var context = new discordbottestingEntities())
            {
                var userIdAsString = user.Id.ToString();

                int tokens;

                var success = Int32.TryParse(tokenAmount, out tokens);

                if (!success)
                {
                    throw new ApplicationException("Could not parse tokenamount to int");
                }

                var entity = context.tokens.Where(input => input.user_id == userIdAsString).FirstOrDefault();

                if (entity != null)
                {
                    entity.tokens += tokens;
                }
                else
                {
                    context.tokens.Add(new token
                    {
                        tokens = tokens,
                        user_id = userIdAsString,
                        username = user.Name
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
