using IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IO
{
    public class Csv
    {
        [SerializeField] private string _filePath;
        [SerializeField] private string resourcePath;

        public string filePath => _filePath;
        public string ResourcePath => resourcePath;
        public string FilePath => Path.Combine(CsvReader.BasePath, _filePath);

        [field: SerializeField] public char SplitSymbol { get; private set; }

        protected Csv(string path, char splitSymbol)
        {
            _filePath = path;
            SplitSymbol = splitSymbol;
        }
    }
}