namespace CommunicationsLib;

public class MessageConverter
{
    // TODO: konwersje i serializacje do tej klasy!!
    // najlepiej jako statyczna

    public static string ImageToStringConverter(string imgPth)
    {

        // Read the image file into a byte array
        byte[] imageBytes = File.ReadAllBytes(imgPth);

        // Convert the byte array to a Base64 string
        string base64String = Convert.ToBase64String(imageBytes);

        // Output the Base64 string
        return base64String;
    }

}
