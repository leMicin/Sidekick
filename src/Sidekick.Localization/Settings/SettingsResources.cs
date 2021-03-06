using Microsoft.Extensions.Localization;

namespace Sidekick.Localization.Settings
{
    public class SettingsResources
    {
        private readonly IStringLocalizer<SettingsResources> resources;

        public SettingsResources(IStringLocalizer<SettingsResources> resources)
        {
            this.resources = resources;
        }

        public string Cancel => resources["Cancel"];
        public string Character_League => resources["Character_League"];
        public string Character_Name => resources["Character_Name"];
        public string Character_Title => resources["Character_Title"];
        public string Chat => resources["Chat"];
        public string Chat_Add => resources["Chat_Add"];
        public string Chat_Exit => resources["Chat_Exit"];
        public string Chat_Hideout => resources["Chat_Hideout"];
        public string Chat_Kick => resources["Chat_Kick"];
        public string Chat_Whisper => resources["Chat_Whisper"];
        public string Chat_Custom => resources["Chat_Custom"];
        public string Chat_Keybind => resources["Chat_Keybind"];
        public string Chat_Command => resources["Chat_Command"];
        public string Chat_Submit => resources["Chat_Submit"];
        public string Chat_Wildcard => resources["Chat_Wildcard"];
        public string Chat_Wildcard_Me_CharacterName => resources["Chat_Wildcard_Me_CharacterName"];
        public string Chat_Wildcard_LastWhisper_CharacterName => resources["Chat_Wildcard_LastWhisper_CharacterName"];
        public string Chat_Commands => resources["Chat_Commands"];
        public string Chat_Commands_Hideout => resources["Chat_Commands_Hideout"];
        public string Chat_Commands_Exit => resources["Chat_Commands_Exit"];
        public string Cheatsheets => resources["Cheatsheets"];
        public string Cheatsheets_Key_Open => resources["Cheatsheets_Key_Open"];
        public string General => resources["General"];
        public string General_RetainClipboard => resources["General_RetainClipboard"];
        public string General_SendCrashReports => resources["General_SendCrashReports"];
        public string General_CopyUserIdToClipboard => resources["General_CopyUserIdToClipboard"];
        public string General_ShowSplashScreen => resources["General_ShowSplashScreen"];
        public string Group_Custom_Keybinds => resources["Group_Custom_Keybinds"];
        public string Group_Keybinds => resources["Group_Keybinds"];
        public string Group_Other => resources["Group_Other"];
        public string Key_Active => resources["Key_Active"];
        public string Key_Close => resources["Key_Close"];
        public string EscapeClosesOverlays => resources["EscapeClosesOverlays"];
        public string Key_Duplicated => resources["Key_Duplicated"];
        public string Key_FindItems => resources["Key_FindItems"];
        public string Key_OpenSettings => resources["Key_OpenSettings"];
        public string Key_Unset => resources["Key_Unset"];
        public string Language_Parser => resources["Language_Parser"];
        public string Language_Title => resources["Language_Title"];
        public string Language_UI => resources["Language_UI"];
        public string Map => resources["Map"];
        public string Map_CloseWithMouse => resources["Map_CloseWithMouse"];
        public string Map_Dangerous => resources["Map_Dangerous"];
        public string Map_Dangerous_Regex => resources["Map_Dangerous_Regex"];
        public string Map_Key_Check => resources["Map_Key_Check"];
        public string Trade => resources["Trade"];
        public string Trade_CloseWithMouse => resources["Trade_CloseWithMouse"];
        public string Trade_Key_Check => resources["Trade_Key_Check"];
        public string Trade_Key_OpenSearch => resources["Trade_Key_OpenSearch"];
        public string Trade_Prediction_Enable => resources["Trade_Prediction_Enable"];
        public string ResetCache => resources["ResetCache"];
        public string Save => resources["Save"];
        public string Stash => resources["Stash"];
        public string Stash_Key_Left => resources["Stash_Key_Left"];
        public string Stash_Key_Right => resources["Stash_Key_Right"];
        public string Title => resources["Title"];
        public string Wiki => resources["Wiki"];
        public string Wiki_Key_Open => resources["Wiki_Key_Open"];
        public string Wiki_Preferred => resources["Wiki_Preferred"];
    }
}
