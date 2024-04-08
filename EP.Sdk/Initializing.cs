using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EP.Morph;

namespace EP.Ner
{
    /// <summary>
    /// Инициализация SDK
    /// </summary>
    public static class Sdk
    {
        public static string Version { get { return ProcessorService.Version; } }

        /// <summary>
        /// Вызывать инициализацию в самом начале
        /// </summary>
        /// <param name="lang">по умолчанию, русский и английский</param>
        public static void Initialize(MorphLang lang = default(MorphLang))
        {
            // сначала инициализация всего сервиса
            ProcessorService.Initialize(lang);

            // а затем конкретных анализаторов
            EP.Ner.Money.MoneyAnalyzer.Initialize();
            EP.Ner.Uri.UriAnalyzer.Initialize();
            EP.Ner.Phone.PhoneAnalyzer.Initialize();
            EP.Ner.Date.DateAnalyzer.Initialize();
            EP.Ner.Keyword.KeywordAnalyzer.Initialize();
            EP.Ner.Definition.DefinitionAnalyzer.Initialize();
            EP.Ner.Denomination.DenominationAnalyzer.Initialize();
            EP.Ner.Measure.MeasureAnalyzer.Initialize();
            EP.Ner.Bank.BankAnalyzer.Initialize();
            EP.Ner.Geo.GeoAnalyzer.Initialize();
            EP.Ner.Address.AddressAnalyzer.Initialize();
            EP.Ner.Org.OrganizationAnalyzer.Initialize();
            EP.Ner.Person.PersonAnalyzer.Initialize();
            EP.Ner.Mail.MailAnalyzer.Initialize();
            EP.Ner.Transport.TransportAnalyzer.Initialize();
            EP.Ner.Decree.DecreeAnalyzer.Initialize();
            EP.Ner.Instrument.InstrumentAnalyzer.Initialize();
            EP.Ner.Titlepage.TitlePageAnalyzer.Initialize();
            EP.Ner.Booklink.BookLinkAnalyzer.Initialize();
            EP.Ner.Business.BusinessAnalyzer.Initialize();
            EP.Ner.Goods.GoodsAnalyzer.Initialize();
#if !UNISHARPING
            EP.Ner.Semantic.SemanticAnalyzer.Initialize();
#endif
            EP.Ner.Named.NamedEntityAnalyzer.Initialize();
            EP.Ner.Weapon.WeaponAnalyzer.Initialize();
        }
    }
}
