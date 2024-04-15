using Azure.AI.OpenAI;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Repositories.Entities;
using ChatGptBot.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data.SqlTypes;
using ContextMessage = ChatGptBot.Repositories.Entities.ContextMessage;

namespace ChatGptBot.Repositories
{
    public interface IConversationRepository
    {
        Task StoreAssistantConversationItem(ConversationItem conversationItem);
        Task StoreUserConversationItem(ConversationItem conversationItem);
        Task <List<ConversationItem>> LoadConversation(Guid conversationId);
    }

    public class ConversationRepository : IConversationRepository, ISingletonScope
    {
        private readonly ChatGptSettings _chatGptSettings;
        private readonly string _cnString;

        public ConversationRepository(IOptions<Storage> storage, IOptions<ChatGptSettings> chatGptSettings)
        {
            _chatGptSettings = chatGptSettings.Value;
            _cnString = storage.Value.ConnectionString;
        }

        public Task StoreUserConversationItem(ConversationItem conversationItem)
        {
            return StoreConversationItem(conversationItem);
        }

        public Task StoreAssistantConversationItem(ConversationItem conversationItem)
        {
            return StoreConversationItem(conversationItem);
        }

        private async Task StoreConversationItem(ConversationItem conversationItem)
        {
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            await CreateConversationHeaderIfRequired(cn, conversationItem);
            var cmd = new SqlCommand($"insert into conversationItems (id,conversationHeaderId,text,chatrole,at,tokens,originaltext,originaltextlanguagecode,userHasChangedTopic) values (@id,@conversationHeaderId, @text,@chatrole,@at, @tokens,@originaltext,@originaltextlanguagecode,@userHasChangedTopic)");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "id",
                Value = conversationItem.Id,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationItem.ConversationId,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "text",
                Value = conversationItem.EnglishText,
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "chatrole",
                Value = conversationItem.ChatRole
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "at",
                Value = conversationItem.At,
            }); 
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "tokens",
                Value = conversationItem.Tokens,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "originaltext",
                Value = conversationItem.EnglishText,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "originaltextlanguagecode",
                Value = "en"
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "userHasChangedTopic",
                Value = false
            });
            cmd.Connection = cn;
            await cmd.ExecuteNonQueryAsync();

            cmd = new SqlCommand($"insert into ConversationItemEmbeddings (id,conversationItemId,proximity,embeddingId) values (@id,@conversationItemId,@proximity,@embeddingId)");
            cmd.Connection = cn;
            var idParam = new SqlParameter
            {
                ParameterName = "id",

            };
            cmd.Parameters.Add(idParam);

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationItemId",
                Value = conversationItem.Id
            });
            var cosineParam = new SqlParameter
            {
                ParameterName = "proximity"

            };
            cmd.Parameters.Add(cosineParam);
            var embeddingIdParam = new SqlParameter
            {
                ParameterName = "embeddingId"

            };
            cmd.Parameters.Add(embeddingIdParam);
            
        }

        private async Task CreateConversationHeaderIfRequired(SqlConnection cn, ConversationItem conversationItem)
        {
            var cmd = new SqlCommand(@"IF NOT EXISTS 
                (SELECT * FROM conversationHeader  
                        WHERE id=@conversationHeaderId)
                BEGIN 
                    INSERT INTO conversationHeader (id, text)
                    VALUES (@conversationHeaderId, @text)
                END");
            cmd.Connection = cn;
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationItem.ConversationId,
            });
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "text",
                Value = conversationItem.EnglishText,
            });
            
            await cmd.ExecuteNonQueryAsync();
        }

        

        

        public async Task<List<ConversationItem>> LoadConversation(Guid conversationId)
        {
            var ret = new List<ConversationItem>();
            await using var cn = new SqlConnection(_cnString);
            await cn.OpenAsync();
            var cmd = new SqlCommand($"select TOP ({_chatGptSettings.MaxConversationHistoryPairsToLoad*2}) id, text,at,chatrole, tokens,originaltext,originaltextlanguagecode,userHasChangedTopic from conversationItems where conversationHeaderId=@conversationHeaderId order by at DESC");
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "conversationHeaderId",
                Value = conversationId,
            });
            cmd.Connection = cn;
            await using var reader = await cmd.ExecuteReaderAsync();
            var provideContext = true;
            while (reader.Read())
            {
                var item = new ConversationItem
                {
                    ConversationId = conversationId,
                    Id = reader.GetGuid(0),
                    EnglishText = reader.GetString(1),
                    At = reader.GetDateTimeOffset(2),
                    ChatRole = reader.GetString(3),
                    Tokens = reader.GetInt32(4),
                    ProvideContext = (reader.GetString(3) == ChatRole.Assistant
                        ? null
                        : provideContext)
                };
                ret.Add(item);
                // scrolling from ost recent to the past conversation items, when i find userChangedContext=true, all next past items are marked as provideContext=False;
                if (reader["userHasChangedTopic"] == DBNull.Value ? false : reader.GetBoolean(7))
                {
                    provideContext = false;
                }

                if (item.ChatRole == ChatRole.User)
                {
                    cmd = new SqlCommand(
                        $"  select e.id, cie.proximity, cie.conversationItemId FROM ConversationItemEmbeddings cie join Embeddings e on cie.embeddingId = e.id where conversationItemId=@conversationItemId");
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "conversationItemId",
                        Value = item.Id,
                    });
                    cmd.Connection = cn;
                    await using var reader2 = await cmd.ExecuteReaderAsync();
                    while (reader2.Read())
                    {
                        item.ContextMessages.Add(new ContextMessage
                        {
                            EmbeddingId = reader2.GetGuid(0),
                            Proximity = reader2.GetFloat(1),
                            RelatedConversationHistoryItem = reader2.GetGuid(2),
                        });
                    }
                }

            }
            return ret.OrderBy(i=> i.At).ToList();
        }
    }


}
