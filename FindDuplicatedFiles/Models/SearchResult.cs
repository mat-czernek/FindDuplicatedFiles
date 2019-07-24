using System;
using System.Collections.Generic;
using System.Linq;

namespace FindDuplicatedFiles.Models
{
    public class SearchResult
    {
        public string Hash { get; set; }
        
        public IEnumerable<string> FileList { get; set; }

        public override string ToString()
        {
            return $"{Hash}{Environment.NewLine}{string.Join(Environment.NewLine, FileList)}";
        }
    }
}