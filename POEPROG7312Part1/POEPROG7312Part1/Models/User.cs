using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace POEPROG7312Part1.Models
{
    public class User
    {
       
            [BsonId]
            public ObjectId Id { get; set; }

            [BsonElement("username")]
            public string Username { get; set; }

            [BsonElement("password")]
            public string Password { get; set; }

            [BsonElement("email")]
            public string Email { get; set; }

            [BsonElement("role")]
             public string Role { get; set; }

    }
    }


