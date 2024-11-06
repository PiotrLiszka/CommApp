using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using ForWriter = Newtonsoft.Json;

namespace CommunicationsLib.MsgParser;

public static class MessageJson
{
    private static EvaluationResults EvaluateJsonMessage(string jsonMessage)
    {
        // Path : "./MsgParser/message.schema.json"
        string schemaPath = string.Concat(
            Path.GetFullPath("."),
            Path.DirectorySeparatorChar, "MsgParser",
            Path.DirectorySeparatorChar, "message.schema.json");
        JsonSchema schema = JsonSchema.FromFile(schemaPath);

        return schema.Evaluate(JsonNode.Parse(jsonMessage)); 
    }

    public static string JsonSerializeMessage(string messageToSerialize, string myUserID = "1")
    {
        MessageInfo messageInfo = new MessageInfo(myUserID, messageToSerialize);

        return JsonSerializer.Serialize(messageInfo);
    }

    public static MessageInfo? JsonDeserializeMessage(string jsonMessage)
    {
        return EvaluateJsonMessage(jsonMessage).IsValid ? System.Text.Json.JsonSerializer.Deserialize<MessageInfo>(jsonMessage) : null;
    }
}
