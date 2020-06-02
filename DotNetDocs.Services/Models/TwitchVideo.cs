using System;

namespace DotNetDocs.Services.Models
{

    public class TwitchVideo
    {
        public string _id { get; set; }
        public long broadcast_id { get; set; }
        public string broadcast_type { get; set; }
        public Channel channel { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string description_html { get; set; }
        public Fps fps { get; set; }
        public string game { get; set; }
        public string language { get; set; }
        public int length { get; set; }
        public Muted_Segments[] muted_segments { get; set; }
        public Preview preview { get; set; }
        public DateTime published_at { get; set; }
        public Resolutions resolutions { get; set; }
        public string status { get; set; }
        public string tag_list { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string viewable { get; set; }
        public object viewable_at { get; set; }
        public int views { get; set; }
    }

    public class Channel
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
    }

    public class Fps
    {
        public float _1080p { get; set; }
        public float _144p { get; set; }
        public float _240p { get; set; }
        public float _360p { get; set; }
        public float _480p { get; set; }
        public float _720p { get; set; }
    }

    public class Preview
    {
        public string large { get; set; }
        public string medium { get; set; }
        public string small { get; set; }
        public string template { get; set; }
    }

    public class Resolutions
    {
        public string _1080p { get; set; }
        public string _144p { get; set; }
        public string _240p { get; set; }
        public string _360p { get; set; }
        public string _480p { get; set; }
        public string _720p { get; set; }
    }

    public class Thumbnails
    {
        public Thumbnail[] large { get; set; }
        public Thumbnail[] medium { get; set; }
        public Thumbnail[] small { get; set; }
        public Thumbnail[] template { get; set; }
    }

    public class Thumbnail
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Muted_Segments
    {
        public int duration { get; set; }
        public int offset { get; set; }
    }
}
