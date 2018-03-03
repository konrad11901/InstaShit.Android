using System;

namespace InstaShitAndroid.InstaShitClasses
{
    public class InstaShit : InstaShitCore.InstaShitCore
    {
        public InstaShit()
            : base(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        {
            
        }
        protected override void Debug(string text)
        {
            Console.WriteLine(text);
            base.Debug(text);
        }
    }
}