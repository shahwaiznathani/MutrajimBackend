using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MutrajimAPI.Models
{
    public class LocaleSetting
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int settingId { get; set; }

        public string SourceLanguage { get; set; }

        public string TargetLanguage { get; set; }

        public string SourceLanguageName { get; set; }

        public string TargetLanguageName { get; set; }
    }
}

