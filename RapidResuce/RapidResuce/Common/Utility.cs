using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;

namespace RapidResuce.Common
{
    public class Utility
    {
        public static string GetTempPath()
        {
            var tempPath = Path.GetTempPath() + $"Testing\\";
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            return tempPath;
        }
        public static string ExtractYouTubeVideoId(string videoLink)
        {
            string videoId = "";

            if (!string.IsNullOrEmpty(videoLink))
            {
                string pattern = @"(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})";

                Match match = Regex.Match(videoLink, pattern);

                if (match.Success)
                {
                    videoId = match.Groups[1].Value;
                }
            }

            return videoId;
        }
        public static bool IsImage(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension == ".jpg" || extension == ".gif" || extension == ".png" || extension == ".Webp" || extension == ".jpeg";
        }
        public static bool IsVideo(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext == ".mp4" || ext == ".avi" || ext == ".mov" || ext == ".wmv" || ext == ".mkv" || ext == ".webm";
        }
        public async static Task<string> SaveMedia(IWebHostEnvironment environment, IFormFile media, string dirName)
        {
            try
            {
                string directoryPath = Path.Combine(environment.WebRootPath, "Uploads", dirName);
                string fileExtension = Path.GetExtension(media.FileName);
                string uniqueFileName = $"{Path.GetFileNameWithoutExtension(media.FileName)}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                string filePath = Path.Combine(directoryPath, uniqueFileName);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await media.CopyToAsync(fileStream);
                }
                var relativePath= Path.Combine( "Uploads",dirName, uniqueFileName);
                return relativePath;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string Base64Decode(string content) => Encoding.UTF8.GetString(Convert.FromBase64String(content));
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static byte[] Base64DecodeToByte(string content) => Convert.FromBase64String(content);
        public static T DeserializeObject<T>(string content) => JsonConvert.DeserializeObject<T>(content);
        public static string GetFileText(string jsonFilePath) => System.IO.File.ReadAllText(jsonFilePath);
        public static bool IsLetter(string strToCheck) => strToCheck.All(char.IsLetter);
        public static bool IsDigit(string strToCheck) => strToCheck.All(char.IsDigit);
        public static string SerializeObject(object content, bool IgnoreNull = false)
        {
            if (!IgnoreNull)
                return JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
            else
                return JsonConvert.SerializeObject(content,
                            Newtonsoft.Json.Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }
        public static string ToUpperFirst(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
        public static string ParseToJson<T>(T obj)
        {
            var jsonBody = JsonConvert.SerializeObject(obj,
                       Newtonsoft.Json.Formatting.None,
                       new JsonSerializerSettings
                       {
                           NullValueHandling = NullValueHandling.Ignore
                       });

            return jsonBody.Replace("Operator", "operator").Replace("_Operator", "operator");
        }
        public static string ConvertToJson(Dictionary<string, object> list)
        {
            var writer = new Newtonsoft.Json.Linq.JTokenWriter();
            writer.WriteStartObject();
            foreach (KeyValuePair<string, object> item in list)
            {
                writer.WritePropertyName(item.Key);
                writer.WriteValue(item.Value);
            }
            writer.WriteEndObject();

            Newtonsoft.Json.Linq.JObject o = (Newtonsoft.Json.Linq.JObject)writer.Token;

            return o.ToString();
        }
        public static int ToInteger(object obj)
        {
            if (obj is null || obj.ToString() == "")
                return 0;

            try
            {
                if (obj.ToString().Contains("."))
                    obj = obj.ToString().Split('.')[0];

                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }
        public static bool ToBoolean(object obj)
        {
            if (obj is null || obj.ToString() == "")
                return false;

            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }
        public static double ToDouble(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }
        public static double ConvertMetersToMiles(double meters)
        {
            const double metersInOneMile = 1609.344;
            return meters / metersInOneMile;
        }
        public static DateTime? ToDateTime(object obj)
        {
            DateTime result;
            if (DateTime.TryParse(obj?.ToString(), out result))
                return result;
            else
                return null;
        }
    }
}