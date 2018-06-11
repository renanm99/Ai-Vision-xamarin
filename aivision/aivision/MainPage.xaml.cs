using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using RestSharp;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Forms;
using aivision.Droid;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace aivision
{
    public partial class MainPage : ContentPage
    {
        public static string result { get; set; }
        public string teste { get; set; }

        public MainPage()
        {
            BindingContext = this;

            InitializeComponent();
            StackLayout sl = new StackLayout() { };
            Main(sl);
            Content = sl;
        }

        public void RestCall()
        {

            /*
            string imagefileName = "praia.jpg";
            // Remove the file extention from the image filename
            imagefileName = imagefileName.Replace(".jpg", "").Replace(".png", "");

            // Retrieving the local Resource ID from the name
            int id = (int)typeof(Resource.Drawable).GetField(imagefileName).GetValue(null);

            // Converting Drawable Resource to Bitmap
            //var myImage = BitmapFactory.DecodeResource(Forms.Context.Resources, id);

            Image img = new Image
            {
                Source = "praia.jpg"
            };
            */

            var client = new RestClient("https://brazilsouth.api.cognitive.microsoft.com");

            var request = new RestRequest("vision/v1.0/analyze", Method.POST);

            request.AddHeader("ocp-apim-subscription-key", "79116473a8504002b598f48a8186d1a0");
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("visualFeatures", "Categories,Description");

            request.AddFile("praia", "praia.jpg", "application/octet-stream");

            IRestResponse response = client.Execute(request);
            teste = response.Content;
        }

        const string subscriptionKey = "79116473a8504002b598f48a8186d1a0";
        const string uriBase = "https://brazilsouth.api.cognitive.microsoft.com/vision/v1.0/analyze";

        void Main(StackLayout sl)
        {
            /*
            var assembly = this.GetType().GetTypeInfo().Assembly; // you can replace "this.GetType()" with "typeof(MyType)", where MyType is any type in your assembly.
            byte[] buffer;
            using (Stream s = assembly.GetManifestResourceStream("praia.jpg"))
            {
                long length = s.Length;
                buffer = new byte[length];
                s.Read(buffer, 0, (int)length);
            }
            */

            Image image = new Image();
            image.Source = "praia.jpg";

            sl.Children.Add(image);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, "praia.jpg");

            if (File.Exists(filename))
            {
                MakeAnalysisRequest("").Wait();
            }
            else
            {
                Console.WriteLine("\nInvalid file path");
            }
            Console.WriteLine("\nPress Enter to exit...");
        }


        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. A third optional parameter is "details".
                string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body. Posts a locally stored JPEG image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                Console.WriteLine(contentString);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
