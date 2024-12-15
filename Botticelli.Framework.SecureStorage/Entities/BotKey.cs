using LiteDB;

namespace Botticelli.SecureStorage.Entities
{
    /// <summary>
    ///     Bot keys
    /// </summary>
    public class BotKey
    {
        /// <summary>
        ///     botticelli bot id
        /// </summary>
        [BsonId]
        public required string Id { get; set; }

        /// <summary>
        ///     A key for a concrete messenger
        /// </summary>
        public required string Key { get; set; }
    }
}