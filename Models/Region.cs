using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Models
{
    [Index(nameof(Nom), IsUnique = true)]
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = "";

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString() => Nom;
    }
}
