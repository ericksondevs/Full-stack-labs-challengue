using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Url
    {
        public int Id { get; set; }

        [Required]
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }
}
