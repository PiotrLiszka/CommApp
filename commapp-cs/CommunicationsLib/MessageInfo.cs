namespace CommunicationsLib;

public class MessageInfo
{
    public MessageInfo () { }

    /// <summary>
    /// Constructor used for sending serialized message to end user
    /// </summary>
    /// <param name="id">Hash/ID of a message sender</param>
    /// <param name="message">Body of a message</param>
    public MessageInfo (int id, string message)
    {
        SenderID = id;
        MessageBody = message;
        TimeSend = TimeOnly.FromDateTime(DateTime.Now);
    }

    /// <summary>
    /// Constructor used in deserialization of a message, with message time of a sender
    /// </summary>
    /// <param name="id">Hash/ID of a message sender</param>
    /// <param name="message">Body of a message</param>
    /// <param name="time">Time of sending message</param>
    public MessageInfo (int id, string message, TimeOnly time)
    {
        SenderID = id;
        MessageBody = message;
        TimeSend = time;
    }

    public int SenderID { get; set; }
    public TimeOnly TimeSend { get; set; }
    public string? MessageBody { get; set; }

}
