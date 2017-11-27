using System;
using System.IO;

namespace InstaShitAndroid.InstaShitClasses
{
    public class InstaShit : InstaShitCore.InstaShitCore
    {
        protected override string GetFileLocation(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
        }
        protected override void Debug(string text)
        {
            Console.WriteLine(text);
            base.Debug(text);
        }
    }
}