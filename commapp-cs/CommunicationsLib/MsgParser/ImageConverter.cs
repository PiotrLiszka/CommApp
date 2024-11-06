using System.Diagnostics;
using System.Text;

namespace CommunicationsLib.MsgParser;

public static class ImageConverter
{
    private static readonly Dictionary<string, string> extensionDictionary = new()
    {
        {"image/jpeg", ".jpg" }, {"image/bmp", ".bmp"}, {"image/png", ".png"}
    };

    public static string ImageToStringConverter(ref string imgPth)
    {
        // Read the image file into a byte array
        byte[] imageBytes = File.ReadAllBytes(imgPth);
        Debug.WriteLine(imageBytes.Length);

        // Convert the byte array to a Base64 string
        string base64String = Convert.ToBase64String(imageBytes);
        Debug.WriteLine(base64String.Length * sizeof(Char));

        // Output the Base64 string
        return base64String;
    }

    public async static void StringToImageConverter(string imgString, string extension)
    {
        byte[] bytes = Convert.FromBase64String(imgString);
        Debug.WriteLine(bytes.Length);

        // create and flush image file in temp folder
        using (var imgFile = new FileStream(
            String.Concat(Path.GetTempPath(), "myimg_", DateTime.Now.Ticks, extensionDictionary[extension]),
            FileMode.OpenOrCreate))
        {
            await imgFile.WriteAsync(bytes, 0, bytes.Length);
            imgFile.Flush();
            imgFile.Close();
        }
    }

}
