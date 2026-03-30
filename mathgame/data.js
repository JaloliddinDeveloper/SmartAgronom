// ============================================================
//  data.js  —  Educational Quiz Platform Data
//  Globals: window.GEO_DATA | window.HISTORY_DATA | window.ENGLISH_DATA
// ============================================================

// ============================================================
//  GEOGRAPHY DATA
//  Format: [uzName, capital, continent, independence, flagEmoji]
// ============================================================
window.GEO_DATA = {
  countries: [
    // ── OSIYO (Asia) ──────────────────────────────────────────
    ["O'zbekiston",          "Toshkent",        "Osiyo",            "1991", "🇺🇿"],
    ["Qozog'iston",          "Ostona",          "Osiyo",            "1991", "🇰🇿"],
    ["Qirg'iziston",         "Bishkek",         "Osiyo",            "1991", "🇰🇬"],
    ["Tojikiston",           "Dushanbe",        "Osiyo",            "1991", "🇹🇯"],
    ["Turkmaniston",         "Ashxobod",        "Osiyo",            "1991", "🇹🇲"],
    ["Rossiya",              "Moskva",          "Osiyo/Yevropa",    "1991", "🇷🇺"],
    ["Xitoy",                "Pekin",           "Osiyo",            "221",  "🇨🇳"],
    ["Yaponiya",             "Tokio",           "Osiyo",            "660",  "🇯🇵"],
    ["Janubiy Koreya",       "Seul",            "Osiyo",            "1948", "🇰🇷"],
    ["Shimoliy Koreya",      "Pyongyang",       "Osiyo",            "1948", "🇰🇵"],
    ["Hindiston",            "Nyu-Dehli",       "Osiyo",            "1947", "🇮🇳"],
    ["Pokiston",             "Islomobod",       "Osiyo",            "1947", "🇵🇰"],
    ["Afg'oniston",          "Kobul",           "Osiyo",            "1919", "🇦🇫"],
    ["Eron",                 "Tehron",          "Osiyo",            "1979", "🇮🇷"],
    ["Iroq",                 "Bag'dod",         "Osiyo",            "1932", "🇮🇶"],
    ["Saudiya Arabistoni",   "Ar-Riyod",        "Osiyo",            "1932", "🇸🇦"],
    ["BAA",                  "Abu-Dabi",        "Osiyo",            "1971", "🇦🇪"],
    ["Turkiya",              "Anqara",          "Osiyo/Yevropa",    "1923", "🇹🇷"],
    ["Ozarbayjon",           "Boku",            "Osiyo",            "1991", "🇦🇿"],
    ["Armaniston",           "Yerevan",         "Osiyo",            "1991", "🇦🇲"],
    ["Gruziya",              "Tbilisi",         "Osiyo",            "1991", "🇬🇪"],
    ["Isroil",               "Quddus",          "Osiyo",            "1948", "🇮🇱"],
    ["Iordaniya",            "Ammon",           "Osiyo",            "1946", "🇯🇴"],
    ["Suriya",               "Damashq",         "Osiyo",            "1946", "🇸🇾"],
    ["Livan",                "Bayrut",          "Osiyo",            "1943", "🇱🇧"],
    ["Yaman",                "Sana",            "Osiyo",            "1967", "🇾🇪"],
    ["Ummon",                "Maskat",          "Osiyo",            "1951", "🇴🇲"],
    ["Qatar",                "Doha",            "Osiyo",            "1971", "🇶🇦"],
    ["Quvayt",               "Quvayt shahri",   "Osiyo",            "1961", "🇰🇼"],
    ["Bahrayn",              "Manoma",          "Osiyo",            "1971", "🇧🇭"],
    ["Vyetnam",              "Xanoy",           "Osiyo",            "1945", "🇻🇳"],
    ["Tailand",              "Bangkok",         "Osiyo",            "1238", "🇹🇭"],
    ["Indoneziya",           "Jakarta",         "Osiyo",            "1945", "🇮🇩"],
    ["Malayziya",            "Kuala-Lumpur",    "Osiyo",            "1957", "🇲🇾"],
    ["Filippin",             "Manila",          "Osiyo",            "1946", "🇵🇭"],
    ["Singapur",             "Singapur",        "Osiyo",            "1965", "🇸🇬"],
    ["Mo'g'uliston",         "Ulan-Bator",      "Osiyo",            "1921", "🇲🇳"],
    ["Bangladesh",           "Daka",            "Osiyo",            "1971", "🇧🇩"],
    ["Shri-Lanka",           "Kolombo",         "Osiyo",            "1948", "🇱🇰"],
    ["Nepal",                "Katmandu",        "Osiyo",            "1768", "🇳🇵"],
    ["Myanma",               "Neypido",         "Osiyo",            "1948", "🇲🇲"],
    ["Kambodja",             "Pnompen",         "Osiyo",            "1953", "🇰🇭"],
    ["Laos",                 "Vyentyan",        "Osiyo",            "1953", "🇱🇦"],
    ["Butan",                "Thimpu",          "Osiyo",            "1907", "🇧🇹"],
    ["Bruney",               "Bandar Seri Begavan", "Osiyo",        "1984", "🇧🇳"],
    ["Maldiv",               "Male",            "Osiyo",            "1965", "🇲🇻"],
    ["Timor-Leste",          "Dili",            "Osiyo",            "2002", "🇹🇱"],
    ["Kipr",                 "Nikosiya",        "Osiyo",            "1960", "🇨🇾"],

    // ── YEVROPA (Europe) ─────────────────────────────────────
    ["Germaniya",            "Berlin",          "Yevropa",          "1990", "🇩🇪"],
    ["Fransiya",             "Parij",           "Yevropa",          "843",  "🇫🇷"],
    ["Buyuk Britaniya",      "London",          "Yevropa",          "927",  "🇬🇧"],
    ["Italiya",              "Rim",             "Yevropa",          "1861", "🇮🇹"],
    ["Ispaniya",             "Madrid",          "Yevropa",          "1479", "🇪🇸"],
    ["Portugaliya",          "Lissabon",        "Yevropa",          "1143", "🇵🇹"],
    ["Niderlandiya",         "Amsterdam",       "Yevropa",          "1581", "🇳🇱"],
    ["Belgiya",              "Bryussel",        "Yevropa",          "1830", "🇧🇪"],
    ["Shveytsariya",         "Bern",            "Yevropa",          "1291", "🇨🇭"],
    ["Avstriya",             "Vena",            "Yevropa",          "1955", "🇦🇹"],
    ["Shvetsiya",            "Stokgolm",        "Yevropa",          "1523", "🇸🇪"],
    ["Norvegiya",            "Oslo",            "Yevropa",          "1905", "🇳🇴"],
    ["Daniya",               "Kopengagen",      "Yevropa",          "980",  "🇩🇰"],
    ["Finlandiya",           "Xelsinki",        "Yevropa",          "1917", "🇫🇮"],
    ["Polsha",               "Varshava",        "Yevropa",          "1918", "🇵🇱"],
    ["Chexiya",              "Praga",           "Yevropa",          "1993", "🇨🇿"],
    ["Slovakiya",            "Bratislava",      "Yevropa",          "1993", "🇸🇰"],
    ["Vengriya",             "Budapesht",       "Yevropa",          "1918", "🇭🇺"],
    ["Ruminiya",             "Buxarest",        "Yevropa",          "1877", "🇷🇴"],
    ["Bolgariya",            "Sofiya",          "Yevropa",          "1908", "🇧🇬"],
    ["Gretsiya",             "Afina",           "Yevropa",          "1821", "🇬🇷"],
    ["Serbiya",              "Belgrad",         "Yevropa",          "2006", "🇷🇸"],
    ["Xorvatiya",            "Zagreb",          "Yevropa",          "1991", "🇭🇷"],
    ["Ukraina",              "Kiyev",           "Yevropa",          "1991", "🇺🇦"],
    ["Belarus",              "Minsk",           "Yevropa",          "1991", "🇧🇾"],
    ["Latviya",              "Riga",            "Yevropa",          "1991", "🇱🇻"],
    ["Litva",                "Vilnyus",         "Yevropa",          "1990", "🇱🇹"],
    ["Estoniya",             "Tallin",          "Yevropa",          "1991", "🇪🇪"],
    ["Islandiya",            "Reykjavik",       "Yevropa",          "1944", "🇮🇸"],
    ["Irlandiya",            "Dublin",          "Yevropa",          "1922", "🇮🇪"],
    ["Sloveniya",            "Lyublyano",       "Yevropa",          "1991", "🇸🇮"],
    ["Bosniya va Gersegovina", "Sarayevo",      "Yevropa",          "1992", "🇧🇦"],
    ["Albaniya",             "Tirana",          "Yevropa",          "1912", "🇦🇱"],
    ["Chernog'oriya",        "Podgoritsa",      "Yevropa",          "2006", "🇲🇪"],
    ["Moldova",              "Kishinev",        "Yevropa",          "1991", "🇲🇩"],
    ["Lyuksemburg",          "Lyuksemburg",     "Yevropa",          "1867", "🇱🇺"],
    ["Malta",                "Valletta",        "Yevropa",          "1964", "🇲🇹"],
    ["Shimoliy Makedoniya",  "Skopye",          "Yevropa",          "1991", "🇲🇰"],

    // ── SHIMOLIY AMERIKA (North America) ─────────────────────
    ["AQSH",                 "Vashington",      "Shimoliy Amerika", "1776", "🇺🇸"],
    ["Kanada",               "Ottava",          "Shimoliy Amerika", "1867", "🇨🇦"],
    ["Meksika",              "Mexiko-shahar",   "Shimoliy Amerika", "1821", "🇲🇽"],
    ["Kuba",                 "Gavana",          "Shimoliy Amerika", "1898", "🇨🇺"],
    ["Yamayka",              "Kingston",        "Shimoliy Amerika", "1962", "🇯🇲"],
    ["Gaiti",                "Port-o-Prens",    "Shimoliy Amerika", "1804", "🇭🇹"],
    ["Dominikan Respublikasi", "Santo-Domingo", "Shimoliy Amerika", "1844", "🇩🇴"],
    ["Kosta-Rika",           "San-Xose",        "Shimoliy Amerika", "1821", "🇨🇷"],
    ["Panama",               "Panama-shahar",   "Shimoliy Amerika", "1903", "🇵🇦"],
    ["Gvatemala",            "Gvatemala-shahar","Shimoliy Amerika", "1821", "🇬🇹"],
    ["Gonduras",             "Tegusigalpa",     "Shimoliy Amerika", "1821", "🇭🇳"],
    ["Salvador",             "San-Salvador",    "Shimoliy Amerika", "1821", "🇸🇻"],
    ["Nikaragua",            "Managua",         "Shimoliy Amerika", "1821", "🇳🇮"],

    // ── JANUBIY AMERIKA (South America) ──────────────────────
    ["Braziliya",            "Braziliya",       "Janubiy Amerika",  "1822", "🇧🇷"],
    ["Argentina",            "Buenos-Ayres",    "Janubiy Amerika",  "1816", "🇦🇷"],
    ["Chili",                "Santyago",        "Janubiy Amerika",  "1818", "🇨🇱"],
    ["Peru",                 "Lima",            "Janubiy Amerika",  "1821", "🇵🇪"],
    ["Kolumbiya",            "Bogota",          "Janubiy Amerika",  "1810", "🇨🇴"],
    ["Venesuela",            "Karakas",         "Janubiy Amerika",  "1811", "🇻🇪"],
    ["Ekvador",              "Kito",            "Janubiy Amerika",  "1822", "🇪🇨"],
    ["Boliviya",             "Sukre",           "Janubiy Amerika",  "1825", "🇧🇴"],
    ["Paragvay",             "Asunsyon",        "Janubiy Amerika",  "1811", "🇵🇾"],
    ["Urugvay",              "Montevideo",      "Janubiy Amerika",  "1825", "🇺🇾"],
    ["Gayana",               "Jorjtaun",        "Janubiy Amerika",  "1966", "🇬🇾"],
    ["Surinam",              "Paramaribo",      "Janubiy Amerika",  "1975", "🇸🇷"],

    // ── AFRIKA (Africa) ──────────────────────────────────────
    ["Misr",                 "Qohira",          "Afrika",           "1922", "🇪🇬"],
    ["Janubiy Afrika",       "Pretoriya",       "Afrika",           "1910", "🇿🇦"],
    ["Nigeriya",             "Abujo",           "Afrika",           "1960", "🇳🇬"],
    ["Efiopiya",             "Addis-Abeba",     "Afrika",           "1137", "🇪🇹"],
    ["Keniya",               "Nayrobi",         "Afrika",           "1963", "🇰🇪"],
    ["Tanzaniya",            "Dodoma",          "Afrika",           "1961", "🇹🇿"],
    ["Marokash",             "Rabat",           "Afrika",           "1956", "🇲🇦"],
    ["Jazoir",               "Jazoir",          "Afrika",           "1962", "🇩🇿"],
    ["Tunis",                "Tunis",           "Afrika",           "1956", "🇹🇳"],
    ["Gana",                 "Akkra",           "Afrika",           "1957", "🇬🇭"],
    ["Senegal",              "Dakar",           "Afrika",           "1960", "🇸🇳"],
    ["Angola",               "Luanda",          "Afrika",           "1975", "🇦🇴"],
    ["Zimbabve",             "Xarare",          "Afrika",           "1980", "🇿🇼"],
    ["Mozambik",             "Maputo",          "Afrika",           "1975", "🇲🇿"],
    ["Uganda",               "Kampala",         "Afrika",           "1962", "🇺🇬"],
    ["Sudan",                "Xartum",          "Afrika",           "1956", "🇸🇩"],
    ["Somali",               "Mogadishu",       "Afrika",           "1960", "🇸🇴"],
    ["Liviya",               "Tripoli",         "Afrika",           "1951", "🇱🇾"],
    ["Madagaskar",           "Antananarivo",    "Afrika",           "1960", "🇲🇬"],
    ["Kamerun",              "Yaunde",          "Afrika",           "1960", "🇨🇲"],
    ["Kongo DR",             "Kinshasa",        "Afrika",           "1960", "🇨🇩"],
    ["Kongo",                "Brazzavil",       "Afrika",           "1960", "🇨🇬"],
    ["Ruanda",               "Kigali",          "Afrika",           "1962", "🇷🇼"],
    ["Zambiya",              "Lusaka",          "Afrika",           "1964", "🇿🇲"],
    ["Namibiya",             "Vindhuk",         "Afrika",           "1990", "🇳🇦"],
    ["Botsvana",             "Gaborone",        "Afrika",           "1966", "🇧🇼"],
    ["Mali",                 "Bamako",          "Afrika",           "1960", "🇲🇱"],
    ["Niger",                "Niamey",          "Afrika",           "1960", "🇳🇪"],
    ["Chad",                 "N'Djamena",       "Afrika",           "1960", "🇹🇩"],
    ["Kot-d'Ivuar",          "Yamussukro",      "Afrika",           "1960", "🇨🇮"],

    // ── OKEANIYA (Oceania) ────────────────────────────────────
    ["Avstraliya",           "Kanberra",        "Okeaniya",         "1901", "🇦🇺"],
    ["Yangi Zelandiya",      "Wellington",      "Okeaniya",         "1907", "🇳🇿"],
    ["Fiji",                 "Suva",            "Okeaniya",         "1970", "🇫🇯"],
    ["Papua Yangi Gvineya",  "Port Morsbi",     "Okeaniya",         "1975", "🇵🇬"],
    ["Samoa",                "Apia",            "Okeaniya",         "1962", "🇼🇸"],
    ["Tonga",                "Nukualofa",       "Okeaniya",         "1970", "🇹🇴"],
    ["Vanuatu",              "Port Vila",       "Okeaniya",         "1980", "🇻🇺"]
  ]
};


// ============================================================
//  HISTORY DATA — O'zbekiston tarixi (220+ savollar)
// ============================================================
window.HISTORY_DATA = {
  questions: [

    // ══════════════════════════════════════════════════════
    //  1. QADIM DAVR
    // ══════════════════════════════════════════════════════
    {q:"Qadimgi Markaziy Osiyo hududida necha ming yil avval dastlabki sivilizatsiyalar paydo bo'lgan?",
     o:["1 ming yil","3 ming yil","5 ming yildan ortiq","500 yil"],c:2},

    {q:"Qadimgi Baqtriya davlati qaysi hududda joylashgan edi?",
     o:["Shimoliy Hindiston","Janubiy O'zbekiston va Shimoliy Afg'oniston","Eron platosi","Kaspiy bo'yi"],c:1},

    {q:"Sug'd davlati qaysi vodiyda gullab-yashnagan?",
     o:["Farg'ona vodiysi","Zarafshon vodiysi","Amudaryo vodiysi","Sirdaryo vodiysi"],c:1},

    {q:"Aleksandr Makedonskiy Markaziy Osiyoga qachon bostirib keldi?",
     o:["Miloddan avvalgi 334 yilda","Miloddan avvalgi 329 yilda","Miloddan avvalgi 312 yilda","Miloddan avvalgi 356 yilda"],c:1},

    {q:"Aleksandr Makedonskiy Samarqandni qanday nomlagan?",
     o:["Afrosiob","Marakanda","Sogdiana","Transoksiana"],c:1},

    {q:"Kushon shohligi qachon tashkil topgan?",
     o:["Miloddan avvalgi 200 yil","Milodiy I asr","Miloddan avvalgi 500 yil","Milodiy III asr"],c:1},

    {q:"Kushon shohligining eng mashhur hukmdori kim edi?",
     o:["Kanishka","Kujula Kadfiz","Vasishka","Huvishka"],c:0},

    {q:"Eftalitlar (Oq Hunlar) davlati qachon mavjud bo'lgan?",
     o:["II–III asrlar","IV–VI asrlar","VII–VIII asrlar","I–II asrlar"],c:1},

    {q:"Parfiya shohligi qaysi hududni o'z ichiga olgan?",
     o:["Hozirgi Eron va Turkmaniston hududlari","Hozirgi O'zbekiston","Hozirgi Hindiston","Hozirgi Xitoy"],c:0},

    {q:"Qadimgi Xorazm davlati qaysi daryoning quyi oqimida joylashgan?",
     o:["Sirdaryo","Zarafshon","Amudaryo","Chu daryosi"],c:2},

    {q:"Markaziy Osiyo qadimda qanday nom bilan atalgan?",
     o:["Transoksiana yoki Movarounnahr","Mesopotamiya","Anatoliya","Levant"],c:0},

    {q:"Sug'd savdogarlari qaysi mashhur yo'l orqali savdo qilgan?",
     o:["Dengiz yo'li","Ipak yo'li","Sharq yo'li","Qoradeniz yo'li"],c:1},

    {q:"Qadimgi Farg'ona vodiysi qaysi nom bilan mashhur bo'lgan?",
     o:["Davan","Sogdiana","Baqtriya","Margiana"],c:0},

    {q:"Aleksandr Makedonskiy Markaziy Osiyoda necha yil qoldi?",
     o:["1 yil","2 yil","3–4 yil","6 yil"],c:2},

    {q:"Qadimgi Marv (Merv) shahri qaysi hududda joylashgan?",
     o:["Hozirgi Turkmaniston","Hozirgi O'zbekiston","Hozirgi Tojikiston","Hozirgi Afg'oniston"],c:0},

    {q:"Zardushtiylik dini qaysi hududda paydo bo'lgan?",
     o:["Arabiston yarim oroli","Markaziy Osiyo (Eron va O'zbekiston hududi)","Hindiston","Misr"],c:1},

    {q:"Avesto kitobi qaysi dinga tegishli muqaddas matn?",
     o:["Islom","Xristianlik","Zardushtiylik","Buddizm"],c:2},

    {q:"Qadimgi Nisa shahri qaysi davlatning poytaxti edi?",
     o:["Kushon","Parfiya","Baqtriya","Sug'd"],c:1},

    {q:"Ipak yo'li qachon ahamiyat kasb eta boshlagan?",
     o:["Miloddan avvalgi I asrda","Milodiy V asrda","Milodiy X asrda","Miloddan avvalgi V asrda"],c:0},

    {q:"Markaziy Osiyoda buddizm qaysi davr orqali tarqaldi?",
     o:["Arab fathi orqali","Kushon shohligi orqali","Mongol bosqini orqali","Somoniylar orqali"],c:1},

    {q:"Qadimgi 'Afrosiob' qaysi hozirgi shahar yaqinida joylashgan?",
     o:["Toshkent","Samarqand","Buxoro","Xiva"],c:1},

    {q:"Kushon imperiyasi o'zining cho'qqisida qaysi hududlarni o'z ichiga olgan?",
     o:["Faqat O'zbekiston","Markaziy Osiyo, Afg'oniston va Shimoliy Hindiston","Faqat Eron","Butun Osiyo"],c:1},

    {q:"Qadimgi Xorazm yozuvi qaysi alfavitga asoslangan edi?",
     o:["Arab","Arameya","Lotin","Yunoncha"],c:1},

    {q:"Miloddan avvalgi IV asrda Markaziy Osiyoga bostirib kelgan mashhur hukmdor kim edi?",
     o:["Kir II","Doro I","Aleksandr Makedonskiy","Xerks"],c:2},

    {q:"'Movarounnahr' so'zi nima degan ma'noni anglatadi?",
     o:["Tog' ortidagi joy","Daryo ortidagi joy (Amudaryo ortida)","Cho'l mamlakati","Sharq mamlakatlar"],c:1},

    // ══════════════════════════════════════════════════════
    //  2. O'RTA ASRLAR
    // ══════════════════════════════════════════════════════
    {q:"Arablar Movarounnahrni qachon fath qildi?",
     o:["VI asrda","VII–VIII asrlarda","IX asrda","X asrda"],c:1},

    {q:"Arab fathini amalga oshirgan asosiy qo'mondon kim edi?",
     o:["Abu Muslim","Qutayba ibn Muslim","Xolid ibn Valid","Umar ibn Xattob"],c:1},

    {q:"Somoniylar davlati qachon tashkil topdi?",
     o:["VII asrda","IX asrda","X asrda","VIII asrda"],c:1},

    {q:"Somoniylar davlatining poytaxti qaysi shahar edi?",
     o:["Samarqand","Termiz","Buxoro","Urganch"],c:2},

    {q:"Somoniylar davrida yashab, ijod etgan mashhur olim — Ibn Sino qaysi sohada dong taratdi?",
     o:["Matematika","Tibbiyot va falsafa","Astronomiya","Tarix"],c:1},

    {q:"Abu Ali ibn Sino qachon tug'ilgan?",
     o:["870 yilda","980 yilda","1048 yilda","1100 yilda"],c:1},

    {q:"Abu Rayhon Beruniy qaysi davlatda yashab ijod etgan?",
     o:["Somoniylar va G'aznaviylar davrida","Faqat Somoniylar davrida","Temuriylar davrida","Mo'g'ullar davrida"],c:0},

    {q:"Beruniy qachon tug'ilgan?",
     o:["940 yilda","973 yilda","1048 yilda","1200 yilda"],c:1},

    {q:"Qoraxoniylar davlati qaysi asrlarda mavjud edi?",
     o:["VII–IX asrlar","X–XII asrlar","XIII–XIV asrlar","V–VI asrlar"],c:1},

    {q:"G'aznaviylar sulolasining eng mashhur hukmdori kim edi?",
     o:["Alptegin","Mahmud G'aznaviy","Mas'ud I","Ibrohim G'aznaviy"],c:1},

    {q:"Saljuqiylar sulolasi qachon Markaziy Osiyoni egallab oldi?",
     o:["X asr oxiri","XI asr o'rtalari","XII asr","IX asr"],c:1},

    {q:"Saljuqiylar poytaxti dastlab qaysi shahar edi?",
     o:["Bag'dod","Nishoppur","Isfahan","Marv"],c:3},

    {q:"Chingizxon Movarounnahrni qachon bosib oldi?",
     o:["1206–1210 yillarda","1219–1221 yillarda","1230–1235 yillarda","1200–1205 yillarda"],c:1},

    {q:"Chingizxon bosqini davrida qaysi yirik shaharlar vayron qilindi?",
     o:["Faqat Samarqand","Samarqand, Buxoro, Urganch va boshqalar","Faqat Buxoro","Toshkent va Farg'ona"],c:1},

    {q:"Chingizxon vafotidan keyin Movarounnahr qaysi ulusga kirdi?",
     o:["Ulus Juchiy","Ulus Chig'atoy","Ulus O'gedey","Ulus Toluy"],c:1},

    {q:"Amir Temur qachon tug'ilgan?",
     o:["1320 yilda","1336 yilda","1350 yilda","1370 yilda"],c:1},

    {q:"Amir Temur qayerda tug'ilgan?",
     o:["Samarqandda","Kesh (hozirgi Shahrisabz) shahrida","Buxoroda","Toshkentda"],c:1},

    {q:"Amir Temur qachon vafot etgan?",
     o:["1400 yilda","1403 yilda","1405 yilda","1410 yilda"],c:2},

    {q:"Amir Temur Samarqandni qachon poytaxti sifatida qaror toptirdi?",
     o:["1366 yilda","1370 yilda","1380 yilda","1395 yilda"],c:1},

    {q:"Amir Temur qaysi mamlakatlarga yurishlar qildi?",
     o:["Faqat Eron","Eron, Iroq, Hindiston, Turkiya, Rossiya va boshqalar","Faqat Hindiston","Faqat Xitoy"],c:1},

    {q:"Amir Temur Hindistonga qachon yurish qildi?",
     o:["1390 yilda","1398–1399 yillarda","1402 yilda","1395 yilda"],c:1},

    {q:"Amir Temur Anqara yaqinida qaysi hukmdorni mag'lub etdi?",
     o:["Husayn Boyqaro","Boyazid I (Usmonli sulton)","Muhammad Sulton","Shahrux"],c:1},

    {q:"Amir Temur davrida qanday ulkan qurilishlar amalga oshirildi?",
     o:["Faqat qal'alar","Ko'plab masjid, madrasa va saroy qurildi, Samarqand obod etildi","Faqat yo'llar","Faqat suv inshootlari"],c:1},

    {q:"Gur Amir maqbarasi qayerda joylashgan?",
     o:["Buxoroda","Xivada","Samarqandda","Toshkentda"],c:2},

    {q:"Ulug'bek kim edi?",
     o:["Amir Temurning o'g'li","Amir Temurning nabirasi","Amir Temurning ukasi","Amir Temurning vaziri"],c:1},

    {q:"Ulug'bek qachon tug'ilgan va vafot etgan?",
     o:["1394–1449 yillarda","1370–1440 yillarda","1400–1465 yillarda","1380–1450 yillarda"],c:0},

    {q:"Ulug'bek qaysi fan sohasida mashhur edi?",
     o:["Tibbiyot","Astronomiya va matematika","She'riyat","Tarix"],c:1},

    {q:"Ulug'bek rasadxonasi qayerda qurilgan?",
     o:["Buxoroda","Samarqandda","Toshkentda","Shahrisabzda"],c:1},

    {q:"Ulug'bek rasadxonasi qachon qurilgan?",
     o:["1394 yilda","1420 yilda","1449 yilda","1400 yilda"],c:1},

    {q:"Alisher Navoiy qachon tug'ilgan?",
     o:["1420 yilda","1441 yilda","1450 yilda","1435 yilda"],c:1},

    {q:"Alisher Navoiy qachon vafot etgan?",
     o:["1490 yilda","1501 yilda","1510 yilda","1495 yilda"],c:1},

    {q:"Alisher Navoiy qaysi tilda asarlar yozgan?",
     o:["Fors tili","Faqat arab tili","O'zbek (chig'atoy) va fors tillarida","Faqat o'zbek tili"],c:2},

    {q:"Alisher Navoiyning mashhur asarlari to'plami nima deb ataladi?",
     o:["Zafarnoma","Xamsai Navoiy (besh doston)","Devoni Kabir","Kutadg'u Bilig"],c:1},

    {q:"Husayn Boyqaro kim edi?",
     o:["Amir Temurning o'g'li","Temuriy hukmdor, Navoiyning hamkorı va do'sti","Mo'g'ul xoni","Buxoro amiri"],c:1},

    {q:"Temuriylar davlati qachon tugadi?",
     o:["1449 yilda","1500 yilda — Shayboniylar bosqini bilan","1550 yilda","1600 yilda"],c:1},

    {q:"Muhammad Tarag'ay (Ulug'bek) qaysi tomonidan o'ldirilgan?",
     o:["Dushmanlar tomonidan jangda","O'z o'g'li Abdullatif tomonidan","Vabo kasalidan","Mo'g'ullar tomonidan"],c:1},

    // ══════════════════════════════════════════════════════
    //  3. YANGI DAVR
    // ══════════════════════════════════════════════════════
    {q:"Shayboniylar sulolasi Movarounnahrni qachon egallab oldi?",
     o:["1450 yilda","1500 yilda","1550 yilda","1600 yilda"],c:1},

    {q:"Shayboniylar qaysi xalqdan edi?",
     o:["Mo'g'ul","O'zbek (o'zbek-qo'ng'irot qabilasi)","Tojik","Eron"],c:1},

    {q:"Buxoro xonligi qachon tashkil topdi?",
     o:["XV asr oxiri","XVI asr boshida Shayboniylar tomonidan","XVII asrda","XIV asrda"],c:1},

    {q:"Xiva xonligi qachon tashkil topdi?",
     o:["1511 yilda","XVI asrda","1700 yilda","1800 yilda"],c:1},

    {q:"Qo'qon xonligi qachon tashkil topdi?",
     o:["XVI asr","XVII asr","XVIII asr boshida (1709 yil)","XIX asr"],c:2},

    {q:"Rossiya imperiyasi O'rta Osiyoni egallab olishni qachon boshladi?",
     o:["XVIII asr","XIX asr o'rtalarida (1860-yillar)","XX asrda","XVII asrda"],c:1},

    {q:"Toshkent Rossiya tomonidan qachon bosib olindi?",
     o:["1860 yilda","1865 yilda","1870 yilda","1875 yilda"],c:1},

    {q:"Rossiya tomonidan Toshkentni bosib olgan general kim edi?",
     o:["Kaufman","Cherniyayev","Skobelev","Romanovskiy"],c:1},

    {q:"Buxoro amirligi Rossiyaga qachon bo'ysundirildi?",
     o:["1866 yilda","1868 yilda","1873 yilda","1876 yilda"],c:1},

    {q:"Xiva xonligi Rossiyaga qachon bo'ysundirildi?",
     o:["1868 yilda","1873 yilda","1876 yilda","1880 yilda"],c:1},

    {q:"Qo'qon xonligi Rossiya tomonidan qachon tugatildi?",
     o:["1873 yilda","1876 yilda","1880 yilda","1868 yilda"],c:1},

    {q:"Jadidchilik harakati nima edi?",
     o:["Harbiy harakat","Ta'lim va ijtimoiy islohotlar uchun kurashgan milliy ziyolilar harakati","Diniy harakat","Savdo uyushmasi"],c:1},

    {q:"Jadidchilik harakati qachon paydo bo'ldi?",
     o:["XIX asr oxiri — XX asr boshi","XVII asrda","XV asrda","XX asr o'rtalarida"],c:0},

    {q:"Jadidchilik harakatining asosiy vakillari kimlar edi?",
     o:["Faqat harbiy zobitlar","Mahmudxo'ja Behbudiy, Abdulla Avloniy, Munavvar Qori va boshqa ziyolilar","Faqat ruhoniylar","Rus amaldorlari"],c:1},

    {q:"Toshkentda birinchi yangi usul maktabi qachon ochildi?",
     o:["1893 yilda","1901 yilda","1916 yilda","1905 yilda"],c:1},

    {q:"1916 yilgi Markaziy Osiyo qo'zg'olonining asosiy sababi nima edi?",
     o:["Iqtisodiy inqiroz","Rus imperiyasining musofir xalqlarni frontga haydashligi","Din uchun kurash","Suvdan foydalanish huquqi"],c:1},

    {q:"Rossiya Turkiston o'lkasini boshqarish uchun qanday idora tuzdi?",
     o:["Guberniya","Turkiston general-gubernatorligi (1867)","Viloyat kengashi","Harbiy komissariat"],c:1},

    {q:"Turkiston general-gubernatorligining birinchi boshlig'i kim edi?",
     o:["Skobelev","Cherniyayev","Konstantin Kaufman","Romanovskiy"],c:2},

    {q:"Paxta monokulturasi O'rta Osiyoda qachon jadal rivojlana boshladi?",
     o:["XVI asrdan","XVIII asrdan","Rossiya bosib olgandan keyin (XIX asr o'rtalaridan)","XX asrdan"],c:2},

    {q:"Behbudiy tomonidan qaysi mashhur drama asari yozilgan?",
     o:["O'tkan kunlar","Padarkush","Mehr va Nahr","Lola"],c:1},

    // ══════════════════════════════════════════════════════
    //  4. SOVET DAVRI
    // ══════════════════════════════════════════════════════
    {q:"O'zbekiston Sovet Sotsialistik Respublikasi qachon tashkil topdi?",
     o:["1917 yilda","1924 yil 27 oktyabrda","1920 yilda","1936 yilda"],c:1},

    {q:"O'zbekiston SSR tashkil etilishida qaysi hudud Tojikistonga berildi?",
     o:["Buxoro","Samarqand va Buxoroning bir qismi (keyinroq)","Farg'ona","Xorazm"],c:1},

    {q:"Sovet davridagi O'zbekistonning birinchi rahbari kim edi?",
     o:["Fayzulla Xo'jayev","Usman Yusupov","Nurulloh Muxtov","Sharaf Rashidov"],c:0},

    {q:"Fayzulla Xo'jayev qachon qatl etildi?",
     o:["1932 yilda","1938 yilda Stalin repressiyasi davrida","1945 yilda","1929 yilda"],c:1},

    {q:"Ikkinchi Jahon urushi davrida O'zbekistonga qanday vazifalar yuklandi?",
     o:["Faqat qo'shin berish","Paxta, meva-sabzavot yetkazish, sanoat korxonalarini qabul qilish, askarlar tayyorlash","Faqat oziq-ovqat yetkazish","Faqat qurol ishlab chiqarish"],c:1},

    {q:"Ikkinchi Jahon urushida O'zbekistondan necha kishi qatnashdi?",
     o:["100 000 dan ortiq","1 milliondan ortiq","500 000 ga yaqin","2 million"],c:1},

    {q:"Mashhur o'zbek generali Sobir Rahimov qayerda shahid bo'ldi?",
     o:["Leningrad yaqinida","Dantsig (Gdansk) janglari paytida, 1945 yil mart","Stalingrad jangi","Berlin yaqinida"],c:1},

    {q:"Ikkinchi Jahon urushida O'zbekistonga ko'chirilgan bolalar yetimxonalari qaerdan keltirildi?",
     o:["Ukrainadan","Leningrad, Ukraina va boshqa hududlardan","Belarusdan","Faqat Moskvadan"],c:1},

    {q:"Sovet davridagi O'zbekistonda paxta maydonlari haddan oshiq kengaytirilganda qanday muammo yuzaga keldi?",
     o:["Ishsizlik","Orol dengiziga keladigan suv keskin kamaydi va dengiz qurib qoldi","Aholining ko'chishi","Iqtisodiy inqiroz"],c:1},

    {q:"Orol dengizi qaysi ikkita daryodan suv oladi?",
     o:["Zarafshon va Chirchiq","Amudaryo va Sirdaryo","Zerafshon va Amudaryo","Chu va Talas"],c:1},

    {q:"Orol dengizi XX asrda necha foizga qisqardi?",
     o:["20%","40%","90% dan ortig'iga","60%"],c:2},

    {q:"Sharaf Rashidov O'zbekiston KP birinchi kotibi lavozimida qanchala ishladi?",
     o:["1950–1966","1959–1983","1970–1990","1965–1985"],c:1},

    {q:"'Paxta ishi' nima edi?",
     o:["Yangi paxta navlarini yaratish","Sovet davrida O'zbekiston rahbarlari paxta hosilini sun'iy oshirganligi haqidagi korrupsiya ishi","Paxta eksporti shartnomasi","Paxta zavodlari qurilishi"],c:1},

    {q:"O'zbekistonda birinchi temir yo'l qachon qurildi?",
     o:["1875 yilda","1888 yilda","1900 yilda","1920 yilda"],c:1},

    {q:"Sovet davrida O'zbekiston poytaxti qaysi shahar edi?",
     o:["Samarqand","Toshkent","Namangan","Andijon"],c:1},

    {q:"Toshkentda 1966 yilda yuz bergan tabiiy ofat qanday edi?",
     o:["Sel","Kuchli zilzila","Qurg'oqchilik","Portlash"],c:1},

    {q:"1966 yil Toshkent zilzilasidan keyin qayta qurish ishlarida qaysi mamlakatlar yordam berdi?",
     o:["Faqat Rossiya","Sovet Ittifoqi barcha respublikalari","G'arb mamlakatlari","Xitoy"],c:1},

    {q:"Sovet davrida O'zbekistonda nechtadan ortiq kolxoz va sovxoz faoliyat yuritdi?",
     o:["500 dan ortiq","1000 dan ortiq","2000 dan ortiq","Bir nechtasi"],c:2},

    {q:"O'zbekistonda kirill alifbosi qachon joriy etildi?",
     o:["1929 yilda (lotin almashtirilib)","1940 yilda","1945 yilda","1950 yilda"],c:1},

    {q:"Sovet davrida O'zbekiston aholisi qanday o'sdi?",
     o:["Kamaydi","2 barobarga o'sdi","Sekin o'sdi","4 barobarga o'sdi"],c:1},

    // ══════════════════════════════════════════════════════
    //  5. MUSTAQILLIK DAVRI
    // ══════════════════════════════════════════════════════
    {q:"O'zbekiston qachon mustaqillikka erishdi?",
     o:["1990 yil 31 avgust","1991 yil 31 avgust","1992 yil 8 dekabr","1989 yil 14 noyabr"],c:1},

    {q:"O'zbekiston Konstitutsiyasi qachon qabul qilingan?",
     o:["1991 yil 31 avgustda","1992 yil 8 dekabrda","1993 yil 1 yanvarda","1991 yil 1 sentyabrda"],c:1},

    {q:"O'zbekiston Respublikasining birinchi Prezidenti kim edi?",
     o:["Shavkat Mirziyoyev","Islom Karimov","Fayzulla Xo'jayev","Sharaf Rashidov"],c:1},

    {q:"Islom Karimov qachon vafot etdi?",
     o:["2010 yilda","2013 yilda","2016 yilda","2018 yilda"],c:2},

    {q:"Islom Karimov O'zbekiston Prezidenti sifatida necha yil xizmat qildi?",
     o:["15 yil","20 yil","25 yil (1991–2016)","30 yil"],c:2},

    {q:"Shavkat Mirziyoyev O'zbekiston Prezidenti qachon saylandi?",
     o:["2016 yil dekabr","2017 yil yanvar","2015 yil","2018 yil"],c:0},

    {q:"Shavkat Mirziyoyev prezidentligining asosiy yo'nalishi qanday?",
     o:["Harbiy kuchayish","Iqtisodiy islohotlar, ochiqlik siyosati va modernizatsiya","Sovet davrini tiklash","Faqat qishloq xo'jaligi"],c:1},

    {q:"O'zbekiston mustaqillik e'lon qilishidan oldin qanday referendum o'tkazildi?",
     o:["Mustaqillik referendumi 1991 yil 29 dekabrda o'tkazildi","Bunday referendum bo'lmadi","1992 yil","1990 yil"],c:0},

    {q:"O'zbekiston milliy valyutasi 'so'm' qachon muomalaga kiritildi?",
     o:["1991 yilda","1993 yilda","1994 yil 1 iyulda","1996 yilda"],c:2},

    {q:"O'zbekiston birinchi bor BMTga qachon qabul qilindi?",
     o:["1991 yilda","1992 yil 2 martda","1993 yilda","1994 yilda"],c:1},

    {q:"O'zbekistonning davlat madhiyasi qachon qabul qilingan?",
     o:["1991 yil 31 avgustda","1992 yilda","1993 yilda","1995 yilda"],c:1},

    {q:"O'zbekiston Respublikasining davlat gerbi nima tasvirlanadi?",
     o:["Sher va qilich","Qushqo'nmas o'simlik, quyosh, cho'qqi uchli yulduz, bug'doy boshoqlari va paxta","Ot va bayraq","Tog' va daryo"],c:1},

    {q:"O'zbekistonning davlat bayrog'idagi ranglar qanday?",
     o:["Qizil, sariq, yashil","Ko'k, oq va yashil (chiziqli), qizil chiziqlar bilan","Ko'k va oq","Yashil va sariq"],c:1},

    {q:"O'zbekiston bayrog'idagi yarim oy va 12 yulduz nimani anglatadi?",
     o:["12 viloyatni","Islomiy an'ana va 12 oyning ramzi","12 oyni","Qadimgi sulolalarni"],c:2},

    {q:"Samarqand qaysi UNESCO ro'yxatida?",
     o:["Tabiat merosi","Dunyo madaniy merosi (World Heritage)","Xavf ostidagi meros","Nomateriall meros"],c:1},

    {q:"Buxorodagi Kalon minorasi necha metr balandlikda?",
     o:["30 metr","47 metr","60 metr","25 metr"],c:1},

    {q:"Ichon-Qal'a qaysi shaharda joylashgan?",
     o:["Samarqandda","Buxoroda","Xivada","Toshkentda"],c:2},

    {q:"Registon maydoni necha madrasadan iborat?",
     o:["2 ta","3 ta (Ulug'bek, Sherdor, Tillakori)","4 ta","5 ta"],c:1},

    {q:"Toshkentning ko'chasi — Amir Temur shoh ko'chasi qachon yaratilgan?",
     o:["Sovet davrida","Mustaqillikdan keyin, 1990-yillar","XVIII asrda","1966 yil zilzilasidan keyin"],c:1},

    {q:"O'zbekiston hozir nechta viloyatdan iborat?",
     o:["10","12","14 (12 viloyat + Toshkent shahar + Qoraqalpog'iston Respublikasi)","16"],c:2},

    {q:"Qoraqalpog'iston O'zbekistondagi qanday ma'muriy birlik?",
     o:["Oddiy viloyat","Avtonom respublika","Maxsus zona","Shahar"],c:1},

    {q:"O'zbekistonda 'Yuksalish' milliy harakati qachon tashkil etildi?",
     o:["2019 yilda","2018 yilda","2020 yilda","2016 yilda"],c:0},

    {q:"O'zbekiston Respublikasining rasmiy tili qaysi?",
     o:["Rus tili","O'zbek tili","Tojik tili","Arab tili"],c:1},

    {q:"O'zbekistonda lotin alifbosiga qachon qaytildi?",
     o:["1991 yilda","1993 yilda","1995 yildan boshlab","2000 yilda"],c:2},

    {q:"Mustaqillik kuni O'zbekistonda qaysi sanada nishonlanadi?",
     o:["1 sentabr","31 avgust","8 dekabr","20 iyun"],c:0},

    {q:"Navro'z bayrami qaysi sanada nishonlanadi?",
     o:["1 yanvar","8 mart","21 mart","1 may"],c:2},

    {q:"O'zbekistonda 'Oʻzbekiston — kelajagi buyuk davlat' konsepsiyasi kim tomonidan ilgari surildi?",
     o:["Islom Karimov","Shavkat Mirziyoyev","Fayzulla Xo'jayev","Sharaf Rashidov"],c:0},

    {q:"Shavkat Mirziyoyev tomonidan qabul qilingan 2017–2021 yillar strategiyasi qanday nomlanadi?",
     o:["'O'zbekiston 2030'","'Harakatlar strategiyasi' (5 ta ustuvor yo'nalish)","'Yangi O'zbekiston'","'Kelajak sari'"],c:1},

    {q:"Toshkent shahri O'zbekiston poytaxti sifatida aholisi bo'yicha Markaziy Osiyoda necha o'rinda turadi?",
     o:["2-o'rinda","1-o'rinda (eng yirik shahar)","3-o'rinda","4-o'rinda"],c:1},

    {q:"O'zbekistonda birinchi metro (yer osti temir yo'li) qachon ochildi?",
     o:["1966 yilda","1977 yilda","1980 yilda","1991 yilda"],c:1},

    {q:"Navoi shahri O'zbekistoning qaysi viloyatida joylashgan?",
     o:["Samarqand viloyatida","Navoiy viloyatida","Buxoro viloyatida","Qashqadaryo viloyatida"],c:1},

    {q:"Termiz shahri qaysi daryo bo'yida joylashgan?",
     o:["Sirdaryo","Zarafshon","Amudaryo","Chirchiq"],c:2},

    {q:"O'zbekiston mustaqillikdan keyin qaysi iqtisodiy tizimga o'tdi?",
     o:["Sotsializm saqlab qolindi","Bozor iqtisodiyotiga bosqichma-bosqich o'tish","Kommunizm","To'liq davlat iqtisodiyoti"],c:1},

    {q:"O'zbekistonda 'Mahalla' instituti nima uchun ahamiyatlidir?",
     o:["Faqat sport tadbirlari uchun","Jamoaviy o'zini-o'zi boshqarish, ijtimoiy yordam va mahalliy boshqaruv uchun","Harbiy tashkilot","Diniy muassasa"],c:1},

    {q:"Samarqand shahri O'zbekistonning necha yillik tarixi bo'lgan eng qadimiy shaharlaridan biri?",
     o:["1000 yildan ortiq","2000 yildan ortiq","3000 yildan ortiq (2750+ yil)","500 yil"],c:2},

    {q:"Farg'ona vodiysi O'zbekistonning qaysi qismida joylashgan?",
     o:["G'arbiy qismida","Sharqiy qismida","Janubiy qismida","Shimoliy qismida"],c:1},

    {q:"O'zbekistondagi Chimyon kurort-tog' hududi qaysi tog' tizmasida joylashgan?",
     o:["Pomir","Tyanshan","Hisor","Zarafshon"],c:1},

    // ── Qo'shimcha qadim davr savollari ──────────────────
    {q:"Qadimgi Xorazm poytaxti qaysi shahar bo'lgan?",
     o:["Urganch","Ko'hna Urganch (Gurganj)","Buxoro","Samarqand"],c:1},

    {q:"Miloddan avvalgi II–I asrlarda Farg'ona vodiysi Xitoy manbalarida qanday nomlangan?",
     o:["Sogdiana","Davan","Baqtriya","Parfiya"],c:1},

    {q:"Ipak yo'li qancha yillik tarixga ega?",
     o:["500 yil","1000 yil","2000 yildan ortiq","3500 yil"],c:2},

    {q:"Miloddan avvalgi II asrda Xan sulolasining elchisi Kim bo'lib, u Markaziy Osiyoni kashf etdi?",
     o:["Zhang Qian","Li Bai","Xuanzang","Fa Xien"],c:0},

    {q:"Zardushtiylikning muqaddas olovini saqlash uchun qurilgan inshootlar qanday nomlanadi?",
     o:["Madrasa","Olov ibodatxonasi (atashkada)","Masjid","Minora"],c:1},

    {q:"'Avesto' kitobining taxminiy yozilish davri qanday?",
     o:["Milodiy VI asr","Miloddan avvalgi XV–VI asrlar","Milodiy I asr","Miloddan avvalgi I asr"],c:1},

    {q:"Qadimgi Termiz shahri qaysi davlatning muhim shahri edi?",
     o:["Sug'd","Kushon shohligi","Parfiya","Eftalitlar"],c:1},

    {q:"Miloddan avvalgi III asrda Markaziy Osiyoda Yunon-Baqtriya shohligi necha yil davom etdi?",
     o:["50 yil","130 yil","200 yil","300 yil"],c:1},

    {q:"Eftalitlar qaysi hududlarni egallab turdi?",
     o:["Faqat Eron","Markaziy Osiyo, Afg'oniston, Shimoli-G'arbiy Hindiston","Faqat Xitoy","Arabiston yarim oroli"],c:1},

    // ── Qo'shimcha o'rta asrlar savollari ────────────────
    {q:"Abu Ali ibn Sino qayerda tug'ilgan?",
     o:["Buxoroda","Afshona (hozirgi O'zbekiston)","Bag'dodda","Tehron yaqinida"],c:1},

    {q:"Ibn Sinoning eng mashhur tibbiy asari qanday nomlanadi?",
     o:["Kitob al-Qonun fit-Tib (Tib qonuni)","Kitob al-Musiqa","Kitob al-Hisob","Risola"],c:0},

    {q:"Muhammad al-Xorazmiy qaysi sohada ilmiy kashfiyotlar qildi?",
     o:["Tibbiyot","Matematika (algebra) va astronomiya","Fizika","Kimyo"],c:1},

    {q:"Al-Xorazmiy qachon yashagan?",
     o:["VIII asrda","IX asrda (780–850)","X asrda","XI asrda"],c:1},

    {q:"'Algoritm' so'zi qaysi olimning nomidan kelib chiqqan?",
     o:["Ibn Sino","Al-Beruniy","Al-Xorazmiy","Ulug'bek"],c:2},

    {q:"Ahmad al-Farg'oniy qaysi sohada mashhur edi?",
     o:["She'riyat","Astronomiya","Tibbiyot","Huquq"],c:1},

    {q:"Al-Farg'oniy qayerda tug'ilgan?",
     o:["Buxoroda","Farg'ona shahrida","Samarqandda","Toshkentda"],c:1},

    {q:"Somoniylar davrida rasmiy davlat tili qaysi edi?",
     o:["Arab tili","Fors (dariy) tili","O'zbek tili","Turk tili"],c:1},

    {q:"Qoraxoniylar ulkan qadamni qayerda qo'yib, islomni qabul qildi?",
     o:["X asrda, islomni qabul qilgan birinchi turk sulolalaridan biri bo'ldi","VII asrda","XII asrda","IX asrda"],c:0},

    {q:"Saljuqiylar sulolasining asoschisi kim edi?",
     o:["Alp Arslon","Tug'rul bek","Malikshoh","Sanjar"],c:1},

    {q:"Saljuqiy sulton Alp Arslon 1071 yilda qaysi mashhur jangda g'alaba qozondi?",
     o:["Qadisiya jangi","Manzikert jangi","Ain Jalut jangi","Bag'dod jangi"],c:1},

    {q:"Chingizxon qachon tug'ilgan?",
     o:["1155 yilda","1162 yilda","1170 yilda","1145 yilda"],c:1},

    {q:"Mo'g'ul bosqini Buxoroni qachon vayron qildi?",
     o:["1218 yilda","1220 yilda","1225 yilda","1215 yilda"],c:1},

    {q:"Mo'g'ul bosqini davrida Ko'hna Urganch qachon vayron bo'ldi?",
     o:["1220 yilda","1221 yilda","1225 yilda","1218 yilda"],c:1},

    {q:"Amir Temur qaysi yil Samarqandni imperiya poytaxtiga aylantirdi?",
     o:["1366","1370","1380","1395"],c:1},

    {q:"Amir Temur Hindistonga yurish qilib qaysi shaharga yetib bordi?",
     o:["Bombay","Dehli","Kalkutta","Chennai"],c:1},

    {q:"Amir Temur Xitoyga yurish paytida qayerda vafot etdi?",
     o:["Samarqandda","O'tror (Faro'b) shahrida","Toshkentda","Termizda"],c:1},

    {q:"Temuriylar sulolasining so'nggi atoqli vakili — Zahiriddin Muhammad Bobur qayerda davlat tuzdi?",
     o:["Fors imperiyasi","Buyuk Mo'g'ullar imperiyasi (Hindistonda)","Usmonli imperiyasi","Safaviy imperiyasi"],c:1},

    {q:"Bobur qachon tug'ilgan?",
     o:["1480 yilda","1483 yilda","1490 yilda","1475 yilda"],c:1},

    {q:"Alisher Navoiyning to'rt devondan iborat asarlari to'plami qanday nomlanadi?",
     o:["Xamsa","Xazoyin ul-maoniy","Mahbub ul-qulub","Munshaot"],c:1},

    {q:"Navoiy asarlarida ishlatgan til keyinchalik qanday til asosiga aylandi?",
     o:["Fors adabiy tili","Hozirgi o'zbek adabiy tili","Turk adabiy tili","Arab adabiy tili"],c:1},

    // ── Qo'shimcha yangi davr savollari ──────────────────
    {q:"Shayboniylar sulolasini qaysi hukmdor asos soldi?",
     o:["Ubaydullaxon","Muhammad Shayboniyxon","Abdullaxon II","Ko'chkunchixon"],c:1},

    {q:"Muhammad Shayboniyxon Navoiy va Boburning zamonidoshimi?",
     o:["Yo'q, u ancha oldin yashagan","Ha, ular bir davrda yashagan","U ancha keyinroq yashagan","Bu haqida ma'lumot yo'q"],c:1},

    {q:"Abdullaxon II (Buxoro xoni) qachon hukmdorlik qildi?",
     o:["1500–1530","1557–1598","1600–1640","1650–1680"],c:1},

    {q:"Buxoro xonligida qaysi sulola XVIII asrda hokimiyatni qo'lga kiritdi?",
     o:["Shayboniylar","Ashtarxoniylar","Mang'itlar","Temuriylar"],c:2},

    {q:"Xiva xonligida qaysi sulola XIX asrda hukm surdi?",
     o:["Shayboniylar","Qo'ng'irotlar","Mang'itlar","Minglar"],c:1},

    {q:"Qo'qon xonligida qaysi sulola hukmronlik qildi?",
     o:["Mang'itlar","Minglar","Qo'ng'irotlar","Shayboniylar"],c:1},

    {q:"Rossiya Qo'qon xonligini tugatib, o'rniga qanday ma'muriy birlik tuzdi?",
     o:["Buxoro viloyati","Farg'ona viloyati","Sirdaryo viloyati","Samarqand viloyati"],c:1},

    {q:"Jadidlar milliy matbuotda qaysi gazetani nashr etishdi?",
     o:["Izvestiya","Turkiston viloyatining gazeti (va Samarqand, Oyina jurnali)","Pravda","Tashkentskiy kurer"],c:1},

    {q:"Munavvar Qori Abdurashidxonov qaysi faoliyati bilan mashhur?",
     o:["Harbiy sarkarda","Jadid pedagog va jamoat arbobi","Buxoro amiri","Rus amaldori"],c:1},

    {q:"1918 yilda Toshkentda tashkil etilgan hukumat qanday nomlanadi?",
     o:["Turkiston Muxtoriyati","Turkiston ASSR","Turkiston Komissariati","Buxoro xonligi"],c:1},

    // ── Qo'shimcha sovet davri savollari ─────────────────
    {q:"O'zbekiston SSRning poytaxti 1930 yilgacha qaysi shahar edi?",
     o:["Toshkent","Samarqand","Buxoro","Namangan"],c:1},

    {q:"O'zbekiston SSRning poytaxti Samarqanddan Toshkentga qachon ko'chirildi?",
     o:["1924 yilda","1930 yilda","1936 yilda","1945 yilda"],c:1},

    {q:"Sovet davrida O'zbekistonda qaysi sanoat tarmoqlari rivojlandi?",
     o:["Faqat paxta qayta ishlash","Paxta, ipakchilik, mashinasozlik, kimyo sanoati","Faqat og'ir sanoat","Faqat oziq-ovqat sanoati"],c:1},

    {q:"Ikkinchi Jahon urushi yillarida Toshkentda qancha harbiy sanoat korxonasi qayta joylashtirildi?",
     o:["10 ta","100 dan ortiq","30 ta","500 ta"],c:1},

    {q:"O'zbekistonda birinchi oliy o'quv yurti — O'rta Osiyo Davlat universiteti qachon tashkil etildi?",
     o:["1918 yilda","1920 yilda","1924 yilda","1930 yilda"],c:1},

    {q:"Orol dengizining qurib borishi qachon sezilarli tezlashdi?",
     o:["1930-yillarda","1960-yillardan","1980-yillardan","2000-yillardan"],c:1},

    {q:"Sovet davrida O'zbekiston paxta yetishtirishda SSR ichida necha o'rinni egalladi?",
     o:["1-o'rin (60% dan ortiq sovet paxtasi)","3-o'rin","5-o'rin","2-o'rin"],c:0},

    {q:"O'zbekistonda 1937–1938 yillardagi Stalin repressiyasi davrida qancha odam qatag'on qilindi?",
     o:["Bir necha o'nta","Minglab odam qatag'on qilindi","100 ta","Repressiya bo'lmadi"],c:1},

    {q:"Ikkinchi Jahon urushida O'zbekistondan frontga ketgan askarlarning taxminiy foizi qanday?",
     o:["Aholining 5%","Har 4 o'zbek erkaklaridan biri frontga jo'natildi","Aholining 1%","Faqat ko'ngillilar bordi"],c:1},

    {q:"Sovet davridagi O'zbekistonda 'hashar' an'anasi nima edi?",
     o:["Bayram","Jamoaviy ixtiyoriy mehnat ko'rsatish an'anasi","Harbiy mashq","Diniy marosim"],c:1},

    // ── Qo'shimcha mustaqillik davri savollari ────────────
    {q:"O'zbekiston 1992 yildan beri qaysi xalqaro tashkilotlarga a'zo?",
     o:["Faqat BMT","BMT, MDH, AQSH va NATO","BMT, MDH, OIC, ShOS va boshqalar","Faqat MDH"],c:2},

    {q:"O'zbekiston Markaziy Osiyo yadrosiz zona shartnomasi — SEMEY shartnomasini qachon imzoladi?",
     o:["2006 yilda","2009 yilda","1997 yilda","2015 yilda"],c:0},

    {q:"Toshkent shahrida o'tkazilgan 1966 yilgi qurishdan keyingi rekonstruksiya qaysi arxitektura uslubida amalga oshirildi?",
     o:["Klassik o'zbek uslubida","Sovet modernizmi uslubida","Gotik uslubida","Zamonaviy evropa uslubida"],c:1},

    {q:"Islom Karimov 'O'zbekiston — kelajagi buyuk davlat' kitobini qachon yozdi?",
     o:["1991 yilda","1992 yilda","1997 yilda","2000 yilda"],c:1},

    {q:"Shavkat Mirziyoyev 'Yangi O'zbekiston' konsepsiyasini qachon rasmiy e'lon qildi?",
     o:["2017 yilda","2022 yilda","2020 yilda","2016 yilda"],c:1},

    {q:"O'zbekistonda qaysi yildan boshlab viza rejimi sezilarli darajada yumshatildi?",
     o:["2016 yildan","2018 yildan","2019 yildan","2015 yildan"],c:1},

    {q:"Samarqand 2022 yilda qaysi xalqaro tashkilot sammitiga mezbonlik qildi?",
     o:["G7 sammiti","ShOS (SCO) sammiti","NATO sammiti","BMT Bosh Assambleyasi"],c:1},

    {q:"O'zbekiston 2023 yilda qaysi xalqaro tashkilotga kuzatuvchi sifatida qabul qilindi?",
     o:["NATO","Yevropa Ittifoqi","MDH","BRICS"],c:3},

    {q:"O'zbekistondagi mashhur 'Oltin Vodiy' iborasi qaysi hududga nisbatan ishlatiladi?",
     o:["Zarafshon vodiysi","Farg'ona vodiysi","Surxondaryo vodiysi","Chirchiq vodiysi"],c:1},

    {q:"O'zbekistonda nechta UNESCO Jahon merosi ob'ekti mavjud (2024 yil holatiga ko'ra)?",
     o:["2 ta","4 ta","6 ta","10 ta"],c:1},

    {q:"O'zbekistonda qaysi yildan boshlab xorijiy valyutani erkin almashtirish mumkin bo'ldi?",
     o:["2016 yil","2017 yil sentabrdan","2019 yildan","2020 yildan"],c:1},

    {q:"O'zbekiston Respublikasi Prezidenti nechi yil muddatga saylanadi?",
     o:["5 yil","7 yil","4 yil","6 yil"],c:1},

    {q:"O'zbekiston Oliy Majlisi qanday tuzilgan?",
     o:["Bir palatali","Ikki palatali (Senat va Qonunchilik palatasi)","Uch palatali","To'rt palatali"],c:1},

    {q:"O'zbekistonda birinchi xususiy bank qachon tashkil etildi?",
     o:["1990 yilda","1992 yilda","1994 yilda","2000 yilda"],c:1},

    {q:"Navoiy viloyati qachon tashkil etildi?",
     o:["1974 yilda","1982 yilda","1991 yilda","1960 yilda"],c:1},

    {q:"O'zbekistondagi Fergana-Toshkent avtomobil yo'li qanday tog' dovonlari orqali o'tadi?",
     o:["Kamchik dovoni","Toshkent dovoni","Chorvoq dovoni","Angren dovoni"],c:0},

    {q:"O'zbekistondagi Kamchik tunneli qachon ochildi?",
     o:["2016 yil","2017 yil mart","2018 yil","2015 yil"],c:1},

    {q:"Andijon shahri qaysi voqeasi bilan 2005 yilda jahon diqqatini tortdi?",
     o:["Zilzila","Andijon voqealari (13 may 2005)","Sel","Tabiiy ofat"],c:1},

    {q:"O'zbekistonda Navro'z bayramini rasman qaysi yildan nishonlash tiklandi?",
     o:["1989 yil","1991 yil","1993 yil","1988 yil"],c:0},

    {q:"O'zbekiston 'Yuksalish' milliy harakatining vazifasi nima?",
     o:["Harbiy kuch to'plash","Fuqarolik jamiyatini rivojlantirish va siyosiy ishtirokni kengaytirish","Chet el investitsiyalarini jalb qilish","Dinga e'tibor qaratish"],c:1},

    {q:"O'zbekistondagi Chirchiq daryosi qaysi tog' tizmasidan boshlanadi?",
     o:["Pomir","Tyanshan (Chatqol va Ugom tizmalari)","Hisor","Zarafshon"],c:1},

    {q:"O'zbekistondagi Tojiobod — qaysi davlat bilan chegaradosh hududdagi o'tish joyi?",
     o:["Afg'oniston","Tojikiston","Qirg'iziston","Qozog'iston"],c:2},

    {q:"O'zbekiston mustaqilligining 30 yillik tantanasi qaysi yilda bo'ldi?",
     o:["2019 yilda","2020 yilda","2021 yilda","2022 yilda"],c:2},

    {q:"O'zbekistondagi Muborak gaz konlari qaysi viloyatda joylashgan?",
     o:["Buxoro viloyatida","Qashqadaryo viloyatida","Navoiy viloyatida","Surxondaryo viloyatida"],c:1},

    {q:"O'zbekistondagi Olmaliq shahrida qaysi sanoat sohasi rivojlangan?",
     o:["To'qimachilik","Kon metallurgiyasi (mis, oltin, molibden)","Neft kimyosi","Elektron sanoat"],c:1}
  ]
};


// ============================================================
//  ENGLISH DATA — Inglizcha so'z juftliklari (500+)
//  Format: [o'zbekcha, inglizcha]
// ============================================================
window.ENGLISH_DATA = {
  categories: {

    // ── hayvonlar (40+ pairs) ─────────────────────────────
    hayvonlar: [
      ["mushuk","cat"],
      ["it","dog"],
      ["ot","horse"],
      ["sigir","cow"],
      ["qo'y","sheep"],
      ["echki","goat"],
      ["cho'chqa","pig"],
      ["tovuq","chicken"],
      ["o'rdak","duck"],
      ["g'oz","goose"],
      ["quyon","rabbit"],
      ["sichqon","mouse"],
      ["kalamush","rat"],
      ["sher","lion"],
      ["yo'lbars","tiger"],
      ["ayiq","bear"],
      ["bo'ri","wolf"],
      ["tulki","fox"],
      ["fil","elephant"],
      ["maymun","monkey"],
      ["zebra","zebra"],
      ["jirafa","giraffe"],
      ["kiyik","deer"],
      ["qo'zichoq","lamb"],
      ["tuya","camel"],
      ["ilon","snake"],
      ["toshbaqa","turtle"],
      ["baqa","frog"],
      ["baliq","fish"],
      ["qush","bird"],
      ["burgut","eagle"],
      ["kabutar","pigeon"],
      ["qarg'a","crow"],
      ["to'tiqush","parrot"],
      ["krokodil","crocodile"],
      ["pingvin","penguin"],
      ["delfin","dolphin"],
      ["kit","whale"],
      ["oktyopus","octopus"],
      ["hasharot","insect"],
      ["chumoli","ant"],
      ["kapalak","butterfly"],
      ["asalari","bee"],
      ["o'rgimchak","spider"]
    ],

    // ── mevalar (30+ pairs) ───────────────────────────────
    mevalar: [
      ["olma","apple"],
      ["nok","pear"],
      ["banan","banana"],
      ["apelsin","orange"],
      ["limon","lemon"],
      ["mandarin","tangerine"],
      ["greypfrut","grapefruit"],
      ["uzum","grape"],
      ["qulupnay","strawberry"],
      ["gilos","cherry"],
      ["olcha","sour cherry"],
      ["shaftoli","peach"],
      ["o'rik","apricot"],
      ["anor","pomegranate"],
      ["tarvuz","watermelon"],
      ["qovun","melon"],
      ["anjir","fig"],
      ["xurmo","persimmon"],
      ["kivi","kiwi"],
      ["mango","mango"],
      ["ananas","pineapple"],
      ["kokos","coconut"],
      ["papaya","papaya"],
      ["malin","raspberry"],
      ["smorodina","currant"],
      ["krujovnik","gooseberry"],
      ["mushmula","loquat"],
      ["nektarin","nectarine"],
      ["laym","lime"],
      ["behi","quince"],
      ["xurmo","date"]
    ],

    // ── sabzavotlar (25+ pairs) ───────────────────────────
    sabzavotlar: [
      ["sabzi","carrot"],
      ["kartoshka","potato"],
      ["piyoz","onion"],
      ["sarimsoq","garlic"],
      ["pomidor","tomato"],
      ["bodring","cucumber"],
      ["karam","cabbage"],
      ["qalampir","pepper"],
      ["baqlajon","eggplant"],
      ["qovoq","pumpkin"],
      ["zucchini","zucchini"],
      ["shalgam","turnip"],
      ["lavlagi","beetroot"],
      ["lobiya","bean"],
      ["no'xat","pea"],
      ["makkajo'xori","corn"],
      ["brokkoli","broccoli"],
      ["qorovul (karnabahar)","cauliflower"],
      ["shpinat","spinach"],
      ["salat","lettuce"],
      ["ukrop","dill"],
      ["ko'k piyoz","green onion"],
      ["reddish","radish"],
      ["selderey","celery"],
      ["asparagus","asparagus"],
      ["artishok","artichoke"]
    ],

    // ── ranglar (20+ pairs) ───────────────────────────────
    ranglar: [
      ["qizil","red"],
      ["ko'k","blue"],
      ["yashil","green"],
      ["sariq","yellow"],
      ["to'q sariq","orange"],
      ["binafsha","purple"],
      ["pushti","pink"],
      ["jigarrang","brown"],
      ["qora","black"],
      ["oq","white"],
      ["kulrang","grey"],
      ["kumush","silver"],
      ["oltin","gold"],
      ["moviy","light blue"],
      ["to'q ko'k","dark blue"],
      ["qo'ng'ir","beige"],
      ["limon sariq","lemon"],
      ["liloviy","lilac"],
      ["terang yashil","lime"],
      ["marjon rangi","coral"],
      ["turkuaz","turquoise"],
      ["qirmizi","crimson"]
    ],

    // ── raqamlar (20+ pairs) ──────────────────────────────
    raqamlar: [
      ["nol","zero"],
      ["bir","one"],
      ["ikki","two"],
      ["uch","three"],
      ["to'rt","four"],
      ["besh","five"],
      ["olti","six"],
      ["yetti","seven"],
      ["sakkiz","eight"],
      ["to'qqiz","nine"],
      ["o'n","ten"],
      ["o'n bir","eleven"],
      ["o'n ikki","twelve"],
      ["yigirma","twenty"],
      ["o'ttiz","thirty"],
      ["qirq","forty"],
      ["ellik","fifty"],
      ["yuz","hundred"],
      ["ming","thousand"],
      ["birinchi","first"],
      ["ikkinchi","second"],
      ["uchinchi","third"],
      ["oxirgi","last"]
    ],

    // ── oila (20+ pairs) ──────────────────────────────────
    oila: [
      ["ona","mother"],
      ["ota","father"],
      ["aka","older brother"],
      ["uka","younger brother"],
      ["opa","older sister"],
      ["singil","younger sister"],
      ["buva","grandfather"],
      ["buvi","grandmother"],
      ["amaki","uncle"],
      ["xola","aunt"],
      ["jiyan","nephew/niece"],
      ["nevara","grandchild"],
      ["er","husband"],
      ["xotin","wife"],
      ["o'g'il","son"],
      ["qiz","daughter"],
      ["qarindosh","relative"],
      ["oila","family"],
      ["bola","child"],
      ["kuyov","son-in-law"],
      ["kelin","daughter-in-law"],
      ["qaynona","mother-in-law"],
      ["qaynota","father-in-law"]
    ],

    // ── kiyim (25+ pairs) ─────────────────────────────────
    kiyim: [
      ["ko'ylak","shirt"],
      ["futbolka","t-shirt"],
      ["shim","trousers"],
      ["jins","jeans"],
      ["ko'ynak (ayollar)","dress"],
      ["yubka","skirt"],
      ["kurtka","jacket"],
      ["palto","coat"],
      ["to'n","robe"],
      ["sviter","sweater"],
      ["pulover","pullover"],
      ["kiyim-kechak","clothes"],
      ["poyabzal","shoes"],
      ["etik","boots"],
      ["krossovka","sneakers"],
      ["sandal","sandals"],
      ["shlyapa","hat"],
      ["do'ppi","skullcap"],
      ["sharf","scarf"],
      ["qo'lqop","gloves"],
      ["kamar","belt"],
      ["galstuk","tie"],
      ["ichki kiyim","underwear"],
      ["pijama","pajamas"],
      ["xalat","robe/gown"],
      ["shipper","slipper"]
    ],

    // ── uy (25+ pairs) ────────────────────────────────────
    uy: [
      ["uy","house"],
      ["kvartira","apartment"],
      ["xona","room"],
      ["oshxona","kitchen"],
      ["yotoqxona","bedroom"],
      ["mehmonxona","living room"],
      ["hammom","bathroom"],
      ["hojatxona","toilet"],
      ["balkon","balcony"],
      ["deraza","window"],
      ["eshik","door"],
      ["pol","floor"],
      ["shift","ceiling"],
      ["devor","wall"],
      ["zina","stairs"],
      ["krovot","bed"],
      ["stol","table"],
      ["stul","chair"],
      ["divan","sofa"],
      ["shkaf","wardrobe"],
      ["javon","shelf"],
      ["televizor","television"],
      ["muzlatkich","refrigerator"],
      ["pech","stove"],
      ["kir yuvish mashinasi","washing machine"],
      ["gilam","carpet"]
    ],

    // ── taom (30+ pairs) ──────────────────────────────────
    taom: [
      ["non","bread"],
      ["guruch","rice"],
      ["go'sht","meat"],
      ["tovuq go'shti","chicken"],
      ["baliq","fish"],
      ["tuxum","egg"],
      ["sut","milk"],
      ["qatiq","yogurt"],
      ["pishloq","cheese"],
      ["yog'","oil/butter"],
      ["qand","sugar"],
      ["tuz","salt"],
      ["qalampir (ziravor)","pepper (spice)"],
      ["non tekis","flatbread"],
      ["osh (palov)","pilaf"],
      ["sho'rva","soup"],
      ["lag'mon","noodle soup"],
      ["manti","dumplings"],
      ["samsa","samsa"],
      ["shashlik","shashlik"],
      ["salat","salad"],
      ["meva sharbati","fruit juice"],
      ["choy","tea"],
      ["qahva","coffee"],
      ["suv","water"],
      ["muzqaymoq","ice cream"],
      ["tort","cake"],
      ["pechene","cookies"],
      ["shokolad","chocolate"],
      ["qovurma","fried dish"],
      ["qaymaq","cream"],
      ["asal","honey"]
    ],

    // ── tabiat (25+ pairs) ────────────────────────────────
    tabiat: [
      ["quyosh","sun"],
      ["oy","moon"],
      ["yulduz","star"],
      ["osmon","sky"],
      ["bulut","cloud"],
      ["yomg'ir","rain"],
      ["qor","snow"],
      ["shamol","wind"],
      ["momaqaldiroq","thunder"],
      ["chaqmoq","lightning"],
      ["kamalak","rainbow"],
      ["tuman","fog"],
      ["muz","ice"],
      ["daryo","river"],
      ["ko'l","lake"],
      ["dengiz","sea"],
      ["okean","ocean"],
      ["tog'","mountain"],
      ["tepalik","hill"],
      ["tekislik","plain"],
      ["cho'l","desert"],
      ["o'rmon","forest"],
      ["dala","field"],
      ["gul","flower"],
      ["daraxt","tree"],
      ["o't","grass"]
    ],

    // ── transport (20+ pairs) ─────────────────────────────
    transport: [
      ["mashina","car"],
      ["avtobus","bus"],
      ["poyezd","train"],
      ["samolyot","airplane"],
      ["velosiped","bicycle"],
      ["mototsikl","motorcycle"],
      ["kema","ship"],
      ["metro","subway"],
      ["taksi","taxi"],
      ["marshrutka","minibus"],
      ["vertolyot","helicopter"],
      ["qayiq","boat"],
      ["yuk mashinasi","truck"],
      ["ambulans","ambulance"],
      ["o't o'chiruvchi mashina","fire truck"],
      ["tramvay","tram"],
      ["avtobus to'xtashi","bus stop"],
      ["aeroport","airport"],
      ["temir yo'l stansiyasi","railway station"],
      ["benzin","gasoline"],
      ["haydovchi","driver"],
      ["yo'lovchi","passenger"]
    ],

    // ── kasb (30+ pairs) ──────────────────────────────────
    kasb: [
      ["o'qituvchi","teacher"],
      ["shifokor","doctor"],
      ["hamshira","nurse"],
      ["muhandis","engineer"],
      ["arxitektor","architect"],
      ["huquqshunos","lawyer"],
      ["hakim","judge"],
      ["politsiyachi","police officer"],
      ["harbiy","soldier"],
      ["pilot","pilot"],
      ["oshpaz","cook/chef"],
      ["novvoy","baker"],
      ["sotuvchi","seller"],
      ["fermer","farmer"],
      ["bog'bon","gardener"],
      ["qurilishchi","builder"],
      ["elektrik","electrician"],
      ["suratchi","photographer"],
      ["jurnalist","journalist"],
      ["yozuvchi","writer"],
      ["rassom","painter/artist"],
      ["musiqachi","musician"],
      ["aktyor","actor"],
      ["sport o'yinchisi","athlete"],
      ["dasturchi","programmer"],
      ["buxgalter","accountant"],
      ["tadbirkor","entrepreneur"],
      ["direktor","director"],
      ["kassir","cashier"],
      ["stomatolog","dentist"],
      ["veterinar","veterinarian"]
    ],

    // ── maktab (20+ pairs) ────────────────────────────────
    maktab: [
      ["maktab","school"],
      ["universitet","university"],
      ["sinf","classroom"],
      ["doska","blackboard"],
      ["qalam","pencil"],
      ["ruchka","pen"],
      ["daftar","notebook"],
      ["kitob","book"],
      ["portfel","schoolbag"],
      ["o'quvchi","student (school)"],
      ["talaba","student (university)"],
      ["ma'ruza","lecture"],
      ["imtihon","exam"],
      ["test","test"],
      ["matematika","mathematics"],
      ["tarix","history"],
      ["geografiya","geography"],
      ["kimyo","chemistry"],
      ["fizika","physics"],
      ["biologiya","biology"],
      ["adabiyot","literature"]
    ],

    // ── sport (20+ pairs) ─────────────────────────────────
    sport: [
      ["futbol","football/soccer"],
      ["basketbol","basketball"],
      ["volebol","volleyball"],
      ["tennis","tennis"],
      ["suzish","swimming"],
      ["yugurish","running"],
      ["boks","boxing"],
      ["kurash","wrestling"],
      ["gimnastika","gymnastics"],
      ["velosipedchilik","cycling"],
      ["tog'dan tushish (ski)","skiing"],
      ["qayiqda suzish","rowing"],
      ["ko'tarish (shtanga)","weightlifting"],
      ["karate","karate"],
      ["sport o'yini","sports game"],
      ["stadion","stadium"],
      ["sport zali","gym"],
      ["musobaqa","competition"],
      ["g'olib","winner"],
      ["jamoa","team"],
      ["golf","golf"],
      ["baseball","baseball"]
    ],

    // ── his (20+ pairs) ───────────────────────────────────
    his: [
      ["xursand","happy"],
      ["g'amgin","sad"],
      ["g'azablangan","angry"],
      ["qo'rqquv","afraid"],
      ["hayratda","surprised"],
      ["charchagan","tired"],
      ["dam olgan","rested"],
      ["och","hungry"],
      ["to'q","full"],
      ["sovuq","cold"],
      ["issiq","hot"],
      ["yaxshi","good"],
      ["yomon","bad"],
      ["katta","big"],
      ["kichik","small"],
      ["tez","fast"],
      ["sekin","slow"],
      ["baland","tall/high"],
      ["past","short/low"],
      ["yangi","new"],
      ["eski","old"],
      ["chiroyli","beautiful"],
      ["xunuk","ugly"]
    ],

    // ── kunlar (7 pairs) ──────────────────────────────────
    kunlar: [
      ["dushanba","Monday"],
      ["seshanba","Tuesday"],
      ["chorshanba","Wednesday"],
      ["payshanba","Thursday"],
      ["juma","Friday"],
      ["shanba","Saturday"],
      ["yakshanba","Sunday"]
    ],

    // ── oylar (12 pairs) ──────────────────────────────────
    oylar: [
      ["yanvar","January"],
      ["fevral","February"],
      ["mart","March"],
      ["aprel","April"],
      ["may","May"],
      ["iyun","June"],
      ["iyul","July"],
      ["avgust","August"],
      ["sentabr","September"],
      ["oktabr","October"],
      ["noyabr","November"],
      ["dekabr","December"]
    ],

    // ── salomlashish (20+ phrases) ────────────────────────
    salomlashish: [
      ["salom","hello"],
      ["xayr","goodbye"],
      ["hayrli tong","good morning"],
      ["hayrli kun","good afternoon"],
      ["hayrli kech","good evening"],
      ["kechasi xayr","good night"],
      ["rahmat","thank you"],
      ["marhamat","you're welcome"],
      ["iltimos","please"],
      ["kechirasiz","excuse me/sorry"],
      ["ha","yes"],
      ["yo'q","no"],
      ["bilmayman","I don't know"],
      ["tushundim","I understand"],
      ["qaytadan ayting","please repeat"],
      ["ismingiz nima?","what is your name?"],
      ["mening ismim ...","my name is ..."],
      ["qandaysiz?","how are you?"],
      ["yaxshiman","I'm fine"],
      ["ko'rishguncha","see you later"],
      ["xush kelibsiz","welcome"],
      ["baxtli tug'ilgan kun","happy birthday"]
    ],

    // ── fe'llar (40+ pairs, infinitive) ──────────────────
    "fe'llar": [
      ["bo'lmoq","to be"],
      ["bormoq","to go"],
      ["kelmoq","to come"],
      ["ko'rmoq","to see"],
      ["eshitmoq","to hear"],
      ["gapirmoq","to speak"],
      ["o'qimoq","to read"],
      ["yozmoq","to write"],
      ["yemoq","to eat"],
      ["ichmoq","to drink"],
      ["uxlamoq","to sleep"],
      ["turmoq","to stand/get up"],
      ["o'tirmoq","to sit"],
      ["yugurmoq","to run"],
      ["yurmoq","to walk"],
      ["qo'ymoq","to put"],
      ["olmoq","to take"],
      ["bermoq","to give"],
      ["so'ramoq","to ask"],
      ["javob bermoq","to answer"],
      ["sevmoq","to love"],
      ["yoqtirmoq","to like"],
      ["ishlashmoq","to work"],
      ["o'rganmoq","to learn"],
      ["o'ynamoq","to play"],
      ["kulmoq","to laugh"],
      ["yig'lamoq","to cry"],
      ["ochmoq","to open"],
      ["yopmoq","to close"],
      ["sotib olmoq","to buy"],
      ["sotmoq","to sell"],
      ["qilmoq","to do/make"],
      ["bilmoq","to know"],
      ["o'ylamoq","to think"],
      ["istamoq","to want"],
      ["kerak bo'lmoq","to need"],
      ["qaytmoq","to return"],
      ["boshlashmoq","to start"],
      ["tugatmoq","to finish"],
      ["topmoq","to find"],
      ["yo'qotmoq","to lose"],
      ["ko'tarishmoq","to lift"],
      ["sakramoq","to jump"],
      ["uchmoq","to fly"],
      ["suzmoq","to swim"],
      ["yashash","to live"],
      ["o'lmoq","to die"],
      ["tug'ilmoq","to be born"],
      ["o'smoq","to grow"],
      ["o'zgartirmoq","to change"],
      ["qarar qilmoq","to decide"],
      ["eslamoq","to remember"],
      ["unutmoq","to forget"],
      ["to'xtamoq","to stop"],
      ["boshlash","to begin"],
      ["davom ettirmoq","to continue"],
      ["kutmoq","to wait"],
      ["yetmoq","to reach"],
      ["taklif qilmoq","to invite"],
      ["qabul qilmoq","to accept"],
      ["rad etmoq","to refuse"],
      ["to'lamoq","to pay"],
      ["hisoblashmoq","to count"],
      ["o'lchashmoq","to measure"]
    ],

    // ── vaqt (time) ───────────────────────────────────────
    vaqt: [
      ["bugun","today"],
      ["ertaga","tomorrow"],
      ["kecha","yesterday"],
      ["hozir","now"],
      ["keyin","later"],
      ["oldin","before"],
      ["kechqurun","evening"],
      ["tush","noon"],
      ["tong","morning"],
      ["tun","night"],
      ["soat","hour"],
      ["daqiqa","minute"],
      ["soniya","second"],
      ["kun","day"],
      ["hafta","week"],
      ["oy","month"],
      ["yil","year"],
      ["bahor","spring"],
      ["yoz","summer"],
      ["kuz","autumn"],
      ["qish","winter"],
      ["doim","always"],
      ["hech qachon","never"],
      ["ba'zan","sometimes"]
    ],

    // ── sifatlar (adjectives) ─────────────────────────────
    sifatlar: [
      ["baxtli","happy"],
      ["baxtsiz","unhappy"],
      ["zo'r","great"],
      ["dahshatli","terrible"],
      ["to'g'ri","correct"],
      ["noto'g'ri","incorrect"],
      ["oson","easy"],
      ["qiyin","difficult"],
      ["og'ir","heavy"],
      ["yengil","light"],
      ["toza","clean"],
      ["iflos","dirty"],
      ["to'liq","full/complete"],
      ["bo'sh","empty"],
      ["aqlli","clever/smart"],
      ["ahmoq","silly/foolish"],
      ["mard","brave"],
      ["qo'rqoq","cowardly"],
      ["saxiy","generous"],
      ["xasis","stingy"],
      ["mehnatkash","hardworking"],
      ["dangasa","lazy"],
      ["kuchli","strong"],
      ["zaif","weak"]
    ],

    // ── joy va yo'nalish (place & direction) ─────────────
    joy: [
      ["qayerda","where"],
      ["bu yerda","here"],
      ["u yerda","there"],
      ["yuqorida","above/up"],
      ["pastda","below/down"],
      ["o'ngda","on the right"],
      ["chapda","on the left"],
      ["oldinda","in front"],
      ["orqada","behind"],
      ["ichida","inside"],
      ["tashqarida","outside"],
      ["yaqin","near"],
      ["uzoq","far"],
      ["shimol","north"],
      ["janub","south"],
      ["sharq","east"],
      ["g'arb","west"],
      ["markaz","center"],
      ["bozor","market"],
      ["do'kon","shop/store"],
      ["kasalxona","hospital"],
      ["poliklinika","clinic"],
      ["bank","bank"],
      ["mehmonxona","hotel"],
      ["restoran","restaurant"],
      ["kutubxona","library"],
      ["muzey","museum"],
      ["park","park"],
      ["bog'","garden"]
    ]

  }
};
