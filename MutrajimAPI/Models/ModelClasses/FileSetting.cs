using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MutrajimAPI.Models
{
    public class FileSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fileID { get; set; }

        public string fileFormat { get; set; }

        public string fileLocation { get; set; }

    }
}

