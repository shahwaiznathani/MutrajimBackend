using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MutrajimAPI.Models
{
    public class Project
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }


        [Column(TypeName = "nvarchar(100)")]
        public string SourceLanguage { get; set; }


        [Column(TypeName = "nvarchar(100)")]
        public string TargetLanguage { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string SourceLanguageName { get; set; }


        [Column(TypeName = "nvarchar(100)")]
        public string TargetLanguageName { get; set; }
    }
}

