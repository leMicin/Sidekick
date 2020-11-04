using MediatR;

namespace Sidekick.Domain.Languages.Commands
{
    /// <summary>
    /// Sets the language currently used inside Path of Exile
    /// </summary>
    public class SetLanguageCommand : ICommand
    {
        /// <summary>
        /// Sets the language currently used inside Path of Exile
        /// </summary>
        /// <param name="languageCode">The language to set the parser and game data to. Exemple: en</param>
        public SetLanguageCommand(string languageCode)
        {
            LanguageCode = languageCode;
        }

        /// <summary>
        /// The language to set the parser and game data to. Exemple: en
        /// </summary>
        public string LanguageCode { get; }
    }
}
