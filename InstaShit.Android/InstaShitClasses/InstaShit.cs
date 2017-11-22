using System;
using System.IO;

namespace InstaShitAndroid.InstaShitClasses
{
    public class InstaShit : InstaShitCore.InstaShitCore
    {
        public InstaShit() : base(false)
        {

        }
        protected override string GetFileLocation(string fileName)
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
        }
        protected override void Debug(string text)
        {
            Console.WriteLine(text);
            base.Debug(text);
        }
    }
}