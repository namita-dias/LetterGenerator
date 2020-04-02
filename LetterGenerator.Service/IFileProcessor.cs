using System;
using System.Collections.Generic;
using System.Text;

namespace LetterGenerator.Service
{
    public interface IFileProcessor
    {
        bool FileExists(string path);
    }
}
