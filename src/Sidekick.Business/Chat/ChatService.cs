using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Core.Natives;
using Sidekick.Domain.Settings;

namespace Sidekick.Business.Chat
{
    public class ChatService : IChatService
    {
        private readonly ILogger logger;
        private readonly INativeKeyboard keyboard;
        private readonly INativeClipboard clipboard;
        private readonly ISidekickSettings settings;

        public ChatService(
            ILogger<ChatService> logger,
            INativeKeyboard keyboard,
            INativeClipboard clipboard,
            ISidekickSettings settings)
        {
            this.logger = logger;
            this.keyboard = keyboard;
            this.clipboard = clipboard;
            this.settings = settings;
        }

        public async Task Write(string text)
        {
            string clipboardValue = null;
            if (settings.RetainClipboard)
            {
                clipboardValue = await clipboard.GetText();
            }

            await clipboard.SetText(text);

            keyboard.SendInput("Enter");
            keyboard.SendInput("Ctrl+A");
            keyboard.Paste();
            keyboard.SendInput("Enter");
            keyboard.SendInput("Enter");
            keyboard.SendInput("Up");
            keyboard.SendInput("Up");
            keyboard.SendInput("Esc");

            if (settings.RetainClipboard)
            {
                await Task.Delay(100);
                await clipboard.SetText(clipboardValue);
            }

            logger.LogInformation("ChatService - Wrote '{text}' in the chat", text);
        }

        public async Task StartWriting(string text)
        {
            string clipboardValue = null;
            if (settings.RetainClipboard)
            {
                clipboardValue = await clipboard.GetText();
            }

            await clipboard.SetText(text);

            keyboard.SendInput("Enter");
            keyboard.SendInput("Ctrl+A");
            keyboard.Paste();

            if (settings.RetainClipboard)
            {
                await Task.Delay(100);
                await clipboard.SetText(clipboardValue);
            }

            logger.LogInformation("ChatService - Started writing '{text}' in the chat", text);
        }
    }
}
