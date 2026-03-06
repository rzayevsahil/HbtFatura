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
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Aladağ" },
                new() { Id = Guid.NewGuid(), Name = "Ceyhan" },
                new() { Id = Guid.NewGuid(), Name = "Feke" },
                new() { Id = Guid.NewGuid(), Name = "Karaisalı" },
                new() { Id = Guid.NewGuid(), Name = "Karataş" },
                new() { Id = Guid.NewGuid(), Name = "Kozan" },
                new() { Id = Guid.NewGuid(), Name = "Pozantı" },
                new() { Id = Guid.NewGuid(), Name = "Saimbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Sarıçam" },
                new() { Id = Guid.NewGuid(), Name = "Seyhan" },
                new() { Id = Guid.NewGuid(), Name = "Tufanbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Yumurtalık" },
                new() { Id = Guid.NewGuid(), Name = "Yüreğir" },
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
                new() { Id = Guid.NewGuid(), Name = "Besni" },
                new() { Id = Guid.NewGuid(), Name = "Gerger" },
                new() { Id = Guid.NewGuid(), Name = "Gölbaşı" },
                new() { Id = Guid.NewGuid(), Name = "Kahta" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Samsat" },
                new() { Id = Guid.NewGuid(), Name = "Sincik" },
                new() { Id = Guid.NewGuid(), Name = "Tut" },
                new() { Id = Guid.NewGuid(), Name = "Çelikhan" }
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
                new() { Id = Guid.NewGuid(), Name = "Başmakçı" },
                new() { Id = Guid.NewGuid(), Name = "Bolvadin" },
                new() { Id = Guid.NewGuid(), Name = "Dazkırı" },
                new() { Id = Guid.NewGuid(), Name = "Dinar" },
                new() { Id = Guid.NewGuid(), Name = "Emirdağ" },
                new() { Id = Guid.NewGuid(), Name = "Evciler" },
                new() { Id = Guid.NewGuid(), Name = "Hocalar" },
                new() { Id = Guid.NewGuid(), Name = "Kızılören" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sandıklı" },
                new() { Id = Guid.NewGuid(), Name = "Sinanpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Sultandağı" },
                new() { Id = Guid.NewGuid(), Name = "Çay" },
                new() { Id = Guid.NewGuid(), Name = "Çobanlar" },
                new() { Id = Guid.NewGuid(), Name = "İhsaniye" },
                new() { Id = Guid.NewGuid(), Name = "İscehisar" },
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
                new() { Id = Guid.NewGuid(), Name = "Eskil" },
                new() { Id = Guid.NewGuid(), Name = "Gülağaç" },
                new() { Id = Guid.NewGuid(), Name = "Güzelyurt" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ortaköy" },
                new() { Id = Guid.NewGuid(), Name = "Sarıyahşi" },
                new() { Id = Guid.NewGuid(), Name = "Sultanhanı" }
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
                new() { Id = Guid.NewGuid(), Name = "Gümüşhacıköy" },
                new() { Id = Guid.NewGuid(), Name = "Hamamözü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Merzifon" },
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
                new() { Id = Guid.NewGuid(), Name = "Altındağ" },
                new() { Id = Guid.NewGuid(), Name = "Ayaş" },
                new() { Id = Guid.NewGuid(), Name = "Bala" },
                new() { Id = Guid.NewGuid(), Name = "Beypazarı" },
                new() { Id = Guid.NewGuid(), Name = "Elmadağ" },
                new() { Id = Guid.NewGuid(), Name = "Etimesgut" },
                new() { Id = Guid.NewGuid(), Name = "Evren" },
                new() { Id = Guid.NewGuid(), Name = "Gölbaşı" },
                new() { Id = Guid.NewGuid(), Name = "Güdül" },
                new() { Id = Guid.NewGuid(), Name = "Haymana" },
                new() { Id = Guid.NewGuid(), Name = "Kahramankazan" },
                new() { Id = Guid.NewGuid(), Name = "Kalecik" },
                new() { Id = Guid.NewGuid(), Name = "Keçiören" },
                new() { Id = Guid.NewGuid(), Name = "Kızılcahamam" },
                new() { Id = Guid.NewGuid(), Name = "Mamak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nallıhan" },
                new() { Id = Guid.NewGuid(), Name = "Polatlı" },
                new() { Id = Guid.NewGuid(), Name = "Pursaklar" },
                new() { Id = Guid.NewGuid(), Name = "Sincan" },
                new() { Id = Guid.NewGuid(), Name = "Yenimahalle" },
                new() { Id = Guid.NewGuid(), Name = "Çamlıdere" },
                new() { Id = Guid.NewGuid(), Name = "Çankaya" },
                new() { Id = Guid.NewGuid(), Name = "Çubuk" },
                new() { Id = Guid.NewGuid(), Name = "Şereflikoçhisar" }
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
                new() { Id = Guid.NewGuid(), Name = "Akseki" },
                new() { Id = Guid.NewGuid(), Name = "Aksu" },
                new() { Id = Guid.NewGuid(), Name = "Alanya" },
                new() { Id = Guid.NewGuid(), Name = "Demre" },
                new() { Id = Guid.NewGuid(), Name = "Döşemealtı" },
                new() { Id = Guid.NewGuid(), Name = "Elmalı" },
                new() { Id = Guid.NewGuid(), Name = "Finike" },
                new() { Id = Guid.NewGuid(), Name = "Gazipaşa" },
                new() { Id = Guid.NewGuid(), Name = "Gündoğmuş" },
                new() { Id = Guid.NewGuid(), Name = "Kaş" },
                new() { Id = Guid.NewGuid(), Name = "Kemer" },
                new() { Id = Guid.NewGuid(), Name = "Kepez" },
                new() { Id = Guid.NewGuid(), Name = "Konyaaltı" },
                new() { Id = Guid.NewGuid(), Name = "Korkuteli" },
                new() { Id = Guid.NewGuid(), Name = "Kumluca" },
                new() { Id = Guid.NewGuid(), Name = "Manavgat" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muratpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Serik" },
                new() { Id = Guid.NewGuid(), Name = "İbradı" }
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
                new() { Id = Guid.NewGuid(), Name = "Çıldır" }
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
                new() { Id = Guid.NewGuid(), Name = "Arhavi" },
                new() { Id = Guid.NewGuid(), Name = "Borçka" },
                new() { Id = Guid.NewGuid(), Name = "Hopa" },
                new() { Id = Guid.NewGuid(), Name = "Kemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Murgul" },
                new() { Id = Guid.NewGuid(), Name = "Yusufeli" },
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
                new() { Id = Guid.NewGuid(), Name = "Didim" },
                new() { Id = Guid.NewGuid(), Name = "Efeler" },
                new() { Id = Guid.NewGuid(), Name = "Germencik" },
                new() { Id = Guid.NewGuid(), Name = "Karacasu" },
                new() { Id = Guid.NewGuid(), Name = "Karpuzlu" },
                new() { Id = Guid.NewGuid(), Name = "Koçarlı" },
                new() { Id = Guid.NewGuid(), Name = "Kuyucak" },
                new() { Id = Guid.NewGuid(), Name = "Kuşadası" },
                new() { Id = Guid.NewGuid(), Name = "Köşk" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nazilli" },
                new() { Id = Guid.NewGuid(), Name = "Sultanhisar" },
                new() { Id = Guid.NewGuid(), Name = "Söke" },
                new() { Id = Guid.NewGuid(), Name = "Yenipazar" },
                new() { Id = Guid.NewGuid(), Name = "Çine" },
                new() { Id = Guid.NewGuid(), Name = "İncirliova" }
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
                new() { Id = Guid.NewGuid(), Name = "Diyadin" },
                new() { Id = Guid.NewGuid(), Name = "Doğubayazıt" },
                new() { Id = Guid.NewGuid(), Name = "Eleşkirt" },
                new() { Id = Guid.NewGuid(), Name = "Hamur" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Patnos" },
                new() { Id = Guid.NewGuid(), Name = "Taşlıçay" },
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
                new() { Id = Guid.NewGuid(), Name = "Altıeylül" },
                new() { Id = Guid.NewGuid(), Name = "Ayvalık" },
                new() { Id = Guid.NewGuid(), Name = "Balya" },
                new() { Id = Guid.NewGuid(), Name = "Bandırma" },
                new() { Id = Guid.NewGuid(), Name = "Bigadiç" },
                new() { Id = Guid.NewGuid(), Name = "Burhaniye" },
                new() { Id = Guid.NewGuid(), Name = "Dursunbey" },
                new() { Id = Guid.NewGuid(), Name = "Edremit" },
                new() { Id = Guid.NewGuid(), Name = "Erdek" },
                new() { Id = Guid.NewGuid(), Name = "Gömeç" },
                new() { Id = Guid.NewGuid(), Name = "Gönen" },
                new() { Id = Guid.NewGuid(), Name = "Havran" },
                new() { Id = Guid.NewGuid(), Name = "Karesi" },
                new() { Id = Guid.NewGuid(), Name = "Kepsut" },
                new() { Id = Guid.NewGuid(), Name = "Manyas" },
                new() { Id = Guid.NewGuid(), Name = "Marmara" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Savaştepe" },
                new() { Id = Guid.NewGuid(), Name = "Susurluk" },
                new() { Id = Guid.NewGuid(), Name = "Sındırgı" },
                new() { Id = Guid.NewGuid(), Name = "İvrindi" }
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
                new() { Id = Guid.NewGuid(), Name = "Kurucaşile" },
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
                new() { Id = Guid.NewGuid(), Name = "Beşiri" },
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
                new() { Id = Guid.NewGuid(), Name = "Aydıntepe" },
                new() { Id = Guid.NewGuid(), Name = "Demirözü" },
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
                new() { Id = Guid.NewGuid(), Name = "Dörtdivan" },
                new() { Id = Guid.NewGuid(), Name = "Gerede" },
                new() { Id = Guid.NewGuid(), Name = "Göynük" },
                new() { Id = Guid.NewGuid(), Name = "Kıbrıscık" },
                new() { Id = Guid.NewGuid(), Name = "Mengen" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mudurnu" },
                new() { Id = Guid.NewGuid(), Name = "Seben" },
                new() { Id = Guid.NewGuid(), Name = "Yeniçağa" }
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
                new() { Id = Guid.NewGuid(), Name = "Altınyayla" },
                new() { Id = Guid.NewGuid(), Name = "Ağlasun" },
                new() { Id = Guid.NewGuid(), Name = "Bucak" },
                new() { Id = Guid.NewGuid(), Name = "Gölhisar" },
                new() { Id = Guid.NewGuid(), Name = "Karamanlı" },
                new() { Id = Guid.NewGuid(), Name = "Kemer" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Tefenni" },
                new() { Id = Guid.NewGuid(), Name = "Yeşilova" },
                new() { Id = Guid.NewGuid(), Name = "Çavdır" },
                new() { Id = Guid.NewGuid(), Name = "Çeltikçi" }
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
                new() { Id = Guid.NewGuid(), Name = "Gemlik" },
                new() { Id = Guid.NewGuid(), Name = "Gürsu" },
                new() { Id = Guid.NewGuid(), Name = "Harmancık" },
                new() { Id = Guid.NewGuid(), Name = "Karacabey" },
                new() { Id = Guid.NewGuid(), Name = "Keles" },
                new() { Id = Guid.NewGuid(), Name = "Kestel" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mudanya" },
                new() { Id = Guid.NewGuid(), Name = "Mustafakemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Nilüfer" },
                new() { Id = Guid.NewGuid(), Name = "Orhaneli" },
                new() { Id = Guid.NewGuid(), Name = "Orhangazi" },
                new() { Id = Guid.NewGuid(), Name = "Osmangazi" },
                new() { Id = Guid.NewGuid(), Name = "Yenişehir" },
                new() { Id = Guid.NewGuid(), Name = "Yıldırım" },
                new() { Id = Guid.NewGuid(), Name = "İnegöl" },
                new() { Id = Guid.NewGuid(), Name = "İznik" }
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
                new() { Id = Guid.NewGuid(), Name = "Gölpazarı" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Osmaneli" },
                new() { Id = Guid.NewGuid(), Name = "Pazaryeri" },
                new() { Id = Guid.NewGuid(), Name = "Söğüt" },
                new() { Id = Guid.NewGuid(), Name = "Yenipazar" },
                new() { Id = Guid.NewGuid(), Name = "İnhisar" }
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
                new() { Id = Guid.NewGuid(), Name = "Adaklı" },
                new() { Id = Guid.NewGuid(), Name = "Genç" },
                new() { Id = Guid.NewGuid(), Name = "Karlıova" },
                new() { Id = Guid.NewGuid(), Name = "Kiğı" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Solhan" },
                new() { Id = Guid.NewGuid(), Name = "Yayladere" },
                new() { Id = Guid.NewGuid(), Name = "Yedisu" }
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
                new() { Id = Guid.NewGuid(), Name = "Adilcevaz" },
                new() { Id = Guid.NewGuid(), Name = "Ahlat" },
                new() { Id = Guid.NewGuid(), Name = "Güroymak" },
                new() { Id = Guid.NewGuid(), Name = "Hizan" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mutki" },
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
                new() { Id = Guid.NewGuid(), Name = "Acıpayam" },
                new() { Id = Guid.NewGuid(), Name = "Babadağ" },
                new() { Id = Guid.NewGuid(), Name = "Baklan" },
                new() { Id = Guid.NewGuid(), Name = "Bekilli" },
                new() { Id = Guid.NewGuid(), Name = "Beyağaç" },
                new() { Id = Guid.NewGuid(), Name = "Bozkurt" },
                new() { Id = Guid.NewGuid(), Name = "Buldan" },
                new() { Id = Guid.NewGuid(), Name = "Güney" },
                new() { Id = Guid.NewGuid(), Name = "Honaz" },
                new() { Id = Guid.NewGuid(), Name = "Kale" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Merkezefendi" },
                new() { Id = Guid.NewGuid(), Name = "Pamukkale" },
                new() { Id = Guid.NewGuid(), Name = "Sarayköy" },
                new() { Id = Guid.NewGuid(), Name = "Serinhisar" },
                new() { Id = Guid.NewGuid(), Name = "Tavas" },
                new() { Id = Guid.NewGuid(), Name = "Çal" },
                new() { Id = Guid.NewGuid(), Name = "Çameli" },
                new() { Id = Guid.NewGuid(), Name = "Çardak" },
                new() { Id = Guid.NewGuid(), Name = "Çivril" }
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
                new() { Id = Guid.NewGuid(), Name = "Cumayeri" },
                new() { Id = Guid.NewGuid(), Name = "Gölyaka" },
                new() { Id = Guid.NewGuid(), Name = "Gümüşova" },
                new() { Id = Guid.NewGuid(), Name = "Kaynaşlı" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Yığılca" },
                new() { Id = Guid.NewGuid(), Name = "Çilimli" }
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
                new() { Id = Guid.NewGuid(), Name = "Bismil" },
                new() { Id = Guid.NewGuid(), Name = "Dicle" },
                new() { Id = Guid.NewGuid(), Name = "Ergani" },
                new() { Id = Guid.NewGuid(), Name = "Eğil" },
                new() { Id = Guid.NewGuid(), Name = "Hani" },
                new() { Id = Guid.NewGuid(), Name = "Hazro" },
                new() { Id = Guid.NewGuid(), Name = "Kayapınar" },
                new() { Id = Guid.NewGuid(), Name = "Kocaköy" },
                new() { Id = Guid.NewGuid(), Name = "Kulp" },
                new() { Id = Guid.NewGuid(), Name = "Lice" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Silvan" },
                new() { Id = Guid.NewGuid(), Name = "Sur" },
                new() { Id = Guid.NewGuid(), Name = "Yenişehir" },
                new() { Id = Guid.NewGuid(), Name = "Çermik" },
                new() { Id = Guid.NewGuid(), Name = "Çüngüş" },
                new() { Id = Guid.NewGuid(), Name = "Çınar" }
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
                new() { Id = Guid.NewGuid(), Name = "Meriç" },
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
                new() { Id = Guid.NewGuid(), Name = "Arıcak" },
                new() { Id = Guid.NewGuid(), Name = "Ağın" },
                new() { Id = Guid.NewGuid(), Name = "Baskil" },
                new() { Id = Guid.NewGuid(), Name = "Karakoçan" },
                new() { Id = Guid.NewGuid(), Name = "Keban" },
                new() { Id = Guid.NewGuid(), Name = "Kovancılar" },
                new() { Id = Guid.NewGuid(), Name = "Maden" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Palu" },
                new() { Id = Guid.NewGuid(), Name = "Sivrice" }
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
                new() { Id = Guid.NewGuid(), Name = "Aziziye" },
                new() { Id = Guid.NewGuid(), Name = "Aşkale" },
                new() { Id = Guid.NewGuid(), Name = "Horasan" },
                new() { Id = Guid.NewGuid(), Name = "Hınıs" },
                new() { Id = Guid.NewGuid(), Name = "Karayazı" },
                new() { Id = Guid.NewGuid(), Name = "Karaçoban" },
                new() { Id = Guid.NewGuid(), Name = "Köprüköy" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Narman" },
                new() { Id = Guid.NewGuid(), Name = "Oltu" },
                new() { Id = Guid.NewGuid(), Name = "Olur" },
                new() { Id = Guid.NewGuid(), Name = "Palandöken" },
                new() { Id = Guid.NewGuid(), Name = "Pasinler" },
                new() { Id = Guid.NewGuid(), Name = "Pazaryolu" },
                new() { Id = Guid.NewGuid(), Name = "Tekman" },
                new() { Id = Guid.NewGuid(), Name = "Tortum" },
                new() { Id = Guid.NewGuid(), Name = "Uzundere" },
                new() { Id = Guid.NewGuid(), Name = "Yakutiye" },
                new() { Id = Guid.NewGuid(), Name = "Çat" },
                new() { Id = Guid.NewGuid(), Name = "İspir" },
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
                new() { Id = Guid.NewGuid(), Name = "Kemaliye" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Otlukbeli" },
                new() { Id = Guid.NewGuid(), Name = "Refahiye" },
                new() { Id = Guid.NewGuid(), Name = "Tercan" },
                new() { Id = Guid.NewGuid(), Name = "Çayırlı" },
                new() { Id = Guid.NewGuid(), Name = "Üzümlü" },
                new() { Id = Guid.NewGuid(), Name = "İliç" }
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
                new() { Id = Guid.NewGuid(), Name = "Beylikova" },
                new() { Id = Guid.NewGuid(), Name = "Günyüzü" },
                new() { Id = Guid.NewGuid(), Name = "Han" },
                new() { Id = Guid.NewGuid(), Name = "Mahmudiye" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mihalgazi" },
                new() { Id = Guid.NewGuid(), Name = "Mihalıççık" },
                new() { Id = Guid.NewGuid(), Name = "Odunpazarı" },
                new() { Id = Guid.NewGuid(), Name = "Sarıcakaya" },
                new() { Id = Guid.NewGuid(), Name = "Seyitgazi" },
                new() { Id = Guid.NewGuid(), Name = "Sivrihisar" },
                new() { Id = Guid.NewGuid(), Name = "Tepebaşı" },
                new() { Id = Guid.NewGuid(), Name = "Çifteler" },
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
                new() { Id = Guid.NewGuid(), Name = "Karkamış" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nizip" },
                new() { Id = Guid.NewGuid(), Name = "Nurdağı" },
                new() { Id = Guid.NewGuid(), Name = "Oğuzeli" },
                new() { Id = Guid.NewGuid(), Name = "Yavuzeli" },
                new() { Id = Guid.NewGuid(), Name = "İslahiye" },
                new() { Id = Guid.NewGuid(), Name = "Şahinbey" },
                new() { Id = Guid.NewGuid(), Name = "Şehitkamil" }
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
                new() { Id = Guid.NewGuid(), Name = "Kelkit" },
                new() { Id = Guid.NewGuid(), Name = "Köse" },
                new() { Id = Guid.NewGuid(), Name = "Kürtün" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Torul" },
                new() { Id = Guid.NewGuid(), Name = "Şiran" }
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
                new() { Id = Guid.NewGuid(), Name = "Dereli" },
                new() { Id = Guid.NewGuid(), Name = "Doğankent" },
                new() { Id = Guid.NewGuid(), Name = "Espiye" },
                new() { Id = Guid.NewGuid(), Name = "Eynesil" },
                new() { Id = Guid.NewGuid(), Name = "Görele" },
                new() { Id = Guid.NewGuid(), Name = "Güce" },
                new() { Id = Guid.NewGuid(), Name = "Keşap" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Piraziz" },
                new() { Id = Guid.NewGuid(), Name = "Tirebolu" },
                new() { Id = Guid.NewGuid(), Name = "Yağlıdere" },
                new() { Id = Guid.NewGuid(), Name = "Çamoluk" },
                new() { Id = Guid.NewGuid(), Name = "Çanakçı" },
                new() { Id = Guid.NewGuid(), Name = "Şebinkarahisar" }
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
                new() { Id = Guid.NewGuid(), Name = "Derecik" },
                new() { Id = Guid.NewGuid(), Name = "Yüksekova" },
                new() { Id = Guid.NewGuid(), Name = "Çukurca" },
                new() { Id = Guid.NewGuid(), Name = "Şemdinli" }
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
                new() { Id = Guid.NewGuid(), Name = "Altınözü" },
                new() { Id = Guid.NewGuid(), Name = "Antakya" },
                new() { Id = Guid.NewGuid(), Name = "Arsuz" },
                new() { Id = Guid.NewGuid(), Name = "Belen" },
                new() { Id = Guid.NewGuid(), Name = "Defne" },
                new() { Id = Guid.NewGuid(), Name = "Dörtyol" },
                new() { Id = Guid.NewGuid(), Name = "Erzin" },
                new() { Id = Guid.NewGuid(), Name = "Hassa" },
                new() { Id = Guid.NewGuid(), Name = "Kumlu" },
                new() { Id = Guid.NewGuid(), Name = "Kırıkhan" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Payas" },
                new() { Id = Guid.NewGuid(), Name = "Reyhanlı" },
                new() { Id = Guid.NewGuid(), Name = "Samandağ" },
                new() { Id = Guid.NewGuid(), Name = "Yayladağı" },
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
                new() { Id = Guid.NewGuid(), Name = "Eğirdir" },
                new() { Id = Guid.NewGuid(), Name = "Gelendost" },
                new() { Id = Guid.NewGuid(), Name = "Gönen" },
                new() { Id = Guid.NewGuid(), Name = "Keçiborlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Senirkent" },
                new() { Id = Guid.NewGuid(), Name = "Sütçüler" },
                new() { Id = Guid.NewGuid(), Name = "Uluborlu" },
                new() { Id = Guid.NewGuid(), Name = "Yalvaç" },
                new() { Id = Guid.NewGuid(), Name = "Yenişarbademli" },
                new() { Id = Guid.NewGuid(), Name = "Şarkikaraağaç" }
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
                new() { Id = Guid.NewGuid(), Name = "Aralık" },
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
                new() { Id = Guid.NewGuid(), Name = "Afşin" },
                new() { Id = Guid.NewGuid(), Name = "Andırın" },
                new() { Id = Guid.NewGuid(), Name = "Dulkadiroğlu" },
                new() { Id = Guid.NewGuid(), Name = "Ekinözü" },
                new() { Id = Guid.NewGuid(), Name = "Elbistan" },
                new() { Id = Guid.NewGuid(), Name = "Göksun" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nurhak" },
                new() { Id = Guid.NewGuid(), Name = "Onikişubat" },
                new() { Id = Guid.NewGuid(), Name = "Pazarcık" },
                new() { Id = Guid.NewGuid(), Name = "Türkoğlu" },
                new() { Id = Guid.NewGuid(), Name = "Çağlayancerit" }
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
                new() { Id = Guid.NewGuid(), Name = "Eflani" },
                new() { Id = Guid.NewGuid(), Name = "Eskipazar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ovacık" },
                new() { Id = Guid.NewGuid(), Name = "Safranbolu" },
                new() { Id = Guid.NewGuid(), Name = "Yenice" }
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
                new() { Id = Guid.NewGuid(), Name = "Ayrancı" },
                new() { Id = Guid.NewGuid(), Name = "Başyayla" },
                new() { Id = Guid.NewGuid(), Name = "Ermenek" },
                new() { Id = Guid.NewGuid(), Name = "Kazımkarabekir" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sarıveliler" }
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
                new() { Id = Guid.NewGuid(), Name = "Digor" },
                new() { Id = Guid.NewGuid(), Name = "Kağızman" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sarıkamış" },
                new() { Id = Guid.NewGuid(), Name = "Selim" },
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
                new() { Id = Guid.NewGuid(), Name = "Ağlı" },
                new() { Id = Guid.NewGuid(), Name = "Bozkurt" },
                new() { Id = Guid.NewGuid(), Name = "Cide" },
                new() { Id = Guid.NewGuid(), Name = "Daday" },
                new() { Id = Guid.NewGuid(), Name = "Devrekani" },
                new() { Id = Guid.NewGuid(), Name = "Doğanyurt" },
                new() { Id = Guid.NewGuid(), Name = "Hanönü" },
                new() { Id = Guid.NewGuid(), Name = "Küre" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pınarbaşı" },
                new() { Id = Guid.NewGuid(), Name = "Seydiler" },
                new() { Id = Guid.NewGuid(), Name = "Taşköprü" },
                new() { Id = Guid.NewGuid(), Name = "Tosya" },
                new() { Id = Guid.NewGuid(), Name = "Çatalzeytin" },
                new() { Id = Guid.NewGuid(), Name = "İhsangazi" },
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
                new() { Id = Guid.NewGuid(), Name = "Akkışla" },
                new() { Id = Guid.NewGuid(), Name = "Bünyan" },
                new() { Id = Guid.NewGuid(), Name = "Develi" },
                new() { Id = Guid.NewGuid(), Name = "Felahiye" },
                new() { Id = Guid.NewGuid(), Name = "Hacılar" },
                new() { Id = Guid.NewGuid(), Name = "Kocasinan" },
                new() { Id = Guid.NewGuid(), Name = "Melikgazi" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pınarbaşı" },
                new() { Id = Guid.NewGuid(), Name = "Sarıoğlan" },
                new() { Id = Guid.NewGuid(), Name = "Sarız" },
                new() { Id = Guid.NewGuid(), Name = "Talas" },
                new() { Id = Guid.NewGuid(), Name = "Tomarza" },
                new() { Id = Guid.NewGuid(), Name = "Yahyalı" },
                new() { Id = Guid.NewGuid(), Name = "Yeşilhisar" },
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
                new() { Id = Guid.NewGuid(), Name = "Bahşili" },
                new() { Id = Guid.NewGuid(), Name = "Balışeyh" },
                new() { Id = Guid.NewGuid(), Name = "Delice" },
                new() { Id = Guid.NewGuid(), Name = "Karakeçili" },
                new() { Id = Guid.NewGuid(), Name = "Keskin" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sulakyurt" },
                new() { Id = Guid.NewGuid(), Name = "Yahşihan" },
                new() { Id = Guid.NewGuid(), Name = "Çelebi" }
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
                new() { Id = Guid.NewGuid(), Name = "Babaeski" },
                new() { Id = Guid.NewGuid(), Name = "Demirköy" },
                new() { Id = Guid.NewGuid(), Name = "Kofçaz" },
                new() { Id = Guid.NewGuid(), Name = "Lüleburgaz" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pehlivanköy" },
                new() { Id = Guid.NewGuid(), Name = "Pınarhisar" },
                new() { Id = Guid.NewGuid(), Name = "Vize" }
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
                new() { Id = Guid.NewGuid(), Name = "Akpınar" },
                new() { Id = Guid.NewGuid(), Name = "Akçakent" },
                new() { Id = Guid.NewGuid(), Name = "Boztepe" },
                new() { Id = Guid.NewGuid(), Name = "Kaman" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mucur" },
                new() { Id = Guid.NewGuid(), Name = "Çiçekdağı" }
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
                new() { Id = Guid.NewGuid(), Name = "Başiskele" },
                new() { Id = Guid.NewGuid(), Name = "Darıca" },
                new() { Id = Guid.NewGuid(), Name = "Derince" },
                new() { Id = Guid.NewGuid(), Name = "Dilovası" },
                new() { Id = Guid.NewGuid(), Name = "Gebze" },
                new() { Id = Guid.NewGuid(), Name = "Gölcük" },
                new() { Id = Guid.NewGuid(), Name = "Kandıra" },
                new() { Id = Guid.NewGuid(), Name = "Karamürsel" },
                new() { Id = Guid.NewGuid(), Name = "Kartepe" },
                new() { Id = Guid.NewGuid(), Name = "Körfez" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Çayırova" },
                new() { Id = Guid.NewGuid(), Name = "İzmit" }
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
                new() { Id = Guid.NewGuid(), Name = "Ahırlı" },
                new() { Id = Guid.NewGuid(), Name = "Akören" },
                new() { Id = Guid.NewGuid(), Name = "Akşehir" },
                new() { Id = Guid.NewGuid(), Name = "Altınekin" },
                new() { Id = Guid.NewGuid(), Name = "Beyşehir" },
                new() { Id = Guid.NewGuid(), Name = "Bozkır" },
                new() { Id = Guid.NewGuid(), Name = "Cihanbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Derbent" },
                new() { Id = Guid.NewGuid(), Name = "Derebucak" },
                new() { Id = Guid.NewGuid(), Name = "Doğanhisar" },
                new() { Id = Guid.NewGuid(), Name = "Emirgazi" },
                new() { Id = Guid.NewGuid(), Name = "Ereğli" },
                new() { Id = Guid.NewGuid(), Name = "Güneysınır" },
                new() { Id = Guid.NewGuid(), Name = "Hadim" },
                new() { Id = Guid.NewGuid(), Name = "Halkapınar" },
                new() { Id = Guid.NewGuid(), Name = "Hüyük" },
                new() { Id = Guid.NewGuid(), Name = "Ilgın" },
                new() { Id = Guid.NewGuid(), Name = "Kadınhanı" },
                new() { Id = Guid.NewGuid(), Name = "Karapınar" },
                new() { Id = Guid.NewGuid(), Name = "Karatay" },
                new() { Id = Guid.NewGuid(), Name = "Kulu" },
                new() { Id = Guid.NewGuid(), Name = "Meram" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sarayönü" },
                new() { Id = Guid.NewGuid(), Name = "Selçuklu" },
                new() { Id = Guid.NewGuid(), Name = "Seydişehir" },
                new() { Id = Guid.NewGuid(), Name = "Taşkent" },
                new() { Id = Guid.NewGuid(), Name = "Tuzlukçu" },
                new() { Id = Guid.NewGuid(), Name = "Yalıhüyük" },
                new() { Id = Guid.NewGuid(), Name = "Yunak" },
                new() { Id = Guid.NewGuid(), Name = "Çeltik" },
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
                new() { Id = Guid.NewGuid(), Name = "Altıntaş" },
                new() { Id = Guid.NewGuid(), Name = "Aslanapa" },
                new() { Id = Guid.NewGuid(), Name = "Domaniç" },
                new() { Id = Guid.NewGuid(), Name = "Dumlupınar" },
                new() { Id = Guid.NewGuid(), Name = "Emet" },
                new() { Id = Guid.NewGuid(), Name = "Gediz" },
                new() { Id = Guid.NewGuid(), Name = "Hisarcık" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pazarlar" },
                new() { Id = Guid.NewGuid(), Name = "Simav" },
                new() { Id = Guid.NewGuid(), Name = "Tavşanlı" },
                new() { Id = Guid.NewGuid(), Name = "Çavdarhisar" },
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
                new() { Id = Guid.NewGuid(), Name = "Elbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Musabeyli" },
                new() { Id = Guid.NewGuid(), Name = "Polateli" }
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
                new() { Id = Guid.NewGuid(), Name = "Arapgir" },
                new() { Id = Guid.NewGuid(), Name = "Arguvan" },
                new() { Id = Guid.NewGuid(), Name = "Battalgazi" },
                new() { Id = Guid.NewGuid(), Name = "Darende" },
                new() { Id = Guid.NewGuid(), Name = "Doğanyol" },
                new() { Id = Guid.NewGuid(), Name = "Doğanşehir" },
                new() { Id = Guid.NewGuid(), Name = "Hekimhan" },
                new() { Id = Guid.NewGuid(), Name = "Kale" },
                new() { Id = Guid.NewGuid(), Name = "Kuluncak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pütürge" },
                new() { Id = Guid.NewGuid(), Name = "Yazıhan" },
                new() { Id = Guid.NewGuid(), Name = "Yeşilyurt" }
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
                new() { Id = Guid.NewGuid(), Name = "Ahmetli" },
                new() { Id = Guid.NewGuid(), Name = "Akhisar" },
                new() { Id = Guid.NewGuid(), Name = "Alaşehir" },
                new() { Id = Guid.NewGuid(), Name = "Demirci" },
                new() { Id = Guid.NewGuid(), Name = "Gölmarmara" },
                new() { Id = Guid.NewGuid(), Name = "Gördes" },
                new() { Id = Guid.NewGuid(), Name = "Kula" },
                new() { Id = Guid.NewGuid(), Name = "Köprübaşı" },
                new() { Id = Guid.NewGuid(), Name = "Kırkağaç" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Salihli" },
                new() { Id = Guid.NewGuid(), Name = "Saruhanlı" },
                new() { Id = Guid.NewGuid(), Name = "Sarıgöl" },
                new() { Id = Guid.NewGuid(), Name = "Selendi" },
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
                new() { Id = Guid.NewGuid(), Name = "Dargeçit" },
                new() { Id = Guid.NewGuid(), Name = "Derik" },
                new() { Id = Guid.NewGuid(), Name = "Kızıltepe" },
                new() { Id = Guid.NewGuid(), Name = "Mazıdağı" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Midyat" },
                new() { Id = Guid.NewGuid(), Name = "Nusaybin" },
                new() { Id = Guid.NewGuid(), Name = "Savur" },
                new() { Id = Guid.NewGuid(), Name = "Yeşilli" },
                new() { Id = Guid.NewGuid(), Name = "Ömerli" }
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
                new() { Id = Guid.NewGuid(), Name = "Akdeniz" },
                new() { Id = Guid.NewGuid(), Name = "Anamur" },
                new() { Id = Guid.NewGuid(), Name = "Aydıncık" },
                new() { Id = Guid.NewGuid(), Name = "Bozyazı" },
                new() { Id = Guid.NewGuid(), Name = "Erdemli" },
                new() { Id = Guid.NewGuid(), Name = "Gülnar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mezitli" },
                new() { Id = Guid.NewGuid(), Name = "Mut" },
                new() { Id = Guid.NewGuid(), Name = "Silifke" },
                new() { Id = Guid.NewGuid(), Name = "Tarsus" },
                new() { Id = Guid.NewGuid(), Name = "Toroslar" },
                new() { Id = Guid.NewGuid(), Name = "Yenişehir" },
                new() { Id = Guid.NewGuid(), Name = "Çamlıyayla" }
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
                new() { Id = Guid.NewGuid(), Name = "Fethiye" },
                new() { Id = Guid.NewGuid(), Name = "Kavaklıdere" },
                new() { Id = Guid.NewGuid(), Name = "Köyceğiz" },
                new() { Id = Guid.NewGuid(), Name = "Marmaris" },
                new() { Id = Guid.NewGuid(), Name = "Menteşe" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Milas" },
                new() { Id = Guid.NewGuid(), Name = "Ortaca" },
                new() { Id = Guid.NewGuid(), Name = "Seydikemer" },
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
                new() { Id = Guid.NewGuid(), Name = "Bulanık" },
                new() { Id = Guid.NewGuid(), Name = "Hasköy" },
                new() { Id = Guid.NewGuid(), Name = "Korkut" },
                new() { Id = Guid.NewGuid(), Name = "Malazgirt" },
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
                new() { Id = Guid.NewGuid(), Name = "Acıgöl" },
                new() { Id = Guid.NewGuid(), Name = "Avanos" },
                new() { Id = Guid.NewGuid(), Name = "Derinkuyu" },
                new() { Id = Guid.NewGuid(), Name = "Gülşehir" },
                new() { Id = Guid.NewGuid(), Name = "Hacıbektaş" },
                new() { Id = Guid.NewGuid(), Name = "Kozaklı" },
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
                new() { Id = Guid.NewGuid(), Name = "Altunhisar" },
                new() { Id = Guid.NewGuid(), Name = "Bor" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ulukışla" },
                new() { Id = Guid.NewGuid(), Name = "Çamardı" },
                new() { Id = Guid.NewGuid(), Name = "Çiftlik" }
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
                new() { Id = Guid.NewGuid(), Name = "Altınordu" },
                new() { Id = Guid.NewGuid(), Name = "Aybastı" },
                new() { Id = Guid.NewGuid(), Name = "Fatsa" },
                new() { Id = Guid.NewGuid(), Name = "Gölköy" },
                new() { Id = Guid.NewGuid(), Name = "Gülyalı" },
                new() { Id = Guid.NewGuid(), Name = "Gürgentepe" },
                new() { Id = Guid.NewGuid(), Name = "Kabadüz" },
                new() { Id = Guid.NewGuid(), Name = "Kabataş" },
                new() { Id = Guid.NewGuid(), Name = "Korgan" },
                new() { Id = Guid.NewGuid(), Name = "Kumru" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Mesudiye" },
                new() { Id = Guid.NewGuid(), Name = "Perşembe" },
                new() { Id = Guid.NewGuid(), Name = "Ulubey" },
                new() { Id = Guid.NewGuid(), Name = "Çamaş" },
                new() { Id = Guid.NewGuid(), Name = "Çatalpınar" },
                new() { Id = Guid.NewGuid(), Name = "Çaybaşı" },
                new() { Id = Guid.NewGuid(), Name = "Ünye" },
                new() { Id = Guid.NewGuid(), Name = "İkizce" }
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
                new() { Id = Guid.NewGuid(), Name = "Düziçi" },
                new() { Id = Guid.NewGuid(), Name = "Hasanbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Kadirli" },
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
                new() { Id = Guid.NewGuid(), Name = "Derepazarı" },
                new() { Id = Guid.NewGuid(), Name = "Fındıklı" },
                new() { Id = Guid.NewGuid(), Name = "Güneysu" },
                new() { Id = Guid.NewGuid(), Name = "Hemşin" },
                new() { Id = Guid.NewGuid(), Name = "Kalkandere" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pazar" },
                new() { Id = Guid.NewGuid(), Name = "Çamlıhemşin" },
                new() { Id = Guid.NewGuid(), Name = "Çayeli" },
                new() { Id = Guid.NewGuid(), Name = "İkizdere" },
                new() { Id = Guid.NewGuid(), Name = "İyidere" }
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
                new() { Id = Guid.NewGuid(), Name = "Adapazarı" },
                new() { Id = Guid.NewGuid(), Name = "Akyazı" },
                new() { Id = Guid.NewGuid(), Name = "Arifiye" },
                new() { Id = Guid.NewGuid(), Name = "Erenler" },
                new() { Id = Guid.NewGuid(), Name = "Ferizli" },
                new() { Id = Guid.NewGuid(), Name = "Geyve" },
                new() { Id = Guid.NewGuid(), Name = "Hendek" },
                new() { Id = Guid.NewGuid(), Name = "Karapürçek" },
                new() { Id = Guid.NewGuid(), Name = "Karasu" },
                new() { Id = Guid.NewGuid(), Name = "Kaynarca" },
                new() { Id = Guid.NewGuid(), Name = "Kocaali" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pamukova" },
                new() { Id = Guid.NewGuid(), Name = "Sapanca" },
                new() { Id = Guid.NewGuid(), Name = "Serdivan" },
                new() { Id = Guid.NewGuid(), Name = "Söğütlü" },
                new() { Id = Guid.NewGuid(), Name = "Taraklı" }
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
                new() { Id = Guid.NewGuid(), Name = "Ondokuz Mayıs" },
                new() { Id = Guid.NewGuid(), Name = "Alaçam" },
                new() { Id = Guid.NewGuid(), Name = "Asarcık" },
                new() { Id = Guid.NewGuid(), Name = "Atakum" },
                new() { Id = Guid.NewGuid(), Name = "Ayvacık" },
                new() { Id = Guid.NewGuid(), Name = "Bafra" },
                new() { Id = Guid.NewGuid(), Name = "Canik" },
                new() { Id = Guid.NewGuid(), Name = "Havza" },
                new() { Id = Guid.NewGuid(), Name = "Kavak" },
                new() { Id = Guid.NewGuid(), Name = "Ladik" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Salıpazarı" },
                new() { Id = Guid.NewGuid(), Name = "Tekkeköy" },
                new() { Id = Guid.NewGuid(), Name = "Terme" },
                new() { Id = Guid.NewGuid(), Name = "Vezirköprü" },
                new() { Id = Guid.NewGuid(), Name = "Yakakent" },
                new() { Id = Guid.NewGuid(), Name = "Çarşamba" },
                new() { Id = Guid.NewGuid(), Name = "İlkadım" }
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
                new() { Id = Guid.NewGuid(), Name = "Ayancık" },
                new() { Id = Guid.NewGuid(), Name = "Boyabat" },
                new() { Id = Guid.NewGuid(), Name = "Dikmen" },
                new() { Id = Guid.NewGuid(), Name = "Durağan" },
                new() { Id = Guid.NewGuid(), Name = "Erfelek" },
                new() { Id = Guid.NewGuid(), Name = "Gerze" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Saraydüzü" },
                new() { Id = Guid.NewGuid(), Name = "Türkeli" }
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
                new() { Id = Guid.NewGuid(), Name = "Akıncılar" },
                new() { Id = Guid.NewGuid(), Name = "Altınyayla" },
                new() { Id = Guid.NewGuid(), Name = "Divriği" },
                new() { Id = Guid.NewGuid(), Name = "Doğanşar" },
                new() { Id = Guid.NewGuid(), Name = "Gemerek" },
                new() { Id = Guid.NewGuid(), Name = "Gölova" },
                new() { Id = Guid.NewGuid(), Name = "Gürün" },
                new() { Id = Guid.NewGuid(), Name = "Hafik" },
                new() { Id = Guid.NewGuid(), Name = "Kangal" },
                new() { Id = Guid.NewGuid(), Name = "Koyulhisar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Suşehri" },
                new() { Id = Guid.NewGuid(), Name = "Ulaş" },
                new() { Id = Guid.NewGuid(), Name = "Yıldızeli" },
                new() { Id = Guid.NewGuid(), Name = "Zara" },
                new() { Id = Guid.NewGuid(), Name = "İmranlı" },
                new() { Id = Guid.NewGuid(), Name = "Şarkışla" }
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
                new() { Id = Guid.NewGuid(), Name = "Pervari" },
                new() { Id = Guid.NewGuid(), Name = "Tillo" },
                new() { Id = Guid.NewGuid(), Name = "Şirvan" }
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
                new() { Id = Guid.NewGuid(), Name = "Kapaklı" },
                new() { Id = Guid.NewGuid(), Name = "Malkara" },
                new() { Id = Guid.NewGuid(), Name = "Marmaraereğlisi" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muratlı" },
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
                new() { Id = Guid.NewGuid(), Name = "Başçiftlik" },
                new() { Id = Guid.NewGuid(), Name = "Erbaa" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Niksar" },
                new() { Id = Guid.NewGuid(), Name = "Pazar" },
                new() { Id = Guid.NewGuid(), Name = "Reşadiye" },
                new() { Id = Guid.NewGuid(), Name = "Sulusaray" },
                new() { Id = Guid.NewGuid(), Name = "Turhal" },
                new() { Id = Guid.NewGuid(), Name = "Yeşilyurt" },
                new() { Id = Guid.NewGuid(), Name = "Zile" }
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
                new() { Id = Guid.NewGuid(), Name = "Araklı" },
                new() { Id = Guid.NewGuid(), Name = "Arsin" },
                new() { Id = Guid.NewGuid(), Name = "Beşikdüzü" },
                new() { Id = Guid.NewGuid(), Name = "Dernekpazarı" },
                new() { Id = Guid.NewGuid(), Name = "Düzköy" },
                new() { Id = Guid.NewGuid(), Name = "Hayrat" },
                new() { Id = Guid.NewGuid(), Name = "Köprübaşı" },
                new() { Id = Guid.NewGuid(), Name = "Maçka" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Of" },
                new() { Id = Guid.NewGuid(), Name = "Ortahisar" },
                new() { Id = Guid.NewGuid(), Name = "Sürmene" },
                new() { Id = Guid.NewGuid(), Name = "Tonya" },
                new() { Id = Guid.NewGuid(), Name = "Vakfıkebir" },
                new() { Id = Guid.NewGuid(), Name = "Yomra" },
                new() { Id = Guid.NewGuid(), Name = "Çarşıbaşı" },
                new() { Id = Guid.NewGuid(), Name = "Çaykara" },
                new() { Id = Guid.NewGuid(), Name = "Şalpazarı" }
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
                new() { Id = Guid.NewGuid(), Name = "Mazgirt" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Nazımiye" },
                new() { Id = Guid.NewGuid(), Name = "Ovacık" },
                new() { Id = Guid.NewGuid(), Name = "Pertek" },
                new() { Id = Guid.NewGuid(), Name = "Pülümür" },
                new() { Id = Guid.NewGuid(), Name = "Çemişgezek" }
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
                new() { Id = Guid.NewGuid(), Name = "Karahallı" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Sivaslı" },
                new() { Id = Guid.NewGuid(), Name = "Ulubey" },
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
                new() { Id = Guid.NewGuid(), Name = "Edremit" },
                new() { Id = Guid.NewGuid(), Name = "Erciş" },
                new() { Id = Guid.NewGuid(), Name = "Gevaş" },
                new() { Id = Guid.NewGuid(), Name = "Gürpınar" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Muradiye" },
                new() { Id = Guid.NewGuid(), Name = "Saray" },
                new() { Id = Guid.NewGuid(), Name = "Tuşba" },
                new() { Id = Guid.NewGuid(), Name = "Çaldıran" },
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
                new() { Id = Guid.NewGuid(), Name = "Altınova" },
                new() { Id = Guid.NewGuid(), Name = "Armutlu" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Termal" },
                new() { Id = Guid.NewGuid(), Name = "Çiftlikköy" },
                new() { Id = Guid.NewGuid(), Name = "Çınarcık" }
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
                new() { Id = Guid.NewGuid(), Name = "Akdağmadeni" },
                new() { Id = Guid.NewGuid(), Name = "Aydıncık" },
                new() { Id = Guid.NewGuid(), Name = "Boğazlıyan" },
                new() { Id = Guid.NewGuid(), Name = "Kadışehri" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Saraykent" },
                new() { Id = Guid.NewGuid(), Name = "Sarıkaya" },
                new() { Id = Guid.NewGuid(), Name = "Sorgun" },
                new() { Id = Guid.NewGuid(), Name = "Yenifakılı" },
                new() { Id = Guid.NewGuid(), Name = "Yerköy" },
                new() { Id = Guid.NewGuid(), Name = "Çandır" },
                new() { Id = Guid.NewGuid(), Name = "Çayıralan" },
                new() { Id = Guid.NewGuid(), Name = "Çekerek" },
                new() { Id = Guid.NewGuid(), Name = "Şefaatli" }
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
                new() { Id = Guid.NewGuid(), Name = "Alaplı" },
                new() { Id = Guid.NewGuid(), Name = "Devrek" },
                new() { Id = Guid.NewGuid(), Name = "Ereğli" },
                new() { Id = Guid.NewGuid(), Name = "Gökçebey" },
                new() { Id = Guid.NewGuid(), Name = "Kilimli" },
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
                new() { Id = Guid.NewGuid(), Name = "Ayvacık" },
                new() { Id = Guid.NewGuid(), Name = "Bayramiç" },
                new() { Id = Guid.NewGuid(), Name = "Biga" },
                new() { Id = Guid.NewGuid(), Name = "Bozcaada" },
                new() { Id = Guid.NewGuid(), Name = "Eceabat" },
                new() { Id = Guid.NewGuid(), Name = "Ezine" },
                new() { Id = Guid.NewGuid(), Name = "Gelibolu" },
                new() { Id = Guid.NewGuid(), Name = "Gökçeada" },
                new() { Id = Guid.NewGuid(), Name = "Lapseki" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Yenice" },
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
                new() { Id = Guid.NewGuid(), Name = "Eldivan" },
                new() { Id = Guid.NewGuid(), Name = "Ilgaz" },
                new() { Id = Guid.NewGuid(), Name = "Korgun" },
                new() { Id = Guid.NewGuid(), Name = "Kurşunlu" },
                new() { Id = Guid.NewGuid(), Name = "Kızılırmak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Orta" },
                new() { Id = Guid.NewGuid(), Name = "Yapraklı" },
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
                new() { Id = Guid.NewGuid(), Name = "Kargı" },
                new() { Id = Guid.NewGuid(), Name = "Laçin" },
                new() { Id = Guid.NewGuid(), Name = "Mecitözü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Ortaköy" },
                new() { Id = Guid.NewGuid(), Name = "Osmancık" },
                new() { Id = Guid.NewGuid(), Name = "Oğuzlar" },
                new() { Id = Guid.NewGuid(), Name = "Sungurlu" },
                new() { Id = Guid.NewGuid(), Name = "Uğurludağ" },
                new() { Id = Guid.NewGuid(), Name = "İskilip" }
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
                new() { Id = Guid.NewGuid(), Name = "Ataşehir" },
                new() { Id = Guid.NewGuid(), Name = "Avcılar" },
                new() { Id = Guid.NewGuid(), Name = "Bahçelievler" },
                new() { Id = Guid.NewGuid(), Name = "Bakırköy" },
                new() { Id = Guid.NewGuid(), Name = "Bayrampaşa" },
                new() { Id = Guid.NewGuid(), Name = "Bağcılar" },
                new() { Id = Guid.NewGuid(), Name = "Başakşehir" },
                new() { Id = Guid.NewGuid(), Name = "Beykoz" },
                new() { Id = Guid.NewGuid(), Name = "Beylikdüzü" },
                new() { Id = Guid.NewGuid(), Name = "Beyoğlu" },
                new() { Id = Guid.NewGuid(), Name = "Beşiktaş" },
                new() { Id = Guid.NewGuid(), Name = "Büyükçekmece" },
                new() { Id = Guid.NewGuid(), Name = "Eminönü" },
                new() { Id = Guid.NewGuid(), Name = "Esenler" },
                new() { Id = Guid.NewGuid(), Name = "Esenyurt" },
                new() { Id = Guid.NewGuid(), Name = "Eyüpsultan" },
                new() { Id = Guid.NewGuid(), Name = "Fatih" },
                new() { Id = Guid.NewGuid(), Name = "Gaziosmanpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Güngören" },
                new() { Id = Guid.NewGuid(), Name = "Kadıköy" },
                new() { Id = Guid.NewGuid(), Name = "Kartal" },
                new() { Id = Guid.NewGuid(), Name = "Kağıthane" },
                new() { Id = Guid.NewGuid(), Name = "Küçükçekmece" },
                new() { Id = Guid.NewGuid(), Name = "Maltepe" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Pendik" },
                new() { Id = Guid.NewGuid(), Name = "Sancaktepe" },
                new() { Id = Guid.NewGuid(), Name = "Sarıyer" },
                new() { Id = Guid.NewGuid(), Name = "Silivri" },
                new() { Id = Guid.NewGuid(), Name = "Sultanbeyli" },
                new() { Id = Guid.NewGuid(), Name = "Sultangazi" },
                new() { Id = Guid.NewGuid(), Name = "Tuzla" },
                new() { Id = Guid.NewGuid(), Name = "Zeytinburnu" },
                new() { Id = Guid.NewGuid(), Name = "Çatalca" },
                new() { Id = Guid.NewGuid(), Name = "Çekmeköy" },
                new() { Id = Guid.NewGuid(), Name = "Ümraniye" },
                new() { Id = Guid.NewGuid(), Name = "Üsküdar" },
                new() { Id = Guid.NewGuid(), Name = "Şile" },
                new() { Id = Guid.NewGuid(), Name = "Şişli" }
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
                new() { Id = Guid.NewGuid(), Name = "Aliağa" },
                new() { Id = Guid.NewGuid(), Name = "Balçova" },
                new() { Id = Guid.NewGuid(), Name = "Bayraklı" },
                new() { Id = Guid.NewGuid(), Name = "Bayındır" },
                new() { Id = Guid.NewGuid(), Name = "Bergama" },
                new() { Id = Guid.NewGuid(), Name = "Beydağ" },
                new() { Id = Guid.NewGuid(), Name = "Bornova" },
                new() { Id = Guid.NewGuid(), Name = "Buca" },
                new() { Id = Guid.NewGuid(), Name = "Dikili" },
                new() { Id = Guid.NewGuid(), Name = "Foça" },
                new() { Id = Guid.NewGuid(), Name = "Gaziemir" },
                new() { Id = Guid.NewGuid(), Name = "Güzelbahçe" },
                new() { Id = Guid.NewGuid(), Name = "Karabağlar" },
                new() { Id = Guid.NewGuid(), Name = "Karaburun" },
                new() { Id = Guid.NewGuid(), Name = "Karşıyaka" },
                new() { Id = Guid.NewGuid(), Name = "Kemalpaşa" },
                new() { Id = Guid.NewGuid(), Name = "Kiraz" },
                new() { Id = Guid.NewGuid(), Name = "Konak" },
                new() { Id = Guid.NewGuid(), Name = "Kınık" },
                new() { Id = Guid.NewGuid(), Name = "Menderes" },
                new() { Id = Guid.NewGuid(), Name = "Menemen" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Narlıdere" },
                new() { Id = Guid.NewGuid(), Name = "Seferihisar" },
                new() { Id = Guid.NewGuid(), Name = "Selçuk" },
                new() { Id = Guid.NewGuid(), Name = "Tire" },
                new() { Id = Guid.NewGuid(), Name = "Torbalı" },
                new() { Id = Guid.NewGuid(), Name = "Urla" },
                new() { Id = Guid.NewGuid(), Name = "Çeşme" },
                new() { Id = Guid.NewGuid(), Name = "Çiğli" },
                new() { Id = Guid.NewGuid(), Name = "Ödemiş" }
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
                new() { Id = Guid.NewGuid(), Name = "Birecik" },
                new() { Id = Guid.NewGuid(), Name = "Bozova" },
                new() { Id = Guid.NewGuid(), Name = "Ceylanpınar" },
                new() { Id = Guid.NewGuid(), Name = "Eyyübiye" },
                new() { Id = Guid.NewGuid(), Name = "Halfeti" },
                new() { Id = Guid.NewGuid(), Name = "Haliliye" },
                new() { Id = Guid.NewGuid(), Name = "Harran" },
                new() { Id = Guid.NewGuid(), Name = "Hilvan" },
                new() { Id = Guid.NewGuid(), Name = "Karaköprü" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Siverek" },
                new() { Id = Guid.NewGuid(), Name = "Suruç" },
                new() { Id = Guid.NewGuid(), Name = "Viranşehir" }
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
                new() { Id = Guid.NewGuid(), Name = "Cizre" },
                new() { Id = Guid.NewGuid(), Name = "Güçlükonak" },
                new() { Id = Guid.NewGuid(), Name = "Merkez" },
                new() { Id = Guid.NewGuid(), Name = "Silopi" },
                new() { Id = Guid.NewGuid(), Name = "Uludere" },
                new() { Id = Guid.NewGuid(), Name = "İdil" }
            }
        };

        var allCities = new List<City> { adana, adiyaman, afyonkarahi̇sar, aksaray, amasya, ankara, antalya, ardahan, artvi̇n, aydin, agri, balikesi̇r, bartin, batman, bayburt, bolu, burdur, bursa, bi̇leci̇k, bi̇ngol, bi̇tli̇s, deni̇zli̇, duzce, di̇yarbakir, edi̇rne, elazig, erzurum, erzi̇ncan, eski̇sehi̇r, gazi̇antep, gumushane, gi̇resun, hakkari̇, hatay, isparta, igdir, kahramanmaras, karabuk, karaman, kars, kastamonu, kayseri̇, kirikkale, kirklareli̇, kirsehi̇r, kocaeli̇, konya, kutahya, ki̇li̇s, malatya, mani̇sa, mardi̇n, mersi̇n, mugla, mus, nevsehi̇r, ni̇gde, ordu, osmani̇ye, ri̇ze, sakarya, samsun, si̇nop, si̇vas, si̇i̇rt, teki̇rdag, tokat, trabzon, tunceli̇, usak, van, yalova, yozgat, zonguldak, canakkale, cankiri, corum, i̇stanbul, i̇zmi̇r, sanliurfa, sirnak };

        await db.Cities.AddRangeAsync(allCities, ct);
        await db.SaveChangesAsync(ct);
    }
}
