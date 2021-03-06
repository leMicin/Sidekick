using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sidekick.Application.Game.Items.Parser.Patterns;
using Sidekick.Domain.Cache;
using Sidekick.Domain.Game.Items;
using Sidekick.Domain.Game.Items.Metadatas;
using Sidekick.Domain.Game.Items.Metadatas.Models;
using Sidekick.Domain.Game.Items.Models;
using Sidekick.Domain.Game.Languages;
using Sidekick.Infrastructure.PoeApi.Items.Metadatas.Models;

namespace Sidekick.Infrastructure.PoeApi.Items.Metadatas
{
    public class ItemMetadataProvider : IItemMetadataProvider
    {
        private readonly ICacheRepository cacheRepository;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IParserPatterns parserPatterns;

        public ItemMetadataProvider(ICacheRepository cacheRepository,
            IGameLanguageProvider gameLanguageProvider,
            IPoeTradeClient poeTradeClient,
            IParserPatterns parserPatterns)
        {
            this.cacheRepository = cacheRepository;
            this.gameLanguageProvider = gameLanguageProvider;
            this.poeTradeClient = poeTradeClient;
            this.parserPatterns = parserPatterns;
        }

        private Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; set; }
        private List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; set; }

        private Regex Prefixes { get; set; }
        private string GetLineWithoutPrefixes(string line) => Prefixes.Replace(line, string.Empty);

        public async Task Initialize()
        {
            NameAndTypeDictionary = new Dictionary<string, List<ItemMetadata>>();
            NameAndTypeRegex = new List<(Regex Regex, ItemMetadata Item)>();

            var result = await cacheRepository.GetOrSet(
                "Sidekick.Infrastructure.PoeApi.Items.Metadatas.ItemMetadataProvider.Initialize",
                () => poeTradeClient.Fetch<ApiCategory>("data/items"));

            FillPattern(result.Result[0].Entries, Category.Accessory, useRegex: true);
            FillPattern(result.Result[1].Entries, Category.Armour, useRegex: true);
            FillPattern(result.Result[2].Entries, Category.DivinationCard);
            FillPattern(result.Result[3].Entries, Category.Currency);
            FillPattern(result.Result[4].Entries, Category.Flask, useRegex: true);
            FillPattern(result.Result[5].Entries, Category.Gem);
            FillPattern(result.Result[6].Entries, Category.Jewel, useRegex: true);
            FillPattern(result.Result[7].Entries, Category.Map, useRegex: true);
            FillPattern(result.Result[8].Entries, Category.Weapon, useRegex: true);
            FillPattern(result.Result[9].Entries, Category.Leaguestone);
            FillPattern(result.Result[10].Entries, Category.Prophecy);
            FillPattern(result.Result[11].Entries, Category.ItemisedMonster, useRegex: true);
            FillPattern(result.Result[12].Entries, Category.Watchstone);
            FillPattern(result.Result[14].Entries, Category.Contract, useRegex: true);

            Prefixes = new Regex("^(?:" +
                gameLanguageProvider.Language.PrefixSuperior + " |" +
                gameLanguageProvider.Language.PrefixBlighted + " |" +
                gameLanguageProvider.Language.PrefixAnomalous + " |" +
                gameLanguageProvider.Language.PrefixDivergent + " |" +
                gameLanguageProvider.Language.PrefixPhantasmal + " )");
        }

        private void FillPattern(List<ApiItem> items, Category category, bool useRegex = false)
        {
            foreach (var item in items)
            {
                var header = new ItemMetadata()
                {
                    Name = item.Name,
                    Type = item.Type,
                    Rarity = GetRarityForCategory(category, item),
                    Category = category,
                };

                var key = header.Name ?? header.Type;

                // If the item is unique, exclude it from the regex dictionary
                if (header.Rarity == Rarity.Unique)
                {
                    FillDictionary(header, key);
                    continue;
                }

                if (useRegex)
                {
                    NameAndTypeRegex.Add((new Regex(Regex.Escape(key)), header));
                }

                FillDictionary(header, key);
            }
        }

        private void FillDictionary(ItemMetadata metadata, string key)
        {
            if (!NameAndTypeDictionary.TryGetValue(key, out var dictionaryEntry))
            {
                dictionaryEntry = new List<ItemMetadata>();
                NameAndTypeDictionary.Add(key, dictionaryEntry);
            }
            dictionaryEntry.Add(metadata);
        }

        private Rarity GetRarityForCategory(Category category, ApiItem item)
        {
            if (item.Flags.Unique)
            {
                return Rarity.Unique;
            }
            else if (item.Flags.Prophecy)
            {
                return Rarity.Prophecy;
            }

            return category switch
            {
                Category.DivinationCard => Rarity.DivinationCard,
                Category.Gem => Rarity.Gem,
                Category.Currency => Rarity.Currency,
                _ => Rarity.Unknown
            };
        }

        public ItemMetadata Parse(ParsingItem parsingItem)
        {
            var parsingBlock = parsingItem.Blocks.First();
            var itemRarity = GetRarity(parsingBlock);

            // If we find a Vaal Gem, we don't care about any other results
            if (itemRarity == Rarity.Gem &&
                parsingItem.Blocks.Count > 7 && // If the items has more than 7 blocks, it could be a vaal gem
                NameAndTypeDictionary.TryGetValue(parsingItem.Blocks[5].Lines[0].Text, out var vaalGem)) // The vaal gem name is always at the same position
            {
                parsingBlock.Parsed = true;
                return vaalGem.First();
            }

            // Get name and type text
            string name = null;
            string type = null;
            if (parsingBlock.Lines.Count >= 4)
            {
                name = parsingBlock.Lines[2].Text;
                type = parsingBlock.Lines[3].Text;
            }
            else if (parsingBlock.Lines.Count == 3)
            {
                name = parsingBlock.Lines[2].Text;
            }

            // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
            if (itemRarity == Rarity.Rare)
            {
                name = null;
            }

            // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
            var results = new List<ItemMetadata>();

            // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
            if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(name, out var itemData))
            {
                results.AddRange(itemData);
            }
            // Here we check without any prefixes
            else if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(GetLineWithoutPrefixes(name), out itemData))
            {
                results.AddRange(itemData);
            }
            // Now we check the type
            else if (!string.IsNullOrEmpty(type) && NameAndTypeDictionary.TryGetValue(type, out itemData))
            {
                results.AddRange(itemData);
            }
            // Finally. if we don't have any matches, we will look into our regex dictionary
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    results.AddRange(NameAndTypeRegex
                        .Where(pattern => pattern.Regex.IsMatch(name))
                        .Select(x => x.Item));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    results.AddRange(NameAndTypeRegex
                        .Where(pattern => pattern.Regex.IsMatch(type))
                        .Select(x => x.Item));
                }
            }

            // If we have a matching type, we narrow down our results
            if (results.Any(x => x.Type == type))
            {
                results = results.Where(x => x.Type == type).ToList();
            }

            // If we have a Unique item in our results, we return it
            if (results.Any(x => x.Rarity == Rarity.Unique))
            {
                parsingBlock.Parsed = true;
                return results.First(x => x.Rarity == Rarity.Unique);
            }

            var result = results
                .OrderBy(x => x.Rarity == Rarity.Unknown ? 0 : 1)
                .FirstOrDefault();

            if (result != null)
            {
                parsingBlock.Parsed = true;

                // If we don't have the rarity from the metadata, we set it to the value from the text
                if (result.Rarity == Rarity.Unknown)
                {
                    result.Rarity = itemRarity;
                }
            }

            return result;
        }

        private Rarity GetRarity(ParsingBlock parsingBlock)
        {
            foreach (var pattern in parserPatterns.Rarity)
            {
                if (pattern.Value.IsMatch(parsingBlock.Lines[1].Text))
                {
                    return pattern.Key;
                }
            }
            throw new NotSupportedException("Item rarity is unknown.");
        }
    }
}
