namespace CommandComplete
{
    public class ParameterOption
    {
        public ParameterOption(string name, bool takesInputValue, string helptext)
        {
            Name = name;
            TakesInputValue = takesInputValue;
            HelpText = helptext;
        }

        public string Name { get; }

        /// <summary>
        /// If this command parameter is requires a value to be given instead of just acting like a flag. 
        /// Ex: -name Rick vs -IgnorePeopleNamedRick
        /// </summary>
        public bool TakesInputValue { get; }

        /// <summary>
        /// Human Readable text displayed to help user know what this parameter is meant for
        /// </summary>
        public string HelpText { get; }
    }
}
