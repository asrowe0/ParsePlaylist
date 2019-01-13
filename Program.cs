using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;


namespace ParsePlaylist {

    class ParsePlaylist {
        
        string baseURL = "http://www.youtube.com/watch?v=";

        [STAThread]
        static void Main(string[] args) {
            Console.WriteLine("cosby YouTube API: Parse Playlist");
            Console.WriteLine("=================================");
            Console.Write("\nPlaylist ID: ");
            string id = Console.ReadLine();

            try {
                new ParsePlaylist().Run(id).Wait();
            }
            catch (AggregateException ex) {
                foreach (var e in ex.InnerExceptions)
                    Console.WriteLine("Error: " + e.Message);
            }

            Console.ReadKey();
        }

        private async Task Run(string id) {
            var urls = new List<string>();
            
            var youtubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = "AIzaSyDZgIBIPj8NJFa6UKTgI9euqEUu8kwbHrQ",
                ApplicationName = this.GetType().ToString()
            });

            var nextPageToken = "";
            while (nextPageToken != null) {
                var playlistRequest = youtubeService.PlaylistItems.List("contentDetails");
                playlistRequest.PlaylistId = id;
                playlistRequest.MaxResults = 50;
                playlistRequest.PageToken = nextPageToken;

                var playlistResponse = await playlistRequest.ExecuteAsync();

                foreach (var pItem in playlistResponse.Items) {
                    urls.Add(baseURL + pItem.ContentDetails.VideoId);
                }
                Console.WriteLine("Batch Complete...");
                nextPageToken = playlistResponse.NextPageToken;
            }
            
            System.IO.File.WriteAllLines("/output/urls.txt", urls.ToArray());
            Console.WriteLine("All Batches Complete");           
        }
    }
}
