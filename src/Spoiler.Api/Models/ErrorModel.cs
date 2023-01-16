namespace Spoiler.Api.Models
{
    public class ErrorModel
    {
        public string Instance { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public int ErrorCode { get; set; }


        public IDictionary<string, string[]> Errors { get; set; }

        public ErrorModel()
        {
            Errors = new Dictionary<string, string[]>();
        }
    }
}
