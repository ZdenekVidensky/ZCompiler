using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeImport
{
    public static class FileImport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string OpenSourceFile(string path)
        {
            var result = string.Empty;

            using(var openFile = File.Open(path, FileMode.Open))
            {
                var sr = new StreamReader(openFile);
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }
    }
}
