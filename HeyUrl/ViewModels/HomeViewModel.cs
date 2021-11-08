using System.Collections.Generic;
using Entities.Models;

namespace Entities.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Url> Urls { get; set; }
        public Url NewUrl { get; set; }
    }
}
