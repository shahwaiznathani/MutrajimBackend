using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MutrajimAPI.Models
{
    public class KeyValueModel
    {
         [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int KeyID { get; set; }

         //public int KeyCount { get; set; }

         [Column(TypeName = "nvarchar(100)")]
         public string Key { get; set; }

         [Column(TypeName = "nvarchar(100)")]
         public string Value { get; set; }

        public KeyValueModel(int id, string k, string v)
        {
            KeyID = id;
            Key = k;
            Value = v;
        }
        public KeyValueModel()
        {
                
        }
    }
}
