using CommandLine;

namespace FindDuplicatedFiles.Models
{
    public class Options
    {
        [Option('s', "searchDirectory", Required = true, HelpText = "Full path to the target directory with files to be scanned.")]
        public string SearchDirectory { get; set; }
        
        [Option('p', "searchPattern", Required = false, Default = "*.*" ,HelpText = "Search string to match the names of files in target directory.")]
        public string SearchPattern { get; set; }
        
        [Option('i', "includeSubdirectories", Required = false, Default = false, HelpText = "Indicates whether the subdirectories should also be scanned.")]
        public bool IncludeSubdirectories { get; set; }
        
        [Option('o', "outputFile", Required = false, HelpText = "Full path to the output file with scan results.")]
        public string OutputFile { get; set; }
    }
}