using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;

namespace CommunicationsLib.MsgParser;

public static class MessageJsonSerializer
{
    private static bool EvaluateJsonMessage(string jsonMessage)
    {
        // Path : "./MsgParser/message.schema.json"
        string schemaPath = string.Concat(
            Path.GetFullPath("."),
            Path.DirectorySeparatorChar, "MsgParser",
            Path.DirectorySeparatorChar, "message.schema.json");
        JsonSchema schema = JsonSchema.FromFile(schemaPath);

        try
        {
            return schema.Evaluate(JsonNode.Parse(jsonMessage)).IsValid;    // System.Text.Json.JsonReaderException
        }
        catch (JsonException ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

    public static string JsonSerializeMessage(string messageToSerialize, string myUserID = "1")
    {
        MessageInfo messageInfo = new MessageInfo(myUserID, messageToSerialize);

        return JsonSerializer.Serialize(messageInfo);
    }

    public static MessageInfo? JsonDeserializeMessage(string jsonMessage)
    {
        return EvaluateJsonMessage(jsonMessage) ? JsonSerializer.Deserialize<MessageInfo>(jsonMessage) : null;
    }
}
