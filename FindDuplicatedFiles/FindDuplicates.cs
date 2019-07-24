using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using  CommandLine;
using CommandLine.Text;
using FindDuplicatedFiles.Models;

namespace FindDuplicatedFiles
{
    public class FindDuplicates
    {
        private readonly Options _options;

        private readonly bool _isValidParserResult = false;

        private SearchResult[] _results;
        
        public FindDuplicates(IEnumerable<string> parameters)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(parameters);
            
            if (parserResult.Tag == ParserResultType.Parsed) {
                _options = ((Parsed<Options>)parserResult).Value;
                _isValidParserResult = true;
            }
        }

        private bool _isParametersVerificationSucceeded()
        {
            // wrong parameters
            if (!_isValidParserResult)
                return false;
            
            // search directory does not exist
            if (!Directory.Exists(_options.SearchDirectory))
                return false;

            // if outputFile optional parameter is used, check whether the target directory exists
            if(!string.IsNullOrEmpty(_options.OutputFile))
            {
                try
                {
                    var outputFilePath = Path.GetDirectoryName(_options.OutputFile);

                    // output file directory does not exist
                    if (!Directory.Exists(outputFilePath))
                        return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<SearchResult> Search()
        {
            if(!_isParametersVerificationSucceeded())
                return Enumerable.Empty<SearchResult>();
            

            var searchOption = _options.IncludeSubdirectories
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            try
            {
                var allFiles = Directory.EnumerateFiles(_options.SearchDirectory, _options.SearchPattern, searchOption);
                
                var hashedFiles = allFiles.Select(file =>
                {
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        return new
                        {
                            FileName = file,
                            Hash = BitConverter.ToString(SHA1.Create().ComputeHash(fileStream))
                        };
                    }
                }).GroupBy(
                    file => file.Hash,
                    file => file.FileName,
                    (key, group) => new SearchResult() { Hash = key, FileList = group.ToList()}    
                );
                
                _results = hashedFiles.Where(file => file.FileList.Count() > 1).ToArray();
            }
            catch (UnauthorizedAccessException) { }
            
            
            // check whether the output file should be created 
            if (!string.IsNullOrEmpty(_options.OutputFile))
            {
                foreach (var result in _results)
                {
                    try
                    {
                        File.AppendAllText(_options.OutputFile, result.ToString());
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
            }

            return _results;
        }

    }
}