using System.Globalization;
using System.Text.Json.Serialization;

namespace CommunicationsLib.MsgParser;
public class MessageInfo
{
    [JsonIgnore]
    public DateTime TimeSentDT { get; init; }

    [JsonRequired]
    [JsonPropertyName("senderId")]
    public string SenderID { get; init; }

    [JsonRequired]
    [JsonPropertyName("datetime")]
    public string TimeSentStr { get; init; }

    [JsonRequired]
    [JsonPropertyName("message")]
    public string? MessageBody { get; init; }

    /// <summary>
    /// Constructor used for sending serialized message to end user
    /// </summary>
    /// <param name="id">Hash/ID of a message sender</param>
    /// <param name="message">Body of a message</param>
    public MessageInfo(string id, string message)
    {
        SenderID = id;
        MessageBody = message;
        TimeSentStr = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

    }

    /// <summary>
    /// Constructor used in deserialization of a message, with message time of a sender
    /// </summary>
    /// <param name="SenderID">Hash/ID of a message sender</param>
    /// <param name="MessageBody">Body of a message</param>
    /// <param name="time">Time of sending message</param>
    [JsonConstructor]
    public MessageInfo(string SenderID, string MessageBody, string TimeSentStr)
    {
        this.SenderID = SenderID;
        this.MessageBody = MessageBody;
        this.TimeSentStr = TimeSentStr;

        //  Parses the UTC time to current culture
        TimeSentDT = DateTime.Parse(TimeSentStr);
    }

}
