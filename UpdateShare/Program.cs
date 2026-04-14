using Smartsheet.Api;
using Smartsheet.Api.Models;

const string tokenEnv = "SMARTSHEET_ACCESS_TOKEN";
const string sheetEnv = "SHEET_ID";
const string shareEnv = "SHEET_SHARE_ID";

var token = Environment.GetEnvironmentVariable(tokenEnv);
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine($"Set {tokenEnv} to your Smartsheet API access token.");
    Environment.Exit(1);
}

var sheetId = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable(sheetEnv);
if (string.IsNullOrWhiteSpace(sheetId))
{
    Console.Error.WriteLine("Usage: UpdateShare <sheetId>");
    Console.Error.WriteLine($"Or set {sheetId} and run with no arguments.");
    Environment.Exit(1);
}

var shareId = args.Length > 1 ? args[1] : Environment.GetEnvironmentVariable(shareEnv);
if (string.IsNullOrWhiteSpace(shareId))
{
    Console.Error.WriteLine("Usage: UpdateShare <shareId>");
    Console.Error.WriteLine($"Or set {shareId} and run with no arguments.");
    Environment.Exit(1);
}

SmartsheetClient client = new SmartsheetBuilder()
    .SetAccessToken(token)
    .Build();

UpdateShareRequest updateRequest = new UpdateShareRequest();
updateRequest.AccessLevel = AccessLevel.EDITOR;
AssetShare updatedShare = client.AssetSharingResources.UpdateAssetShare(
    assetType: AssetType.SHEET,
    assetId: sheetId,
    shareId: shareId,
    updateShareRequest: updateRequest);

string? lastKey = null;
do
{

    TokenPaginationParameters paginationParameters = 
        new TokenPaginationParameters(lastKey, 100);

    ListAssetSharesResponse response = 
        client.AssetSharingResources.ListAssetShares(
            assetType: AssetType.SHEET,
            assetId: sheetId, 
            tokenPaginationParameters: paginationParameters,
            sharingInclude: null);

    foreach (var item in response.Items)
    {
        Console.WriteLine($"Share \n" + 
            $"Id: { item.Id} \n" +
            $"Type: { item.Type} \n" +
            $"GroupId: { item.GroupId} \n" +
            $"Email: { item.Email} \n" +
            $"Name: { item.Name} \n" +
            $"UserId: { item.UserId} \n" +
            $"AccessLevel: { item.AccessLevel} \n" +
            $"Scope: { item.Scope} ");
    }

    lastKey = response.LastKey;
} while (!string.IsNullOrEmpty(lastKey));