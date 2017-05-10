/// <summary>
/// Simple Class for Clash of Clans API consumption.
/// </summary>
/// <remarks>
/// Line 60 must be changed to include your clans tag.
/// </remarks>
namespace ClashApp
{
    public class ClanViewModel
    {
        public string Tag { get; set; }

        [Display(Name = "Clan Name")]
        public string ClanName { get; set; }

        public string Description { get; set; }

        public Type Type { get; set; }

        [Display(Name = "Clan Level")]
        public string ClanLevel { get; set; }

        [Display(Name = "Clan Points")]
        public string ClanPoints { get; set; }

        [Display(Name = "Required Trophies")]
        public string RequiredTrophies { get; set; }

        [Display(Name = "War Frequency")]
        public string WarFrequency { get; set; }

        [Display(Name = "Win Streak")]
        public string WarWinStreak { get; set; }

        [Display(Name = "Wins")]
        public string WarWins { get; set; }

        [Display(Name = "Ties")]
        public string WarTies { get; set; }
        [Display(Name = "Losses")]
        public string WarLosses { get; set; }

        public string Members { get; set; }

        public string Location { get; set; }

    }

    public enum Type
    {
        Closed,
        Open,
        InviteOnly
    }

    public static class ClashURIRoute
    {
        public static string Token = "foobar";
        public static string Scheme = "Bearer";
        public static string ClanUri = "https://api.clashofclans.com/v1/clans/%23YOURCLANTAG";
    }

    public static class HomeControllerRoute
    {
        public const string GetIndex = ControllerName.Home + "GetIndex";
        public const string GetMembers = ControllerName.Home + "GetMembers";
        public const string GetWarLog = ControllerName.Home + "GetWarLog";
    }

    public static class HomeControllerAction
    {
        public const string Index = "Index";
        public const string Members = "Members";
        public const string WarLog = "WarLog";
    }

    public class HomeController : Controller
    {
        private readonly IClashAPIService clashAPIService;

        public HomeController(IClashAPIService clashAPIService)
        {
            this.clashAPIService = clashAPIService;
        }

        [HttpGet("", Name = HomeControllerRoute.GetIndex)]
        public async Task<IActionResult> Index()
        {
            string response = await this.clashAPIService.GetAPIResponse(ClashURIRoute.ClanUri);
            ClanViewModel Clan = new ClanViewModel();
            Clan = this.clashAPIService.GenerateClan(Clan, response);

            return this.View(HomeControllerAction.Members, Clan);
        }
    }

    public interface IClashAPIService
    {
        Task<string> GetAPIResponse(string _uri);
        WarLogViewModel GenerateWarLog(WarLogViewModel warLog, string response);
        MembersViewModel GenerateMembers(MembersViewModel members, string response);
        ClanViewModel GenerateClan(ClanViewModel clan, string response);
    }

    public sealed class ClashAPIService : IClashAPIService
    {
        public ClashAPIService()
        {

        }
        public async Task<string> GetAPIResponse(string _uri)
        {
            using (var client = new HttpClient())
            {
                AuthenticationHeaderValue auth = new AuthenticationHeaderValue(ClashURIRoute.Scheme, ClashURIRoute.Token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = auth;

                return await client.GetStringAsync(_uri);
            }
        }

        public ClanViewModel GenerateClan(ClanViewModel clan, string response)
        {
            JObject items = JObject.Parse(response);

            clan.Tag = items["tag"].ToString();
            clan.ClanName = items["name"].ToString();
            clan.Description = items["description"].ToString();
            clan.ClanLevel = items["clanLevel"].ToString();
            clan.ClanPoints = items["clanPoints"].ToString();
            clan.RequiredTrophies = items["requiredTrophies"].ToString();
            clan.WarFrequency = items["warFrequency"].ToString();
            clan.WarWinStreak = items["warWinStreak"].ToString();
            clan.WarWins = items["warWins"].ToString();
            clan.WarTies = items["warTies"].ToString();
            clan.WarLosses = items["warLosses"].ToString();
            clan.Members = items["members"].ToString();
            clan.Location = items["location"]["name"].ToString();
            switch (items["type"].ToString())
            {
                case "open":
                    clan.Type = ViewModels.Clash.Type.Open;
                    break;
                case "closed":
                    clan.Type = ViewModels.Clash.Type.Closed;
                    break;
                case "inviteOnly":
                    clan.Type = ViewModels.Clash.Type.InviteOnly;
                    break;
                default:
                    break;
            }

            return clan;
        }
    }
}
