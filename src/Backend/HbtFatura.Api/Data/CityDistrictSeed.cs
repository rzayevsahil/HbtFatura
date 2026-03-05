using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Data;

public static class CityDistrictSeed
{
    public static async Task SeedIfEmptyAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Cities.AnyAsync(ct))
            return;

            // Auto-generated City & District Seed Data

        // Örnek veriler - ADANA
        var adana = new City
        {
            Id = Guid.NewGuid(),
            Code = "1",
            Name = "ADANA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Aladağ" },
                new() { Id = Guid.NewGuid(), Name = "Ceyhan" },
                new() { Id = Guid.NewGuid(), Name = "Feke" },
                new() { Id = Guid.NewGuid(), Name = "Karai̇sali" },
                new() { Id = Guid.NewGuid(), Name = "Karataş" },
                new() { Id = Guid.NewGuid(), Name = "Kozan" },
                new() { Id = Guid.NewGuid(), Name = "Pozanti" },
                new() { Id = Guid.NewGuid(), Name = "Sai̇mbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Sariçam" },
                new() { Id = Guid.NewGuid(), Name = "Seyhan" },
                new() { Id = Guid.NewGuid(), Name = "Tufanbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Yumurtalik" },
                new() { Id = Guid.NewGuid(), Name = "Yüreği̇r" },
                new() { Id = Guid.NewGuid(), Name = "Çukurova" },
                new() { Id = Guid.NewGuid(), Name = "İmamoğlu" }
            }
        };

        // Örnek veriler - ADIYAMAN
        var adiyaman = new City
        {
            Id = Guid.NewGuid(),
            Code = "2",
            Name = "ADIYAMAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Besni̇" },
                new() { Id = Guid.NewGuid(), Name = "Gerger" },
                new() { Id = Guid.NewGuid(), Name = "Gölbaşi" },
                new() { Id = Guid.NewGuid(), Name = "Kahta" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Samsat" },
                new() { Id = Guid.NewGuid(), Name = "Si̇nci̇k" },
                new() { Id = Guid.NewGuid(), Name = "Tut" },
                new() { Id = Guid.NewGuid(), Name = "Çeli̇khan" }
            }
        };

        // Örnek veriler - AFYONKARAHİSAR
        var afyonkarahi̇sar = new City
        {
            Id = Guid.NewGuid(),
            Code = "3",
            Name = "AFYONKARAHİSAR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bayat" },
                new() { Id = Guid.NewGuid(), Name = "Başmakçi" },
                new() { Id = Guid.NewGuid(), Name = "Bolvadi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Dazkiri" },
                new() { Id = Guid.NewGuid(), Name = "Di̇nar" },
                new() { Id = Guid.NewGuid(), Name = "Emi̇rdağ" },
                new() { Id = Guid.NewGuid(), Name = "Evci̇ler" },
                new() { Id = Guid.NewGuid(), Name = "Hocalar" },
                new() { Id = Guid.NewGuid(), Name = "Kizilören" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sandikli" },
                new() { Id = Guid.NewGuid(), Name = "Si̇nanpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Sultandaği" },
                new() { Id = Guid.NewGuid(), Name = "Çay" },
                new() { Id = Guid.NewGuid(), Name = "Çobanlar" },
                new() { Id = Guid.NewGuid(), Name = "İhsani̇ye" },
                new() { Id = Guid.NewGuid(), Name = "İscehi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Şuhut" }
            }
        };

        // Örnek veriler - AKSARAY
        var aksaray = new City
        {
            Id = Guid.NewGuid(),
            Code = "68",
            Name = "AKSARAY",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ağaçören" },
                new() { Id = Guid.NewGuid(), Name = "Eski̇l" },
                new() { Id = Guid.NewGuid(), Name = "Gülağaç" },
                new() { Id = Guid.NewGuid(), Name = "Güzelyurt" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ortaköy" },
                new() { Id = Guid.NewGuid(), Name = "Sariyahşi̇" },
                new() { Id = Guid.NewGuid(), Name = "Sultanhani" }
            }
        };

        // Örnek veriler - AMASYA
        var amasya = new City
        {
            Id = Guid.NewGuid(),
            Code = "5",
            Name = "AMASYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Göynücek" },
                new() { Id = Guid.NewGuid(), Name = "Gümüşhaciköy" },
                new() { Id = Guid.NewGuid(), Name = "Hamamözü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Merzi̇fon" },
                new() { Id = Guid.NewGuid(), Name = "Suluova" },
                new() { Id = Guid.NewGuid(), Name = "Taşova" }
            }
        };

        // Örnek veriler - ANKARA
        var ankara = new City
        {
            Id = Guid.NewGuid(),
            Code = "6",
            Name = "ANKARA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akyurt" },
                new() { Id = Guid.NewGuid(), Name = "Altindağ" },
                new() { Id = Guid.NewGuid(), Name = "Ayaş" },
                new() { Id = Guid.NewGuid(), Name = "Bala" },
                new() { Id = Guid.NewGuid(), Name = "Beypazari" },
                new() { Id = Guid.NewGuid(), Name = "Elmadağ" },
                new() { Id = Guid.NewGuid(), Name = "Eti̇mesgut" },
                new() { Id = Guid.NewGuid(), Name = "Evren" },
                new() { Id = Guid.NewGuid(), Name = "Gölbaşi" },
                new() { Id = Guid.NewGuid(), Name = "Güdül" },
                new() { Id = Guid.NewGuid(), Name = "Haymana" },
                new() { Id = Guid.NewGuid(), Name = "Kahramankazan" },
                new() { Id = Guid.NewGuid(), Name = "Kaleci̇k" },
                new() { Id = Guid.NewGuid(), Name = "Keçi̇ören" },
                new() { Id = Guid.NewGuid(), Name = "Kizilcahamam" },
                new() { Id = Guid.NewGuid(), Name = "Mamak" },
                new() { Id = Guid.NewGuid(), Name = "Nallihan" },
                new() { Id = Guid.NewGuid(), Name = "Polatli" },
                new() { Id = Guid.NewGuid(), Name = "Pursaklar" },
                new() { Id = Guid.NewGuid(), Name = "Si̇ncan" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇mahalle" },
                new() { Id = Guid.NewGuid(), Name = "Çamlidere" },
                new() { Id = Guid.NewGuid(), Name = "Çankaya" },
                new() { Id = Guid.NewGuid(), Name = "Çubuk" },
                new() { Id = Guid.NewGuid(), Name = "Şerefli̇koçhi̇sar" }
            }
        };

        // Örnek veriler - ANTALYA
        var antalya = new City
        {
            Id = Guid.NewGuid(),
            Code = "7",
            Name = "ANTALYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akseki̇" },
                new() { Id = Guid.NewGuid(), Name = "Aksu" },
                new() { Id = Guid.NewGuid(), Name = "Alanya" },
                new() { Id = Guid.NewGuid(), Name = "Demre" },
                new() { Id = Guid.NewGuid(), Name = "Döşemealti" },
                new() { Id = Guid.NewGuid(), Name = "Elmali" },
                new() { Id = Guid.NewGuid(), Name = "Fi̇ni̇ke" },
                new() { Id = Guid.NewGuid(), Name = "Gazi̇paşa" },
                new() { Id = Guid.NewGuid(), Name = "Gündoğmuş" },
                new() { Id = Guid.NewGuid(), Name = "Kaş" },
                new() { Id = Guid.NewGuid(), Name = "Kemer" },
                new() { Id = Guid.NewGuid(), Name = "Kepez" },
                new() { Id = Guid.NewGuid(), Name = "Konyaalti" },
                new() { Id = Guid.NewGuid(), Name = "Korkuteli̇" },
                new() { Id = Guid.NewGuid(), Name = "Kumluca" },
                new() { Id = Guid.NewGuid(), Name = "Manavgat" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muratpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Seri̇k" },
                new() { Id = Guid.NewGuid(), Name = "İbradi" }
            }
        };

        // Örnek veriler - ARDAHAN
        var ardahan = new City
        {
            Id = Guid.NewGuid(),
            Code = "75",
            Name = "ARDAHAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Damal" },
                new() { Id = Guid.NewGuid(), Name = "Göle" },
                new() { Id = Guid.NewGuid(), Name = "Hanak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Posof" },
                new() { Id = Guid.NewGuid(), Name = "Çildir" }
            }
        };

        // Örnek veriler - ARTVİN
        var artvi̇n = new City
        {
            Id = Guid.NewGuid(),
            Code = "8",
            Name = "ARTVİN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ardanuç" },
                new() { Id = Guid.NewGuid(), Name = "Arhavi̇" },
                new() { Id = Guid.NewGuid(), Name = "Borçka" },
                new() { Id = Guid.NewGuid(), Name = "Hopa" },
                new() { Id = Guid.NewGuid(), Name = "Kemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Murgul" },
                new() { Id = Guid.NewGuid(), Name = "Yusufeli̇" },
                new() { Id = Guid.NewGuid(), Name = "Şavşat" }
            }
        };

        // Örnek veriler - AYDIN
        var aydin = new City
        {
            Id = Guid.NewGuid(),
            Code = "9",
            Name = "AYDIN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bozdoğan" },
                new() { Id = Guid.NewGuid(), Name = "Buharkent" },
                new() { Id = Guid.NewGuid(), Name = "Di̇di̇m" },
                new() { Id = Guid.NewGuid(), Name = "Efeler" },
                new() { Id = Guid.NewGuid(), Name = "Germenci̇k" },
                new() { Id = Guid.NewGuid(), Name = "Karacasu" },
                new() { Id = Guid.NewGuid(), Name = "Karpuzlu" },
                new() { Id = Guid.NewGuid(), Name = "Koçarli" },
                new() { Id = Guid.NewGuid(), Name = "Kuyucak" },
                new() { Id = Guid.NewGuid(), Name = "Kuşadasi" },
                new() { Id = Guid.NewGuid(), Name = "Köşk" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nazi̇lli̇" },
                new() { Id = Guid.NewGuid(), Name = "Sultanhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Söke" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇pazar" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇ne" },
                new() { Id = Guid.NewGuid(), Name = "İnci̇rli̇ova" }
            }
        };

        // Örnek veriler - AĞRI
        var agri = new City
        {
            Id = Guid.NewGuid(),
            Code = "4",
            Name = "AĞRI",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Di̇yadi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Doğubayazit" },
                new() { Id = Guid.NewGuid(), Name = "Eleşki̇rt" },
                new() { Id = Guid.NewGuid(), Name = "Hamur" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Patnos" },
                new() { Id = Guid.NewGuid(), Name = "Taşliçay" },
                new() { Id = Guid.NewGuid(), Name = "Tutak" }
            }
        };

        // Örnek veriler - BALIKESİR
        var balikesi̇r = new City
        {
            Id = Guid.NewGuid(),
            Code = "10",
            Name = "BALIKESİR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altieylül" },
                new() { Id = Guid.NewGuid(), Name = "Ayvalik" },
                new() { Id = Guid.NewGuid(), Name = "Balya" },
                new() { Id = Guid.NewGuid(), Name = "Bandirma" },
                new() { Id = Guid.NewGuid(), Name = "Bi̇gadi̇ç" },
                new() { Id = Guid.NewGuid(), Name = "Burhani̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Dursunbey" },
                new() { Id = Guid.NewGuid(), Name = "Edremi̇t" },
                new() { Id = Guid.NewGuid(), Name = "Erdek" },
                new() { Id = Guid.NewGuid(), Name = "Gömeç" },
                new() { Id = Guid.NewGuid(), Name = "Gönen" },
                new() { Id = Guid.NewGuid(), Name = "Havran" },
                new() { Id = Guid.NewGuid(), Name = "Karesi̇" },
                new() { Id = Guid.NewGuid(), Name = "Kepsut" },
                new() { Id = Guid.NewGuid(), Name = "Manyas" },
                new() { Id = Guid.NewGuid(), Name = "Marmara" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Savaştepe" },
                new() { Id = Guid.NewGuid(), Name = "Sindirgi" },
                new() { Id = Guid.NewGuid(), Name = "Susurluk" },
                new() { Id = Guid.NewGuid(), Name = "İvri̇ndi̇" }
            }
        };

        // Örnek veriler - BARTIN
        var bartin = new City
        {
            Id = Guid.NewGuid(),
            Code = "74",
            Name = "BARTIN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Amasra" },
                new() { Id = Guid.NewGuid(), Name = "Kurucaşi̇le" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ulus" }
            }
        };

        // Örnek veriler - BATMAN
        var batman = new City
        {
            Id = Guid.NewGuid(),
            Code = "72",
            Name = "BATMAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Beşi̇ri̇" },
                new() { Id = Guid.NewGuid(), Name = "Gercüş" },
                new() { Id = Guid.NewGuid(), Name = "Hasankeyf" },
                new() { Id = Guid.NewGuid(), Name = "Kozluk" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sason" }
            }
        };

        // Örnek veriler - BAYBURT
        var bayburt = new City
        {
            Id = Guid.NewGuid(),
            Code = "69",
            Name = "BAYBURT",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Aydintepe" },
                new() { Id = Guid.NewGuid(), Name = "Demi̇rözü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" }
            }
        };

        // Örnek veriler - BOLU
        var bolu = new City
        {
            Id = Guid.NewGuid(),
            Code = "14",
            Name = "BOLU",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Dörtdi̇van" },
                new() { Id = Guid.NewGuid(), Name = "Gerede" },
                new() { Id = Guid.NewGuid(), Name = "Göynük" },
                new() { Id = Guid.NewGuid(), Name = "Kibriscik" },
                new() { Id = Guid.NewGuid(), Name = "Mengen" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mudurnu" },
                new() { Id = Guid.NewGuid(), Name = "Seben" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇çağa" }
            }
        };

        // Örnek veriler - BURDUR
        var burdur = new City
        {
            Id = Guid.NewGuid(),
            Code = "15",
            Name = "BURDUR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altinyayla" },
                new() { Id = Guid.NewGuid(), Name = "Ağlasun" },
                new() { Id = Guid.NewGuid(), Name = "Bucak" },
                new() { Id = Guid.NewGuid(), Name = "Gölhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Karamanli" },
                new() { Id = Guid.NewGuid(), Name = "Kemer" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Tefenni̇" },
                new() { Id = Guid.NewGuid(), Name = "Yeşi̇lova" },
                new() { Id = Guid.NewGuid(), Name = "Çavdir" },
                new() { Id = Guid.NewGuid(), Name = "Çelti̇kçi̇" }
            }
        };

        // Örnek veriler - BURSA
        var bursa = new City
        {
            Id = Guid.NewGuid(),
            Code = "16",
            Name = "BURSA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Büyükorhan" },
                new() { Id = Guid.NewGuid(), Name = "Gemli̇k" },
                new() { Id = Guid.NewGuid(), Name = "Gürsu" },
                new() { Id = Guid.NewGuid(), Name = "Harmancik" },
                new() { Id = Guid.NewGuid(), Name = "Karacabey" },
                new() { Id = Guid.NewGuid(), Name = "Keles" },
                new() { Id = Guid.NewGuid(), Name = "Kestel" },
                new() { Id = Guid.NewGuid(), Name = "Mudanya" },
                new() { Id = Guid.NewGuid(), Name = "Mustafakemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Ni̇lüfer" },
                new() { Id = Guid.NewGuid(), Name = "Orhaneli̇" },
                new() { Id = Guid.NewGuid(), Name = "Orhangazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Osmangazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇şehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Yildirim" },
                new() { Id = Guid.NewGuid(), Name = "İnegöl" },
                new() { Id = Guid.NewGuid(), Name = "İzni̇k" }
            }
        };

        // Örnek veriler - BİLECİK
        var bi̇leci̇k = new City
        {
            Id = Guid.NewGuid(),
            Code = "11",
            Name = "BİLECİK",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bozüyük" },
                new() { Id = Guid.NewGuid(), Name = "Gölpazari" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Osmaneli̇" },
                new() { Id = Guid.NewGuid(), Name = "Pazaryeri̇" },
                new() { Id = Guid.NewGuid(), Name = "Söğüt" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇pazar" },
                new() { Id = Guid.NewGuid(), Name = "İnhi̇sar" }
            }
        };

        // Örnek veriler - BİNGÖL
        var bi̇ngol = new City
        {
            Id = Guid.NewGuid(),
            Code = "12",
            Name = "BİNGÖL",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Adakli" },
                new() { Id = Guid.NewGuid(), Name = "Genç" },
                new() { Id = Guid.NewGuid(), Name = "Karliova" },
                new() { Id = Guid.NewGuid(), Name = "Ki̇ği" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Solhan" },
                new() { Id = Guid.NewGuid(), Name = "Yayladere" },
                new() { Id = Guid.NewGuid(), Name = "Yedi̇su" }
            }
        };

        // Örnek veriler - BİTLİS
        var bi̇tli̇s = new City
        {
            Id = Guid.NewGuid(),
            Code = "13",
            Name = "BİTLİS",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Adi̇lcevaz" },
                new() { Id = Guid.NewGuid(), Name = "Ahlat" },
                new() { Id = Guid.NewGuid(), Name = "Güroymak" },
                new() { Id = Guid.NewGuid(), Name = "Hi̇zan" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mutki̇" },
                new() { Id = Guid.NewGuid(), Name = "Tatvan" }
            }
        };

        // Örnek veriler - DENİZLİ
        var deni̇zli̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "20",
            Name = "DENİZLİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Acipayam" },
                new() { Id = Guid.NewGuid(), Name = "Babadağ" },
                new() { Id = Guid.NewGuid(), Name = "Baklan" },
                new() { Id = Guid.NewGuid(), Name = "Beki̇lli̇" },
                new() { Id = Guid.NewGuid(), Name = "Beyağaç" },
                new() { Id = Guid.NewGuid(), Name = "Bozkurt" },
                new() { Id = Guid.NewGuid(), Name = "Buldan" },
                new() { Id = Guid.NewGuid(), Name = "Güney" },
                new() { Id = Guid.NewGuid(), Name = "Honaz" },
                new() { Id = Guid.NewGuid(), Name = "Kale" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Merkezefendi̇" },
                new() { Id = Guid.NewGuid(), Name = "Pamukkale" },
                new() { Id = Guid.NewGuid(), Name = "Sarayköy" },
                new() { Id = Guid.NewGuid(), Name = "Seri̇nhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Tavas" },
                new() { Id = Guid.NewGuid(), Name = "Çal" },
                new() { Id = Guid.NewGuid(), Name = "Çameli̇" },
                new() { Id = Guid.NewGuid(), Name = "Çardak" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇vri̇l" }
            }
        };

        // Örnek veriler - DÜZCE
        var duzce = new City
        {
            Id = Guid.NewGuid(),
            Code = "81",
            Name = "DÜZCE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akçakoca" },
                new() { Id = Guid.NewGuid(), Name = "Cumayeri̇" },
                new() { Id = Guid.NewGuid(), Name = "Gölyaka" },
                new() { Id = Guid.NewGuid(), Name = "Gümüşova" },
                new() { Id = Guid.NewGuid(), Name = "Kaynaşli" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Yiğilca" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇li̇mli̇" }
            }
        };

        // Örnek veriler - DİYARBAKIR
        var di̇yarbakir = new City
        {
            Id = Guid.NewGuid(),
            Code = "21",
            Name = "DİYARBAKIR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bağlar" },
                new() { Id = Guid.NewGuid(), Name = "Bi̇smi̇l" },
                new() { Id = Guid.NewGuid(), Name = "Di̇cle" },
                new() { Id = Guid.NewGuid(), Name = "Ergani̇" },
                new() { Id = Guid.NewGuid(), Name = "Eği̇l" },
                new() { Id = Guid.NewGuid(), Name = "Hani̇" },
                new() { Id = Guid.NewGuid(), Name = "Hazro" },
                new() { Id = Guid.NewGuid(), Name = "Kayapinar" },
                new() { Id = Guid.NewGuid(), Name = "Kocaköy" },
                new() { Id = Guid.NewGuid(), Name = "Kulp" },
                new() { Id = Guid.NewGuid(), Name = "Li̇ce" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Si̇lvan" },
                new() { Id = Guid.NewGuid(), Name = "Sur" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇şehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Çermi̇k" },
                new() { Id = Guid.NewGuid(), Name = "Çinar" },
                new() { Id = Guid.NewGuid(), Name = "Çüngüş" }
            }
        };

        // Örnek veriler - EDİRNE
        var edi̇rne = new City
        {
            Id = Guid.NewGuid(),
            Code = "22",
            Name = "EDİRNE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Enez" },
                new() { Id = Guid.NewGuid(), Name = "Havsa" },
                new() { Id = Guid.NewGuid(), Name = "Keşan" },
                new() { Id = Guid.NewGuid(), Name = "Lalapaşa" },
                new() { Id = Guid.NewGuid(), Name = "Meri̇ç" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Süloğlu" },
                new() { Id = Guid.NewGuid(), Name = "Uzunköprü" },
                new() { Id = Guid.NewGuid(), Name = "İpsala" }
            }
        };

        // Örnek veriler - ELAZIĞ
        var elazig = new City
        {
            Id = Guid.NewGuid(),
            Code = "23",
            Name = "ELAZIĞ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Alacakaya" },
                new() { Id = Guid.NewGuid(), Name = "Aricak" },
                new() { Id = Guid.NewGuid(), Name = "Ağin" },
                new() { Id = Guid.NewGuid(), Name = "Baski̇l" },
                new() { Id = Guid.NewGuid(), Name = "Karakoçan" },
                new() { Id = Guid.NewGuid(), Name = "Keban" },
                new() { Id = Guid.NewGuid(), Name = "Kovancilar" },
                new() { Id = Guid.NewGuid(), Name = "Maden" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Palu" },
                new() { Id = Guid.NewGuid(), Name = "Si̇vri̇ce" }
            }
        };

        // Örnek veriler - ERZURUM
        var erzurum = new City
        {
            Id = Guid.NewGuid(),
            Code = "25",
            Name = "ERZURUM",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Azi̇zi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Aşkale" },
                new() { Id = Guid.NewGuid(), Name = "Hinis" },
                new() { Id = Guid.NewGuid(), Name = "Horasan" },
                new() { Id = Guid.NewGuid(), Name = "Karayazi" },
                new() { Id = Guid.NewGuid(), Name = "Karaçoban" },
                new() { Id = Guid.NewGuid(), Name = "Köprüköy" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Narman" },
                new() { Id = Guid.NewGuid(), Name = "Oltu" },
                new() { Id = Guid.NewGuid(), Name = "Olur" },
                new() { Id = Guid.NewGuid(), Name = "Palandöken" },
                new() { Id = Guid.NewGuid(), Name = "Pasi̇nler" },
                new() { Id = Guid.NewGuid(), Name = "Pazaryolu" },
                new() { Id = Guid.NewGuid(), Name = "Tekman" },
                new() { Id = Guid.NewGuid(), Name = "Tortum" },
                new() { Id = Guid.NewGuid(), Name = "Uzundere" },
                new() { Id = Guid.NewGuid(), Name = "Yakuti̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Çat" },
                new() { Id = Guid.NewGuid(), Name = "İspi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Şenkaya" }
            }
        };

        // Örnek veriler - ERZİNCAN
        var erzi̇ncan = new City
        {
            Id = Guid.NewGuid(),
            Code = "24",
            Name = "ERZİNCAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Kemah" },
                new() { Id = Guid.NewGuid(), Name = "Kemali̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Otlukbeli̇" },
                new() { Id = Guid.NewGuid(), Name = "Refahi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Tercan" },
                new() { Id = Guid.NewGuid(), Name = "Çayirli" },
                new() { Id = Guid.NewGuid(), Name = "Üzümlü" },
                new() { Id = Guid.NewGuid(), Name = "İli̇ç" }
            }
        };

        // Örnek veriler - ESKİŞEHİR
        var eski̇sehi̇r = new City
        {
            Id = Guid.NewGuid(),
            Code = "26",
            Name = "ESKİŞEHİR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Alpu" },
                new() { Id = Guid.NewGuid(), Name = "Beyli̇kova" },
                new() { Id = Guid.NewGuid(), Name = "Günyüzü" },
                new() { Id = Guid.NewGuid(), Name = "Han" },
                new() { Id = Guid.NewGuid(), Name = "Mahmudi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mi̇halgazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Mi̇haliççik" },
                new() { Id = Guid.NewGuid(), Name = "Odunpazari" },
                new() { Id = Guid.NewGuid(), Name = "Saricakaya" },
                new() { Id = Guid.NewGuid(), Name = "Seyi̇tgazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Si̇vri̇hi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Tepebaşi" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇fteler" },
                new() { Id = Guid.NewGuid(), Name = "İnönü" }
            }
        };

        // Örnek veriler - GAZİANTEP
        var gazi̇antep = new City
        {
            Id = Guid.NewGuid(),
            Code = "27",
            Name = "GAZİANTEP",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Araban" },
                new() { Id = Guid.NewGuid(), Name = "Karkamiş" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ni̇zi̇p" },
                new() { Id = Guid.NewGuid(), Name = "Nurdaği" },
                new() { Id = Guid.NewGuid(), Name = "Oğuzeli̇" },
                new() { Id = Guid.NewGuid(), Name = "Yavuzeli̇" },
                new() { Id = Guid.NewGuid(), Name = "İslahi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Şahi̇nbey" },
                new() { Id = Guid.NewGuid(), Name = "Şehi̇tkami̇l" }
            }
        };

        // Örnek veriler - GÜMÜŞHANE
        var gumushane = new City
        {
            Id = Guid.NewGuid(),
            Code = "29",
            Name = "GÜMÜŞHANE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Kelki̇t" },
                new() { Id = Guid.NewGuid(), Name = "Köse" },
                new() { Id = Guid.NewGuid(), Name = "Kürtün" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Torul" },
                new() { Id = Guid.NewGuid(), Name = "Şi̇ran" }
            }
        };

        // Örnek veriler - GİRESUN
        var gi̇resun = new City
        {
            Id = Guid.NewGuid(),
            Code = "28",
            Name = "GİRESUN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Alucra" },
                new() { Id = Guid.NewGuid(), Name = "Bulancak" },
                new() { Id = Guid.NewGuid(), Name = "Dereli̇" },
                new() { Id = Guid.NewGuid(), Name = "Doğankent" },
                new() { Id = Guid.NewGuid(), Name = "Espi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Eynesi̇l" },
                new() { Id = Guid.NewGuid(), Name = "Görele" },
                new() { Id = Guid.NewGuid(), Name = "Güce" },
                new() { Id = Guid.NewGuid(), Name = "Keşap" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pi̇razi̇z" },
                new() { Id = Guid.NewGuid(), Name = "Ti̇rebolu" },
                new() { Id = Guid.NewGuid(), Name = "Yağlidere" },
                new() { Id = Guid.NewGuid(), Name = "Çamoluk" },
                new() { Id = Guid.NewGuid(), Name = "Çanakçi" },
                new() { Id = Guid.NewGuid(), Name = "Şebi̇nkarahi̇sar" }
            }
        };

        // Örnek veriler - HAKKARİ
        var hakkari̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "30",
            Name = "HAKKARİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Yüksekova" },
                new() { Id = Guid.NewGuid(), Name = "Çukurca" },
                new() { Id = Guid.NewGuid(), Name = "Şemdi̇nli̇" }
            }
        };

        // Örnek veriler - HATAY
        var hatay = new City
        {
            Id = Guid.NewGuid(),
            Code = "31",
            Name = "HATAY",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altinözü" },
                new() { Id = Guid.NewGuid(), Name = "Antakya" },
                new() { Id = Guid.NewGuid(), Name = "Arsuz" },
                new() { Id = Guid.NewGuid(), Name = "Belen" },
                new() { Id = Guid.NewGuid(), Name = "Defne" },
                new() { Id = Guid.NewGuid(), Name = "Dörtyol" },
                new() { Id = Guid.NewGuid(), Name = "Erzi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Hassa" },
                new() { Id = Guid.NewGuid(), Name = "Kirikhan" },
                new() { Id = Guid.NewGuid(), Name = "Kumlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Payas" },
                new() { Id = Guid.NewGuid(), Name = "Reyhanli" },
                new() { Id = Guid.NewGuid(), Name = "Samandağ" },
                new() { Id = Guid.NewGuid(), Name = "Yayladaği" },
                new() { Id = Guid.NewGuid(), Name = "İskenderun" }
            }
        };

        // Örnek veriler - ISPARTA
        var isparta = new City
        {
            Id = Guid.NewGuid(),
            Code = "32",
            Name = "ISPARTA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Aksu" },
                new() { Id = Guid.NewGuid(), Name = "Atabey" },
                new() { Id = Guid.NewGuid(), Name = "Eği̇rdi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Gelendost" },
                new() { Id = Guid.NewGuid(), Name = "Gönen" },
                new() { Id = Guid.NewGuid(), Name = "Keçi̇borlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Seni̇rkent" },
                new() { Id = Guid.NewGuid(), Name = "Sütçüler" },
                new() { Id = Guid.NewGuid(), Name = "Uluborlu" },
                new() { Id = Guid.NewGuid(), Name = "Yalvaç" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇şarbademli̇" },
                new() { Id = Guid.NewGuid(), Name = "Şarki̇karaağaç" }
            }
        };

        // Örnek veriler - IĞDIR
        var igdir = new City
        {
            Id = Guid.NewGuid(),
            Code = "76",
            Name = "IĞDIR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Aralik" },
                new() { Id = Guid.NewGuid(), Name = "Karakoyunlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Tuzluca" }
            }
        };

        // Örnek veriler - KAHRAMANMARAŞ
        var kahramanmaras = new City
        {
            Id = Guid.NewGuid(),
            Code = "46",
            Name = "KAHRAMANMARAŞ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Afşi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Andirin" },
                new() { Id = Guid.NewGuid(), Name = "Dulkadi̇roğlu" },
                new() { Id = Guid.NewGuid(), Name = "Eki̇nözü" },
                new() { Id = Guid.NewGuid(), Name = "Elbi̇stan" },
                new() { Id = Guid.NewGuid(), Name = "Göksun" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nurhak" },
                new() { Id = Guid.NewGuid(), Name = "Oni̇ki̇şubat" },
                new() { Id = Guid.NewGuid(), Name = "Pazarcik" },
                new() { Id = Guid.NewGuid(), Name = "Türkoğlu" },
                new() { Id = Guid.NewGuid(), Name = "Çağlayanceri̇t" }
            }
        };

        // Örnek veriler - KARABÜK
        var karabuk = new City
        {
            Id = Guid.NewGuid(),
            Code = "78",
            Name = "KARABÜK",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Eflani̇" },
                new() { Id = Guid.NewGuid(), Name = "Eski̇pazar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ovacik" },
                new() { Id = Guid.NewGuid(), Name = "Safranbolu" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇ce" }
            }
        };

        // Örnek veriler - KARAMAN
        var karaman = new City
        {
            Id = Guid.NewGuid(),
            Code = "70",
            Name = "KARAMAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ayranci" },
                new() { Id = Guid.NewGuid(), Name = "Başyayla" },
                new() { Id = Guid.NewGuid(), Name = "Ermenek" },
                new() { Id = Guid.NewGuid(), Name = "Kazimkarabeki̇r" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sariveli̇ler" }
            }
        };

        // Örnek veriler - KARS
        var kars = new City
        {
            Id = Guid.NewGuid(),
            Code = "36",
            Name = "KARS",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akyaka" },
                new() { Id = Guid.NewGuid(), Name = "Arpaçay" },
                new() { Id = Guid.NewGuid(), Name = "Di̇gor" },
                new() { Id = Guid.NewGuid(), Name = "Kağizman" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sarikamiş" },
                new() { Id = Guid.NewGuid(), Name = "Seli̇m" },
                new() { Id = Guid.NewGuid(), Name = "Susuz" }
            }
        };

        // Örnek veriler - KASTAMONU
        var kastamonu = new City
        {
            Id = Guid.NewGuid(),
            Code = "37",
            Name = "KASTAMONU",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Abana" },
                new() { Id = Guid.NewGuid(), Name = "Araç" },
                new() { Id = Guid.NewGuid(), Name = "Azdavay" },
                new() { Id = Guid.NewGuid(), Name = "Ağli" },
                new() { Id = Guid.NewGuid(), Name = "Bozkurt" },
                new() { Id = Guid.NewGuid(), Name = "Ci̇de" },
                new() { Id = Guid.NewGuid(), Name = "Daday" },
                new() { Id = Guid.NewGuid(), Name = "Devrekani̇" },
                new() { Id = Guid.NewGuid(), Name = "Doğanyurt" },
                new() { Id = Guid.NewGuid(), Name = "Hanönü" },
                new() { Id = Guid.NewGuid(), Name = "Küre" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pinarbaşi" },
                new() { Id = Guid.NewGuid(), Name = "Seydi̇ler" },
                new() { Id = Guid.NewGuid(), Name = "Taşköprü" },
                new() { Id = Guid.NewGuid(), Name = "Tosya" },
                new() { Id = Guid.NewGuid(), Name = "Çatalzeyti̇n" },
                new() { Id = Guid.NewGuid(), Name = "İhsangazi̇" },
                new() { Id = Guid.NewGuid(), Name = "İnebolu" },
                new() { Id = Guid.NewGuid(), Name = "Şenpazar" }
            }
        };

        // Örnek veriler - KAYSERİ
        var kayseri̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "38",
            Name = "KAYSERİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akkişla" },
                new() { Id = Guid.NewGuid(), Name = "Bünyan" },
                new() { Id = Guid.NewGuid(), Name = "Develi̇" },
                new() { Id = Guid.NewGuid(), Name = "Felahi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Hacilar" },
                new() { Id = Guid.NewGuid(), Name = "Kocasi̇nan" },
                new() { Id = Guid.NewGuid(), Name = "Meli̇kgazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Pinarbaşi" },
                new() { Id = Guid.NewGuid(), Name = "Sarioğlan" },
                new() { Id = Guid.NewGuid(), Name = "Sariz" },
                new() { Id = Guid.NewGuid(), Name = "Talas" },
                new() { Id = Guid.NewGuid(), Name = "Tomarza" },
                new() { Id = Guid.NewGuid(), Name = "Yahyali" },
                new() { Id = Guid.NewGuid(), Name = "Yeşi̇lhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Özvatan" },
                new() { Id = Guid.NewGuid(), Name = "İncesu" }
            }
        };

        // Örnek veriler - KIRIKKALE
        var kirikkale = new City
        {
            Id = Guid.NewGuid(),
            Code = "71",
            Name = "KIRIKKALE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bahşi̇li̇" },
                new() { Id = Guid.NewGuid(), Name = "Balişeyh" },
                new() { Id = Guid.NewGuid(), Name = "Deli̇ce" },
                new() { Id = Guid.NewGuid(), Name = "Karakeçi̇li̇" },
                new() { Id = Guid.NewGuid(), Name = "Keski̇n" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sulakyurt" },
                new() { Id = Guid.NewGuid(), Name = "Yahşi̇han" },
                new() { Id = Guid.NewGuid(), Name = "Çelebi̇" }
            }
        };

        // Örnek veriler - KIRKLARELİ
        var kirklareli̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "39",
            Name = "KIRKLARELİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Babaeski̇" },
                new() { Id = Guid.NewGuid(), Name = "Demi̇rköy" },
                new() { Id = Guid.NewGuid(), Name = "Kofçaz" },
                new() { Id = Guid.NewGuid(), Name = "Lüleburgaz" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pehli̇vanköy" },
                new() { Id = Guid.NewGuid(), Name = "Pinarhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Vi̇ze" }
            }
        };

        // Örnek veriler - KIRŞEHİR
        var kirsehi̇r = new City
        {
            Id = Guid.NewGuid(),
            Code = "40",
            Name = "KIRŞEHİR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akpinar" },
                new() { Id = Guid.NewGuid(), Name = "Akçakent" },
                new() { Id = Guid.NewGuid(), Name = "Boztepe" },
                new() { Id = Guid.NewGuid(), Name = "Kaman" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mucur" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇çekdaği" }
            }
        };

        // Örnek veriler - KOCAELİ
        var kocaeli̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "41",
            Name = "KOCAELİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Başi̇skele" },
                new() { Id = Guid.NewGuid(), Name = "Darica" },
                new() { Id = Guid.NewGuid(), Name = "Deri̇nce" },
                new() { Id = Guid.NewGuid(), Name = "Di̇lovasi" },
                new() { Id = Guid.NewGuid(), Name = "Gebze" },
                new() { Id = Guid.NewGuid(), Name = "Gölcük" },
                new() { Id = Guid.NewGuid(), Name = "Kandira" },
                new() { Id = Guid.NewGuid(), Name = "Karamürsel" },
                new() { Id = Guid.NewGuid(), Name = "Kartepe" },
                new() { Id = Guid.NewGuid(), Name = "Körfez" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Çayirova" },
                new() { Id = Guid.NewGuid(), Name = "İzmi̇t" }
            }
        };

        // Örnek veriler - KONYA
        var konya = new City
        {
            Id = Guid.NewGuid(),
            Code = "42",
            Name = "KONYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ahirli" },
                new() { Id = Guid.NewGuid(), Name = "Akören" },
                new() { Id = Guid.NewGuid(), Name = "Akşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Altineki̇n" },
                new() { Id = Guid.NewGuid(), Name = "Beyşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Bozkir" },
                new() { Id = Guid.NewGuid(), Name = "Ci̇hanbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Derbent" },
                new() { Id = Guid.NewGuid(), Name = "Derebucak" },
                new() { Id = Guid.NewGuid(), Name = "Doğanhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Emi̇rgazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Ereğli̇" },
                new() { Id = Guid.NewGuid(), Name = "Güneysinir" },
                new() { Id = Guid.NewGuid(), Name = "Hadi̇m" },
                new() { Id = Guid.NewGuid(), Name = "Halkapinar" },
                new() { Id = Guid.NewGuid(), Name = "Hüyük" },
                new() { Id = Guid.NewGuid(), Name = "Ilgin" },
                new() { Id = Guid.NewGuid(), Name = "Kadinhani" },
                new() { Id = Guid.NewGuid(), Name = "Karapinar" },
                new() { Id = Guid.NewGuid(), Name = "Karatay" },
                new() { Id = Guid.NewGuid(), Name = "Kulu" },
                new() { Id = Guid.NewGuid(), Name = "Meram" },
                new() { Id = Guid.NewGuid(), Name = "Sarayönü" },
                new() { Id = Guid.NewGuid(), Name = "Selçuklu" },
                new() { Id = Guid.NewGuid(), Name = "Seydi̇şehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Taşkent" },
                new() { Id = Guid.NewGuid(), Name = "Tuzlukçu" },
                new() { Id = Guid.NewGuid(), Name = "Yalihüyük" },
                new() { Id = Guid.NewGuid(), Name = "Yunak" },
                new() { Id = Guid.NewGuid(), Name = "Çelti̇k" },
                new() { Id = Guid.NewGuid(), Name = "Çumra" }
            }
        };

        // Örnek veriler - KÜTAHYA
        var kutahya = new City
        {
            Id = Guid.NewGuid(),
            Code = "43",
            Name = "KÜTAHYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altintaş" },
                new() { Id = Guid.NewGuid(), Name = "Aslanapa" },
                new() { Id = Guid.NewGuid(), Name = "Domani̇ç" },
                new() { Id = Guid.NewGuid(), Name = "Dumlupinar" },
                new() { Id = Guid.NewGuid(), Name = "Emet" },
                new() { Id = Guid.NewGuid(), Name = "Gedi̇z" },
                new() { Id = Guid.NewGuid(), Name = "Hi̇sarcik" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pazarlar" },
                new() { Id = Guid.NewGuid(), Name = "Si̇mav" },
                new() { Id = Guid.NewGuid(), Name = "Tavşanli" },
                new() { Id = Guid.NewGuid(), Name = "Çavdarhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Şaphane" }
            }
        };

        // Örnek veriler - KİLİS
        var ki̇li̇s = new City
        {
            Id = Guid.NewGuid(),
            Code = "79",
            Name = "KİLİS",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Elbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Musabeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Polateli̇" }
            }
        };

        // Örnek veriler - MALATYA
        var malatya = new City
        {
            Id = Guid.NewGuid(),
            Code = "44",
            Name = "MALATYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akçadağ" },
                new() { Id = Guid.NewGuid(), Name = "Arapgi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Arguvan" },
                new() { Id = Guid.NewGuid(), Name = "Battalgazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Darende" },
                new() { Id = Guid.NewGuid(), Name = "Doğanyol" },
                new() { Id = Guid.NewGuid(), Name = "Doğanşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Heki̇mhan" },
                new() { Id = Guid.NewGuid(), Name = "Kale" },
                new() { Id = Guid.NewGuid(), Name = "Kuluncak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pütürge" },
                new() { Id = Guid.NewGuid(), Name = "Yazihan" },
                new() { Id = Guid.NewGuid(), Name = "Yeşi̇lyurt" }
            }
        };

        // Örnek veriler - MANİSA
        var mani̇sa = new City
        {
            Id = Guid.NewGuid(),
            Code = "45",
            Name = "MANİSA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ahmetli̇" },
                new() { Id = Guid.NewGuid(), Name = "Akhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Alaşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Demi̇rci̇" },
                new() { Id = Guid.NewGuid(), Name = "Gölmarmara" },
                new() { Id = Guid.NewGuid(), Name = "Gördes" },
                new() { Id = Guid.NewGuid(), Name = "Kirkağaç" },
                new() { Id = Guid.NewGuid(), Name = "Kula" },
                new() { Id = Guid.NewGuid(), Name = "Köprübaşi" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sali̇hli̇" },
                new() { Id = Guid.NewGuid(), Name = "Sarigöl" },
                new() { Id = Guid.NewGuid(), Name = "Saruhanli" },
                new() { Id = Guid.NewGuid(), Name = "Selendi̇" },
                new() { Id = Guid.NewGuid(), Name = "Soma" },
                new() { Id = Guid.NewGuid(), Name = "Turgutlu" },
                new() { Id = Guid.NewGuid(), Name = "Yunusemre" },
                new() { Id = Guid.NewGuid(), Name = "Şehzadeler" }
            }
        };

        // Örnek veriler - MARDİN
        var mardi̇n = new City
        {
            Id = Guid.NewGuid(),
            Code = "47",
            Name = "MARDİN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Artuklu" },
                new() { Id = Guid.NewGuid(), Name = "Dargeçi̇t" },
                new() { Id = Guid.NewGuid(), Name = "Deri̇k" },
                new() { Id = Guid.NewGuid(), Name = "Kiziltepe" },
                new() { Id = Guid.NewGuid(), Name = "Mazidaği" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mi̇dyat" },
                new() { Id = Guid.NewGuid(), Name = "Nusaybi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Savur" },
                new() { Id = Guid.NewGuid(), Name = "Yeşi̇lli̇" },
                new() { Id = Guid.NewGuid(), Name = "Ömerli̇" }
            }
        };

        // Örnek veriler - MERSİN
        var mersi̇n = new City
        {
            Id = Guid.NewGuid(),
            Code = "33",
            Name = "MERSİN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akdeni̇z" },
                new() { Id = Guid.NewGuid(), Name = "Anamur" },
                new() { Id = Guid.NewGuid(), Name = "Aydincik" },
                new() { Id = Guid.NewGuid(), Name = "Bozyazi" },
                new() { Id = Guid.NewGuid(), Name = "Erdemli̇" },
                new() { Id = Guid.NewGuid(), Name = "Gülnar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mezi̇tli̇" },
                new() { Id = Guid.NewGuid(), Name = "Mut" },
                new() { Id = Guid.NewGuid(), Name = "Si̇li̇fke" },
                new() { Id = Guid.NewGuid(), Name = "Tarsus" },
                new() { Id = Guid.NewGuid(), Name = "Toroslar" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇şehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Çamliyayla" }
            }
        };

        // Örnek veriler - MUĞLA
        var mugla = new City
        {
            Id = Guid.NewGuid(),
            Code = "48",
            Name = "MUĞLA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bodrum" },
                new() { Id = Guid.NewGuid(), Name = "Dalaman" },
                new() { Id = Guid.NewGuid(), Name = "Datça" },
                new() { Id = Guid.NewGuid(), Name = "Fethi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Kavaklidere" },
                new() { Id = Guid.NewGuid(), Name = "Köyceği̇z" },
                new() { Id = Guid.NewGuid(), Name = "Marmari̇s" },
                new() { Id = Guid.NewGuid(), Name = "Menteşe" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mi̇las" },
                new() { Id = Guid.NewGuid(), Name = "Ortaca" },
                new() { Id = Guid.NewGuid(), Name = "Seydi̇kemer" },
                new() { Id = Guid.NewGuid(), Name = "Ula" },
                new() { Id = Guid.NewGuid(), Name = "Yatağan" }
            }
        };

        // Örnek veriler - MUŞ
        var mus = new City
        {
            Id = Guid.NewGuid(),
            Code = "49",
            Name = "MUŞ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bulanik" },
                new() { Id = Guid.NewGuid(), Name = "Hasköy" },
                new() { Id = Guid.NewGuid(), Name = "Korkut" },
                new() { Id = Guid.NewGuid(), Name = "Malazgi̇rt" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Varto" }
            }
        };

        // Örnek veriler - NEVŞEHİR
        var nevsehi̇r = new City
        {
            Id = Guid.NewGuid(),
            Code = "50",
            Name = "NEVŞEHİR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Acigöl" },
                new() { Id = Guid.NewGuid(), Name = "Avanos" },
                new() { Id = Guid.NewGuid(), Name = "Deri̇nkuyu" },
                new() { Id = Guid.NewGuid(), Name = "Gülşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Hacibektaş" },
                new() { Id = Guid.NewGuid(), Name = "Kozakli" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ürgüp" }
            }
        };

        // Örnek veriler - NİĞDE
        var ni̇gde = new City
        {
            Id = Guid.NewGuid(),
            Code = "51",
            Name = "NİĞDE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altunhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Bor" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ulukişla" },
                new() { Id = Guid.NewGuid(), Name = "Çamardi" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇ftli̇k" }
            }
        };

        // Örnek veriler - ORDU
        var ordu = new City
        {
            Id = Guid.NewGuid(),
            Code = "52",
            Name = "ORDU",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akkuş" },
                new() { Id = Guid.NewGuid(), Name = "Altinordu" },
                new() { Id = Guid.NewGuid(), Name = "Aybasti" },
                new() { Id = Guid.NewGuid(), Name = "Fatsa" },
                new() { Id = Guid.NewGuid(), Name = "Gölköy" },
                new() { Id = Guid.NewGuid(), Name = "Gülyali" },
                new() { Id = Guid.NewGuid(), Name = "Gürgentepe" },
                new() { Id = Guid.NewGuid(), Name = "Kabadüz" },
                new() { Id = Guid.NewGuid(), Name = "Kabataş" },
                new() { Id = Guid.NewGuid(), Name = "Korgan" },
                new() { Id = Guid.NewGuid(), Name = "Kumru" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mesudi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Perşembe" },
                new() { Id = Guid.NewGuid(), Name = "Ulubey" },
                new() { Id = Guid.NewGuid(), Name = "Çamaş" },
                new() { Id = Guid.NewGuid(), Name = "Çatalpinar" },
                new() { Id = Guid.NewGuid(), Name = "Çaybaşi" },
                new() { Id = Guid.NewGuid(), Name = "Ünye" },
                new() { Id = Guid.NewGuid(), Name = "İki̇zce" }
            }
        };

        // Örnek veriler - OSMANİYE
        var osmani̇ye = new City
        {
            Id = Guid.NewGuid(),
            Code = "80",
            Name = "OSMANİYE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bahçe" },
                new() { Id = Guid.NewGuid(), Name = "Düzi̇çi̇" },
                new() { Id = Guid.NewGuid(), Name = "Hasanbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Kadi̇rli̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sumbas" },
                new() { Id = Guid.NewGuid(), Name = "Toprakkale" }
            }
        };

        // Örnek veriler - RİZE
        var ri̇ze = new City
        {
            Id = Guid.NewGuid(),
            Code = "53",
            Name = "RİZE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ardeşen" },
                new() { Id = Guid.NewGuid(), Name = "Derepazari" },
                new() { Id = Guid.NewGuid(), Name = "Findikli" },
                new() { Id = Guid.NewGuid(), Name = "Güneysu" },
                new() { Id = Guid.NewGuid(), Name = "Hemşi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Kalkandere" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pazar" },
                new() { Id = Guid.NewGuid(), Name = "Çamlihemşi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Çayeli̇" },
                new() { Id = Guid.NewGuid(), Name = "İki̇zdere" },
                new() { Id = Guid.NewGuid(), Name = "İyi̇dere" }
            }
        };

        // Örnek veriler - SAKARYA
        var sakarya = new City
        {
            Id = Guid.NewGuid(),
            Code = "54",
            Name = "SAKARYA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Adapazari" },
                new() { Id = Guid.NewGuid(), Name = "Akyazi" },
                new() { Id = Guid.NewGuid(), Name = "Ari̇fi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Erenler" },
                new() { Id = Guid.NewGuid(), Name = "Feri̇zli̇" },
                new() { Id = Guid.NewGuid(), Name = "Geyve" },
                new() { Id = Guid.NewGuid(), Name = "Hendek" },
                new() { Id = Guid.NewGuid(), Name = "Karapürçek" },
                new() { Id = Guid.NewGuid(), Name = "Karasu" },
                new() { Id = Guid.NewGuid(), Name = "Kaynarca" },
                new() { Id = Guid.NewGuid(), Name = "Kocaali̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pamukova" },
                new() { Id = Guid.NewGuid(), Name = "Sapanca" },
                new() { Id = Guid.NewGuid(), Name = "Serdi̇van" },
                new() { Id = Guid.NewGuid(), Name = "Söğütlü" },
                new() { Id = Guid.NewGuid(), Name = "Tarakli" }
            }
        };

        // Örnek veriler - SAMSUN
        var samsun = new City
        {
            Id = Guid.NewGuid(),
            Code = "55",
            Name = "SAMSUN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "19 Mayis" },
                new() { Id = Guid.NewGuid(), Name = "Alaçam" },
                new() { Id = Guid.NewGuid(), Name = "Asarcik" },
                new() { Id = Guid.NewGuid(), Name = "Atakum" },
                new() { Id = Guid.NewGuid(), Name = "Ayvacik" },
                new() { Id = Guid.NewGuid(), Name = "Bafra" },
                new() { Id = Guid.NewGuid(), Name = "Cani̇k" },
                new() { Id = Guid.NewGuid(), Name = "Havza" },
                new() { Id = Guid.NewGuid(), Name = "Kavak" },
                new() { Id = Guid.NewGuid(), Name = "Ladi̇k" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Salipazari" },
                new() { Id = Guid.NewGuid(), Name = "Tekkeköy" },
                new() { Id = Guid.NewGuid(), Name = "Terme" },
                new() { Id = Guid.NewGuid(), Name = "Vezi̇rköprü" },
                new() { Id = Guid.NewGuid(), Name = "Yakakent" },
                new() { Id = Guid.NewGuid(), Name = "Çarşamba" },
                new() { Id = Guid.NewGuid(), Name = "İlkadim" }
            }
        };

        // Örnek veriler - SİNOP
        var si̇nop = new City
        {
            Id = Guid.NewGuid(),
            Code = "57",
            Name = "SİNOP",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ayancik" },
                new() { Id = Guid.NewGuid(), Name = "Boyabat" },
                new() { Id = Guid.NewGuid(), Name = "Di̇kmen" },
                new() { Id = Guid.NewGuid(), Name = "Durağan" },
                new() { Id = Guid.NewGuid(), Name = "Erfelek" },
                new() { Id = Guid.NewGuid(), Name = "Gerze" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Saraydüzü" },
                new() { Id = Guid.NewGuid(), Name = "Türkeli̇" }
            }
        };

        // Örnek veriler - SİVAS
        var si̇vas = new City
        {
            Id = Guid.NewGuid(),
            Code = "58",
            Name = "SİVAS",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akincilar" },
                new() { Id = Guid.NewGuid(), Name = "Altinyayla" },
                new() { Id = Guid.NewGuid(), Name = "Di̇vri̇ği̇" },
                new() { Id = Guid.NewGuid(), Name = "Doğanşar" },
                new() { Id = Guid.NewGuid(), Name = "Gemerek" },
                new() { Id = Guid.NewGuid(), Name = "Gölova" },
                new() { Id = Guid.NewGuid(), Name = "Gürün" },
                new() { Id = Guid.NewGuid(), Name = "Hafi̇k" },
                new() { Id = Guid.NewGuid(), Name = "Kangal" },
                new() { Id = Guid.NewGuid(), Name = "Koyulhi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Suşehri̇" },
                new() { Id = Guid.NewGuid(), Name = "Ulaş" },
                new() { Id = Guid.NewGuid(), Name = "Yildizeli̇" },
                new() { Id = Guid.NewGuid(), Name = "Zara" },
                new() { Id = Guid.NewGuid(), Name = "İmranli" },
                new() { Id = Guid.NewGuid(), Name = "Şarkişla" }
            }
        };

        // Örnek veriler - SİİRT
        var si̇i̇rt = new City
        {
            Id = Guid.NewGuid(),
            Code = "56",
            Name = "SİİRT",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Baykan" },
                new() { Id = Guid.NewGuid(), Name = "Eruh" },
                new() { Id = Guid.NewGuid(), Name = "Kurtalan" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pervari̇" },
                new() { Id = Guid.NewGuid(), Name = "Ti̇llo" },
                new() { Id = Guid.NewGuid(), Name = "Şi̇rvan" }
            }
        };

        // Örnek veriler - TEKİRDAĞ
        var teki̇rdag = new City
        {
            Id = Guid.NewGuid(),
            Code = "59",
            Name = "TEKİRDAĞ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ergene" },
                new() { Id = Guid.NewGuid(), Name = "Hayrabolu" },
                new() { Id = Guid.NewGuid(), Name = "Kapakli" },
                new() { Id = Guid.NewGuid(), Name = "Malkara" },
                new() { Id = Guid.NewGuid(), Name = "Marmaraereğli̇si̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muratli" },
                new() { Id = Guid.NewGuid(), Name = "Saray" },
                new() { Id = Guid.NewGuid(), Name = "Süleymanpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Çerkezköy" },
                new() { Id = Guid.NewGuid(), Name = "Çorlu" },
                new() { Id = Guid.NewGuid(), Name = "Şarköy" }
            }
        };

        // Örnek veriler - TOKAT
        var tokat = new City
        {
            Id = Guid.NewGuid(),
            Code = "60",
            Name = "TOKAT",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Almus" },
                new() { Id = Guid.NewGuid(), Name = "Artova" },
                new() { Id = Guid.NewGuid(), Name = "Başçi̇ftli̇k" },
                new() { Id = Guid.NewGuid(), Name = "Erbaa" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ni̇ksar" },
                new() { Id = Guid.NewGuid(), Name = "Pazar" },
                new() { Id = Guid.NewGuid(), Name = "Reşadi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Sulusaray" },
                new() { Id = Guid.NewGuid(), Name = "Turhal" },
                new() { Id = Guid.NewGuid(), Name = "Yeşi̇lyurt" },
                new() { Id = Guid.NewGuid(), Name = "Zi̇le" }
            }
        };

        // Örnek veriler - TRABZON
        var trabzon = new City
        {
            Id = Guid.NewGuid(),
            Code = "61",
            Name = "TRABZON",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akçaabat" },
                new() { Id = Guid.NewGuid(), Name = "Arakli" },
                new() { Id = Guid.NewGuid(), Name = "Arsi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Beşi̇kdüzü" },
                new() { Id = Guid.NewGuid(), Name = "Dernekpazari" },
                new() { Id = Guid.NewGuid(), Name = "Düzköy" },
                new() { Id = Guid.NewGuid(), Name = "Hayrat" },
                new() { Id = Guid.NewGuid(), Name = "Köprübaşi" },
                new() { Id = Guid.NewGuid(), Name = "Maçka" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Of" },
                new() { Id = Guid.NewGuid(), Name = "Ortahi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Sürmene" },
                new() { Id = Guid.NewGuid(), Name = "Tonya" },
                new() { Id = Guid.NewGuid(), Name = "Vakfikebi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Yomra" },
                new() { Id = Guid.NewGuid(), Name = "Çarşibaşi" },
                new() { Id = Guid.NewGuid(), Name = "Çaykara" },
                new() { Id = Guid.NewGuid(), Name = "Şalpazari" }
            }
        };

        // Örnek veriler - TUNCELİ
        var tunceli̇ = new City
        {
            Id = Guid.NewGuid(),
            Code = "62",
            Name = "TUNCELİ",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Hozat" },
                new() { Id = Guid.NewGuid(), Name = "Mazgi̇rt" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nazimi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Ovacik" },
                new() { Id = Guid.NewGuid(), Name = "Pertek" },
                new() { Id = Guid.NewGuid(), Name = "Pülümür" },
                new() { Id = Guid.NewGuid(), Name = "Çemi̇şgezek" }
            }
        };

        // Örnek veriler - UŞAK
        var usak = new City
        {
            Id = Guid.NewGuid(),
            Code = "64",
            Name = "UŞAK",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Banaz" },
                new() { Id = Guid.NewGuid(), Name = "Eşme" },
                new() { Id = Guid.NewGuid(), Name = "Karahalli" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Si̇vasli" },
                new() { Id = Guid.NewGuid(), Name = "Ulubey" }
            }
        };

        // Örnek veriler - VAN
        var van = new City
        {
            Id = Guid.NewGuid(),
            Code = "65",
            Name = "VAN",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Bahçesaray" },
                new() { Id = Guid.NewGuid(), Name = "Başkale" },
                new() { Id = Guid.NewGuid(), Name = "Edremi̇t" },
                new() { Id = Guid.NewGuid(), Name = "Erci̇ş" },
                new() { Id = Guid.NewGuid(), Name = "Gevaş" },
                new() { Id = Guid.NewGuid(), Name = "Gürpinar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muradi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Saray" },
                new() { Id = Guid.NewGuid(), Name = "Tuşba" },
                new() { Id = Guid.NewGuid(), Name = "Çaldiran" },
                new() { Id = Guid.NewGuid(), Name = "Çatak" },
                new() { Id = Guid.NewGuid(), Name = "Özalp" },
                new() { Id = Guid.NewGuid(), Name = "İpekyolu" }
            }
        };

        // Örnek veriler - YALOVA
        var yalova = new City
        {
            Id = Guid.NewGuid(),
            Code = "77",
            Name = "YALOVA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Altinova" },
                new() { Id = Guid.NewGuid(), Name = "Armutlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Termal" },
                new() { Id = Guid.NewGuid(), Name = "Çinarcik" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇ftli̇kköy" }
            }
        };

        // Örnek veriler - YOZGAT
        var yozgat = new City
        {
            Id = Guid.NewGuid(),
            Code = "66",
            Name = "YOZGAT",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akdağmadeni̇" },
                new() { Id = Guid.NewGuid(), Name = "Aydincik" },
                new() { Id = Guid.NewGuid(), Name = "Boğazliyan" },
                new() { Id = Guid.NewGuid(), Name = "Kadişehri̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Saraykent" },
                new() { Id = Guid.NewGuid(), Name = "Sarikaya" },
                new() { Id = Guid.NewGuid(), Name = "Sorgun" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇fakili" },
                new() { Id = Guid.NewGuid(), Name = "Yerköy" },
                new() { Id = Guid.NewGuid(), Name = "Çandir" },
                new() { Id = Guid.NewGuid(), Name = "Çayiralan" },
                new() { Id = Guid.NewGuid(), Name = "Çekerek" },
                new() { Id = Guid.NewGuid(), Name = "Şefaatli̇" }
            }
        };

        // Örnek veriler - ZONGULDAK
        var zonguldak = new City
        {
            Id = Guid.NewGuid(),
            Code = "67",
            Name = "ZONGULDAK",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Alapli" },
                new() { Id = Guid.NewGuid(), Name = "Devrek" },
                new() { Id = Guid.NewGuid(), Name = "Ereğli̇" },
                new() { Id = Guid.NewGuid(), Name = "Gökçebey" },
                new() { Id = Guid.NewGuid(), Name = "Ki̇li̇mli̇" },
                new() { Id = Guid.NewGuid(), Name = "Kozlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Çaycuma" }
            }
        };

        // Örnek veriler - ÇANAKKALE
        var canakkale = new City
        {
            Id = Guid.NewGuid(),
            Code = "17",
            Name = "ÇANAKKALE",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ayvacik" },
                new() { Id = Guid.NewGuid(), Name = "Bayrami̇ç" },
                new() { Id = Guid.NewGuid(), Name = "Bi̇ga" },
                new() { Id = Guid.NewGuid(), Name = "Bozcaada" },
                new() { Id = Guid.NewGuid(), Name = "Eceabat" },
                new() { Id = Guid.NewGuid(), Name = "Ezi̇ne" },
                new() { Id = Guid.NewGuid(), Name = "Geli̇bolu" },
                new() { Id = Guid.NewGuid(), Name = "Gökçeada" },
                new() { Id = Guid.NewGuid(), Name = "Lapseki̇" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Yeni̇ce" },
                new() { Id = Guid.NewGuid(), Name = "Çan" }
            }
        };

        // Örnek veriler - ÇANKIRI
        var cankiri = new City
        {
            Id = Guid.NewGuid(),
            Code = "18",
            Name = "ÇANKIRI",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Atkaracalar" },
                new() { Id = Guid.NewGuid(), Name = "Bayramören" },
                new() { Id = Guid.NewGuid(), Name = "Eldi̇van" },
                new() { Id = Guid.NewGuid(), Name = "Ilgaz" },
                new() { Id = Guid.NewGuid(), Name = "Kizilirmak" },
                new() { Id = Guid.NewGuid(), Name = "Korgun" },
                new() { Id = Guid.NewGuid(), Name = "Kurşunlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Orta" },
                new() { Id = Guid.NewGuid(), Name = "Yaprakli" },
                new() { Id = Guid.NewGuid(), Name = "Çerkeş" },
                new() { Id = Guid.NewGuid(), Name = "Şabanözü" }
            }
        };

        // Örnek veriler - ÇORUM
        var corum = new City
        {
            Id = Guid.NewGuid(),
            Code = "19",
            Name = "ÇORUM",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Alaca" },
                new() { Id = Guid.NewGuid(), Name = "Bayat" },
                new() { Id = Guid.NewGuid(), Name = "Boğazkale" },
                new() { Id = Guid.NewGuid(), Name = "Dodurga" },
                new() { Id = Guid.NewGuid(), Name = "Kargi" },
                new() { Id = Guid.NewGuid(), Name = "Laçi̇n" },
                new() { Id = Guid.NewGuid(), Name = "Meci̇tözü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ortaköy" },
                new() { Id = Guid.NewGuid(), Name = "Osmancik" },
                new() { Id = Guid.NewGuid(), Name = "Oğuzlar" },
                new() { Id = Guid.NewGuid(), Name = "Sungurlu" },
                new() { Id = Guid.NewGuid(), Name = "Uğurludağ" },
                new() { Id = Guid.NewGuid(), Name = "İski̇li̇p" }
            }
        };

        // Örnek veriler - İSTANBUL
        var i̇stanbul = new City
        {
            Id = Guid.NewGuid(),
            Code = "34",
            Name = "İSTANBUL",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Adalar" },
                new() { Id = Guid.NewGuid(), Name = "Arnavutköy" },
                new() { Id = Guid.NewGuid(), Name = "Ataşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Avcilar" },
                new() { Id = Guid.NewGuid(), Name = "Bahçeli̇evler" },
                new() { Id = Guid.NewGuid(), Name = "Bakirköy" },
                new() { Id = Guid.NewGuid(), Name = "Bayrampaşa" },
                new() { Id = Guid.NewGuid(), Name = "Bağcilar" },
                new() { Id = Guid.NewGuid(), Name = "Başakşehi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Beykoz" },
                new() { Id = Guid.NewGuid(), Name = "Beyli̇kdüzü" },
                new() { Id = Guid.NewGuid(), Name = "Beyoğlu" },
                new() { Id = Guid.NewGuid(), Name = "Beşi̇ktaş" },
                new() { Id = Guid.NewGuid(), Name = "Büyükçekmece" },
                new() { Id = Guid.NewGuid(), Name = "Emi̇nönü" },
                new() { Id = Guid.NewGuid(), Name = "Esenler" },
                new() { Id = Guid.NewGuid(), Name = "Esenyurt" },
                new() { Id = Guid.NewGuid(), Name = "Eyüpsultan" },
                new() { Id = Guid.NewGuid(), Name = "Fati̇h" },
                new() { Id = Guid.NewGuid(), Name = "Gazi̇osmanpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Güngören" },
                new() { Id = Guid.NewGuid(), Name = "Kadiköy" },
                new() { Id = Guid.NewGuid(), Name = "Kartal" },
                new() { Id = Guid.NewGuid(), Name = "Kağithane" },
                new() { Id = Guid.NewGuid(), Name = "Küçükçekmece" },
                new() { Id = Guid.NewGuid(), Name = "Maltepe" },
                new() { Id = Guid.NewGuid(), Name = "Pendi̇k" },
                new() { Id = Guid.NewGuid(), Name = "Sancaktepe" },
                new() { Id = Guid.NewGuid(), Name = "Sariyer" },
                new() { Id = Guid.NewGuid(), Name = "Si̇li̇vri̇" },
                new() { Id = Guid.NewGuid(), Name = "Sultanbeyli̇" },
                new() { Id = Guid.NewGuid(), Name = "Sultangazi̇" },
                new() { Id = Guid.NewGuid(), Name = "Tuzla" },
                new() { Id = Guid.NewGuid(), Name = "Zeyti̇nburnu" },
                new() { Id = Guid.NewGuid(), Name = "Çatalca" },
                new() { Id = Guid.NewGuid(), Name = "Çekmeköy" },
                new() { Id = Guid.NewGuid(), Name = "Ümrani̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Üsküdar" },
                new() { Id = Guid.NewGuid(), Name = "Şi̇le" },
                new() { Id = Guid.NewGuid(), Name = "Şi̇şli̇" }
            }
        };

        // Örnek veriler - İZMİR
        var i̇zmi̇r = new City
        {
            Id = Guid.NewGuid(),
            Code = "35",
            Name = "İZMİR",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Ali̇ağa" },
                new() { Id = Guid.NewGuid(), Name = "Balçova" },
                new() { Id = Guid.NewGuid(), Name = "Bayindir" },
                new() { Id = Guid.NewGuid(), Name = "Bayrakli" },
                new() { Id = Guid.NewGuid(), Name = "Bergama" },
                new() { Id = Guid.NewGuid(), Name = "Beydağ" },
                new() { Id = Guid.NewGuid(), Name = "Bornova" },
                new() { Id = Guid.NewGuid(), Name = "Buca" },
                new() { Id = Guid.NewGuid(), Name = "Di̇ki̇li̇" },
                new() { Id = Guid.NewGuid(), Name = "Foça" },
                new() { Id = Guid.NewGuid(), Name = "Gazi̇emi̇r" },
                new() { Id = Guid.NewGuid(), Name = "Güzelbahçe" },
                new() { Id = Guid.NewGuid(), Name = "Karabağlar" },
                new() { Id = Guid.NewGuid(), Name = "Karaburun" },
                new() { Id = Guid.NewGuid(), Name = "Karşiyaka" },
                new() { Id = Guid.NewGuid(), Name = "Kemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Kinik" },
                new() { Id = Guid.NewGuid(), Name = "Ki̇raz" },
                new() { Id = Guid.NewGuid(), Name = "Konak" },
                new() { Id = Guid.NewGuid(), Name = "Menderes" },
                new() { Id = Guid.NewGuid(), Name = "Menemen" },
                new() { Id = Guid.NewGuid(), Name = "Narlidere" },
                new() { Id = Guid.NewGuid(), Name = "Seferi̇hi̇sar" },
                new() { Id = Guid.NewGuid(), Name = "Selçuk" },
                new() { Id = Guid.NewGuid(), Name = "Ti̇re" },
                new() { Id = Guid.NewGuid(), Name = "Torbali" },
                new() { Id = Guid.NewGuid(), Name = "Urla" },
                new() { Id = Guid.NewGuid(), Name = "Çeşme" },
                new() { Id = Guid.NewGuid(), Name = "Çi̇ğli̇" },
                new() { Id = Guid.NewGuid(), Name = "Ödemi̇ş" }
            }
        };

        // Örnek veriler - ŞANLIURFA
        var sanliurfa = new City
        {
            Id = Guid.NewGuid(),
            Code = "63",
            Name = "ŞANLIURFA",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Akçakale" },
                new() { Id = Guid.NewGuid(), Name = "Bi̇reci̇k" },
                new() { Id = Guid.NewGuid(), Name = "Bozova" },
                new() { Id = Guid.NewGuid(), Name = "Ceylanpinar" },
                new() { Id = Guid.NewGuid(), Name = "Eyyübi̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Halfeti̇" },
                new() { Id = Guid.NewGuid(), Name = "Hali̇li̇ye" },
                new() { Id = Guid.NewGuid(), Name = "Harran" },
                new() { Id = Guid.NewGuid(), Name = "Hi̇lvan" },
                new() { Id = Guid.NewGuid(), Name = "Karaköprü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Si̇verek" },
                new() { Id = Guid.NewGuid(), Name = "Suruç" },
                new() { Id = Guid.NewGuid(), Name = "Vi̇ranşehi̇r" }
            }
        };

        // Örnek veriler - ŞIRNAK
        var sirnak = new City
        {
            Id = Guid.NewGuid(),
            Code = "73",
            Name = "ŞIRNAK",
            Districts = new List<District>
            {
                new() { Id = Guid.NewGuid(), Name = "Beytüşşebap" },
                new() { Id = Guid.NewGuid(), Name = "Ci̇zre" },
                new() { Id = Guid.NewGuid(), Name = "Güçlükonak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Si̇lopi̇" },
                new() { Id = Guid.NewGuid(), Name = "Uludere" },
                new() { Id = Guid.NewGuid(), Name = "İdi̇l" }
            }
        };

        var allCities = new List<City> { adana, adiyaman, afyonkarahi̇sar, aksaray, amasya, ankara, antalya, ardahan, artvi̇n, aydin, agri, balikesi̇r, bartin, batman, bayburt, bolu, burdur, bursa, bi̇leci̇k, bi̇ngol, bi̇tli̇s, deni̇zli̇, duzce, di̇yarbakir, edi̇rne, elazig, erzurum, erzi̇ncan, eski̇sehi̇r, gazi̇antep, gumushane, gi̇resun, hakkari̇, hatay, isparta, igdir, kahramanmaras, karabuk, karaman, kars, kastamonu, kayseri̇, kirikkale, kirklareli̇, kirsehi̇r, kocaeli̇, konya, kutahya, ki̇li̇s, malatya, mani̇sa, mardi̇n, mersi̇n, mugla, mus, nevsehi̇r, ni̇gde, ordu, osmani̇ye, ri̇ze, sakarya, samsun, si̇nop, si̇vas, si̇i̇rt, teki̇rdag, tokat, trabzon, tunceli̇, usak, van, yalova, yozgat, zonguldak, canakkale, cankiri, corum, i̇stanbul, i̇zmi̇r, sanliurfa, sirnak };

        await db.Cities.AddRangeAsync(allCities, ct);
        await db.SaveChangesAsync(ct);
    }
}
