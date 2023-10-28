using BepInEx;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections.ObjectModel;
using System.IO;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils;
using Sirenix.Serialization;
using UnboundLib.Utils.UI;
using UnityEngine;

namespace EasyBakeCardOven
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, ModVersion)]
    [BepInProcess("Rounds.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string ModId = "dev.scyye.rounds.easybake";
        public const string ModName = "Easy Bake Card Oven";
        public const string ModVersion = "1.0.0";

        public static Main instance { get; private set; }

        public static string name = "Test Card";
        public static string description = "This is a test card";
        public static CardInfoStat[] stats = new[] { new CardInfoStat() { amount = "+5%", stat = "health", positive = true } };

        void Awake()
        {
            instance = this;
            Logger.LogInfo("1");    

            var harmony = new HarmonyLib.Harmony(ModId);
            Debug.Log("2");
            harmony.PatchAll();
            Debug.Log("3");
            if (!Directory.Exists(Path.Combine(Paths.GameRootPath, "cards")))
                Directory.CreateDirectory(Path.Combine(Paths.GameRootPath, "cards"));
            Debug.Log("4");

        }

        void Start()
        {
            var card = new EasyBakeCardInfo(name, description, stats);
            SaveCard("lol", card);
            Debug.Log("5");
            BuildBakedCard(card);
        }

        public void BuildBakedCard(EasyBakeCardInfo card)
        {
            CardManager.cards.Add(card.Name.Sanitize(), new Card("Easy Baked", Config.Bind("Cards: Easy Baked", "__Easy Baked__" + card.Name, defaultValue: true), card.cardInfo));
            
            Debug.Log("Added card " + card.Name);
        }

        public void SaveCard(string filename, EasyBakeCardInfo card)
        {
            string path = Path.Combine(Path.Combine(Paths.GameRootPath, "cards"), filename + ".card");
            var bytes = SerializationUtility.SerializeValue(card, DataFormat.JSON);

            File.WriteAllBytes(path, bytes);

            Debug.Log("Saved card to " + path);
        }
    }
    
    public class EasyBakeCardInfo
    {
        public string Name;
        public string Description;
        public CardInfoStat[] Stats;

        public CardInfo cardInfo;
        /*
        void stuff()
        {
            var templateCard = Resources.Load<GameObject>("0 Cards/0. PlainCard").GetComponent<CardInfo>();
            templateCard.allowMultiple = true;

            GameObject gameObject = UnityEngine.Object.Instantiate(templateCard.gameObject, Vector3.up * 100f, Quaternion.identity);

            CardInfo newCardInfo = gameObject.GetComponent<CardInfo>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            T val = gameObject.AddComponent<T>();
            val.isPrefab = true;
            newCardInfo.cardBase = val.GetCardBase();
            newCardInfo.cardStats = val.GetStats() ?? Array.Empty<CardInfoStat>();
            newCardInfo.cardName = val.GetTitle();
            gameObject.gameObject.name = ("__" + val.GetModName() + "__" + val.GetTitle()).Sanitize();
            newCardInfo.cardDestription = val.GetDescription();
            newCardInfo.sourceCard = newCardInfo;
            newCardInfo.rarity = val.GetRarity();
            newCardInfo.colorTheme = val.GetTheme();
            newCardInfo.cardArt = val.GetCardArt();

        }*/

        public EasyBakeCardInfo(string name, string description, CardInfoStat[] stats)
        {
            var templateCard = Resources.Load<GameObject>("0 Cards/0. PlainCard").GetComponent<CardInfo>();
            templateCard.allowMultiple = true;
            GameObject gameObject = UnityEngine.Object.Instantiate(templateCard.gameObject, Vector3.up * 100f, Quaternion.identity);

            UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            gameObject.transform.SetParent(null, worldPositionStays: true);
            CardInfo card = gameObject.GetComponent<CardInfo>();
            var cc = gameObject.AddComponent<CustomCard>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            gameObject.name = "__Easy Baked__" + name;

            card.cardName = name;
            card.cardDestription = description;
            card.cardStats = stats;
            card.cardArt = templateCard.cardArt;
            card.sourceCard = card;

            PhotonNetwork.PrefabPool.RegisterPrefab(gameObject.name, gameObject);

            cardInfo = card;

            Name = name;
            Description = description;
            Stats = stats;
        }
    }

    public class EasyBakeCard : CustomCard
    {
        public EasyBakeCardInfo info { get; set; }
        

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override string GetDescription()
        {
            return info.Description;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }

        protected override CardInfoStat[] GetStats()
        {
            return info.Stats;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }

        protected override string GetTitle()
        {
            return info.Name;
        }
    }
}
