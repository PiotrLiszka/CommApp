namespace CommunicationsLib.MsgParser;

public static class ImageConverter
{

    public static string ImageToStringConverter(string imgPth)
    {

        // Read the image file into a byte array
        byte[] imageBytes = File.ReadAllBytes(imgPth);

        // Convert the byte array to a Base64 string
        string base64String = Convert.ToBase64String(imageBytes);

        // Output the Base64 string
        return base64String;
    }

    public static void StringToImageConverter(string imgString)
    {
        byte[] bytes = Convert.FromBase64String(imgString);

        // create and flush in temp folder (only jpg for now)
        using (var imgFile = new FileStream(
            String.Concat(Path.GetTempPath(), DateTime.Now.Ticks , ".jpg"),
            FileMode.OpenOrCreate))
        {
            imgFile.WriteAsync(bytes, 0, bytes.Length);
            imgFile.Flush();
        }
    }

}
