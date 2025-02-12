using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBibliotheque.Blazor.Models
{
    public record UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateOnly PubDate { get; set; }
    }
}