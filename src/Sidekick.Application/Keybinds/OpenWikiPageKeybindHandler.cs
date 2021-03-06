using System;
using System.Threading.Tasks;
using MediatR;
using Sidekick.Domain.App.Commands;
using Sidekick.Domain.Errors;
using Sidekick.Domain.Game.Items.Commands;
using Sidekick.Domain.Game.Items.Models;
using Sidekick.Domain.Game.Languages.Commands;
using Sidekick.Domain.Keybinds;
using Sidekick.Domain.Platforms;
using Sidekick.Domain.Settings;
using Sidekick.Domain.Views;
using Sidekick.Domain.Wikis;

namespace Sidekick.Application.Keybinds
{
    public class OpenWikiPageKeybindHandler : IKeybindHandler
    {
        private readonly IClipboardProvider clipboardProvider;
        private readonly IMediator mediator;
        private readonly IViewLocator viewLocator;
        private readonly ISidekickSettings settings;
        private readonly IProcessProvider processProvider;

        public OpenWikiPageKeybindHandler(
            IClipboardProvider clipboardProvider,
            IMediator mediator,
            IViewLocator viewLocator,
            ISidekickSettings settings,
            IProcessProvider processProvider)
        {
            this.clipboardProvider = clipboardProvider;
            this.mediator = mediator;
            this.viewLocator = viewLocator;
            this.settings = settings;
            this.processProvider = processProvider;
        }

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute()
        {
            var text = await clipboardProvider.Copy();
            var item = await mediator.Send(new ParseItemCommand(text));

            if (item == null)
            {
                // If the item can't be parsed, show an error
                await viewLocator.Open(View.Error, ErrorType.Unparsable);
                return;
            }

            if (!await mediator.Send(new IsGameLanguageEnglishQuery()))
            {
                // Only available for english language
                await viewLocator.Open(View.Error, ErrorType.UnavailableTranslation);
                return;
            }

            if (string.IsNullOrEmpty(item.Metadata.Name))
            {
                // Most items will open the basetype wiki link.
                // Does not work for unique items that are not identified.
                await viewLocator.Open(View.Error, ErrorType.InvalidItem);
                return;
            }

            if (settings.Wiki_Preferred == WikiSetting.PoeDb)
            {
                await OpenPoeDb(item);
            }
            else
            {
                await OpenPoeWiki(item);
            }
        }

        #region PoeDb
        private const string PoeDb_BaseUri = "https://poedb.tw/";
        private const string PoeDb_SubUrlUnique = "unique.php?n=";
        private const string PoeDb_SubUrlGem = "gem.php?n=";
        private const string PoeDb_SubUrlItem = "item.php?n=";
        private Task OpenPoeDb(Item item)
        {
            var subUrl = item.Metadata.Rarity switch
            {
                Rarity.Unique => PoeDb_SubUrlUnique,
                Rarity.Gem => PoeDb_SubUrlGem,
                _ => PoeDb_SubUrlItem
            };

            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            var wikiLink = subUrl + searchLink.Replace(" ", "+");

            return mediator.Send(new OpenBrowserCommand(new Uri(PoeDb_BaseUri + wikiLink)));
        }
        #endregion

        #region PoeWiki
        private Task OpenPoeWiki(Item item)
        {
            // determine search link, so wiki can be opened for any item
            var searchLink = item.Metadata.Name ?? item.Metadata.Type;
            // replace space encodes with '_' to match the link layout of the poe wiki and then url encode it
            var itemLink = System.Net.WebUtility.UrlEncode(searchLink.Replace(" ", "_"));

            return mediator.Send(new OpenBrowserCommand(new Uri($"https://pathofexile.gamepedia.com/{itemLink}")));
        }
        #endregion
    }
}
