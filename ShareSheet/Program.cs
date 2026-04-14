using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_ACCESS_TOKEN";
const string sheetEnv = "SHEET_ID";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

var sheetId = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable(sheetEnv);
if (string.IsNullOrWhiteSpace(sheetId))
{
    Console.Error.WriteLine("Usage: ShareSheet <sheetId>");
    Console.Error.WriteLine($"Or set {sheetId} and run with no arguments.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

CreateShareRequest request = new CreateShareRequest();
request.Email="jim.hinkey@smartsheet.com";
request.Subject="Sheet for review";
request.Message="What do you think of this sheet?";
request.AccessLevel=AccessLevel.VIEWER;

BulkItemResult<AssetShare> itemResult =
    client.AssetSharingResources.ShareAsset(
        assetType: AssetType.SHEET,
        assetId: sheetId, 
        createShareRequests: new List<CreateShareRequest> {request},
        sendEmail: true);

foreach (var result in itemResult.Result)
{
    Console.WriteLine($"Share " + 
        $"\nid: { result.Id} " +
        $"\ntype: { result.Type} " +
        $"\ngroup ID: { result.GroupId} " +
        $"\nemail: { result.Email} " +
        $"\nname: { result.Name} " +
        $"\nuser ID: { result.UserId} " +
        $"\nAccessLevel: { result.AccessLevel} " +
        $"\nScope: { result.Scope} ");
}
